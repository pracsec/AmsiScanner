using AmsiScanner.Common.Classes;
using AmsiScanner.Common.Indexing;
using AmsiScanner.Common.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmsiScanner.Common {
    public class SignatureFinderByCharSettings {
        public int MinimumDeltaStepSize {
            get { return this._minimimDeltaStepSize; }
            private set {
                if(value < 1) {
                    throw new ArgumentException("The MinimumDeltaStepSize must be greater than 1.", "MinimumDeltaStepSize");
                }

                this._minimimDeltaStepSize = value;
            }
        }

        public bool Verbose {
            get;
            private set;
        } = true;

        public IndexPredictor DeltaPredictor {
            get;
            set;
        } = new PreviousPredictor(0, 1);

        public IndexPredictor NablaPredictor {
            get;
            set;
        } = new PreviousPredictor(0, 1);

        public SigfindMethod Method {
            get;
            set;
        } = SigfindMethod.PowerShell;

        public SignatureFinderByCharSettings(int minimumDeltaStepSize = 6, bool verbose = true) {
            this.Verbose = verbose;
            this.MinimumDeltaStepSize = minimumDeltaStepSize;
        }

        private int _minimimDeltaStepSize = 6;
    }

    public delegate void VerboseIterationByCharCompleteEventHandler(int iteration, int stepSise, int nSteps, string delta);
    public delegate void VerboseMinimizationByCharCompleteEventHandler(string reduced);

    public class SignatureFinderByChar : BackgroundWorker {
        public event VerboseIterationByCharCompleteEventHandler VerboseIterationComplete;
        public event VerboseMinimizationByCharCompleteEventHandler VerboseMinimizationComplete;

        public string ReducedString {
            get {
                lock (this._deltaLock) {
                    return this._delta;
                }
            }

            private set {
                lock (this._deltaLock) {
                    this._delta = value;
                }
            }
        }

        public long AmsiCallCount {
            get {
                return Interlocked.Read(ref this._amsiCallCount);
            }
        }

        public SignatureFinderByChar(string text) {
            this._settings = new SignatureFinderByCharSettings();
            this._text = text;
        }

        public SignatureFinderByChar(string text, SignatureFinderByCharSettings settings) {
            this._text = text;
            this._settings = settings;
        }

        protected override void Run(CancellationToken token) {
            using (AmsiSession session = new AmsiSession()) {
                string str = this._text;
                this.ReducedString = str;

                Interlocked.Increment(ref this._amsiCallCount);
                if (this.ScanString(session, str) != AmsiResult.Detected) {
                    return;
                }

                int iteration = 0;
                int nSteps = 2;
                while (nSteps < str.Length * 2 && !token.IsCancellationRequested) {
                    this.ReducedString = str;

                    if (nSteps > str.Length) {
                        nSteps = str.Length;
                    }

                    int stepSize = (int)Math.Ceiling(str.Length / (double)nSteps);

                    //Raise the iteration event handler
                    this.RaiseVerboseIterationCompleteEvent(iteration, stepSize, nSteps, str);

                    //Increment the iteration
                    iteration++;

                    //Generate the deltas
                    string[] strArrays = SignatureFinderByChar.GenerateDeltas(str, stepSize);

                    //Only scan deltas if the step size is large enough
                    bool flag = false;
                    string[] strArrays1 = strArrays;
                    int num1;
                    if (stepSize > this._settings.MinimumDeltaStepSize) {
                        this._settings.DeltaPredictor.Reset(0, strArrays1.Length - 1);
                        num1 = this._settings.DeltaPredictor.Next();
                        int count = 0;
                        while (this._settings.DeltaPredictor.HasNext) {
                            //Only check for cancellation every 10 iterations
                            if (count++ % 10 == 0) {
                                if (token.IsCancellationRequested) {
                                    break;
                                }
                            }

                            this._amsiCallCount = session.AmsiCallCount;
                            string str1 = strArrays1[num1];
                            if (this.ScanString(session, str1) != AmsiResult.Detected) {
                                num1 = this._settings.DeltaPredictor.Next();
                            } else {
                                flag = true;
                                nSteps = 2;
                                str = str1;
                                break;
                            }
                        }
                        if (flag) {
                            continue;
                        }
                    }

                    //Dont need to generate nablas if the number of steps is 2
                    if (nSteps != 2) {
                        strArrays1 = SignatureFinderByChar.GenerateNablas(strArrays);
                        this._settings.NablaPredictor.Reset(0, strArrays1.Length - 1);
                        num1 = this._settings.NablaPredictor.Next();
                        int count = 0;
                        while (this._settings.NablaPredictor.HasNext) {
                            //Only check for  cancellation every 10 iterations
                            if (count++ % 10 == 0) {
                                if (token.IsCancellationRequested) {
                                    break;
                                }
                            }

                            this._amsiCallCount = session.AmsiCallCount;
                            string str2 = strArrays1[num1];
                            if (this.ScanString(session, str2) != AmsiResult.Detected) {
                                num1 = this._settings.NablaPredictor.Next();
                            } else {
                                flag = true;
                                nSteps--;
                                str = str2;
                                break;
                            }
                        }
                        if (flag) {
                            continue;
                        }
                        nSteps *= 2;
                    } else {
                        nSteps *= 2;
                    }
                }
                this._amsiCallCount = session.AmsiCallCount;                    
            }
        }

        protected override void OnStopping() {
            this.RaiseMinimizationCompleteEvent(this._delta);
        }

        private AmsiResult ScanString(AmsiSession session, string text) {
            if(this._settings.Method == SigfindMethod.PowerShell) {
                return session.ScanStringReflective(text);
            } else {
                return session.ScanString(text);
            }
        }

        private static string[] GenerateDeltas(string delta, int step) {
            LinkedList<string> strs = new LinkedList<string>();
            for (int i = 0; i < delta.Length; i += step) {
                string str = delta.Substring(i, Math.Min(delta.Length - i, step));
                strs.AddLast(str);
            }
            return strs.ToArray<string>();
        }

        private static string[] GenerateNablas(string[] deltas) {
            LinkedList<string> strs = new LinkedList<string>();
            for (int i = 0; i < (int)deltas.Length; i++) {
                StringBuilder stringBuilder = new StringBuilder();
                for (int j = 0; j < (int)deltas.Length; j++) {
                    if (i != j) {
                        stringBuilder.Append(deltas[j]);
                    }
                }
                strs.AddLast(stringBuilder.ToString());
            }
            return strs.ToArray<string>();
        }

        private void RaiseVerboseIterationCompleteEvent(int iteration, int stepSize, int nSteps, string delta) {
            try {
                if (this.VerboseIterationComplete != null) {
                    this.VerboseIterationComplete(iteration, stepSize, nSteps, delta);
                }
            } catch { }
        }

        private void RaiseMinimizationCompleteEvent(string reduced) {
            try {
                if (this.VerboseMinimizationComplete != null) {
                    this.VerboseMinimizationComplete(reduced);
                }
            } catch { }
        }

        private readonly SignatureFinderByCharSettings _settings;

        private readonly string _text;
        private string _delta;
        private readonly object _deltaLock = new object();
        private long _amsiCallCount;
    }
}
