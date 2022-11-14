using AmsiScanner.Common.Classes;
using AmsiScanner.Common.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Cmdlets {

    public enum SigfindType {
        Char,
        Token
    }

    [Cmdlet(VerbsLifecycle.Invoke, "Sigfind", DefaultParameterSetName = InvokeSigfind.PARAMETER_SET_FILE)]
    public class InvokeSigfind : PSCmdlet {
        [Parameter(ParameterSetName = InvokeSigfind.PARAMETER_SET_FILE, Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public FileInfo File {
            get;
            set;
        }

        [Parameter(ParameterSetName = InvokeSigfind.PARAMETER_SET_TEXT, Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNull]
        public string Text {
            get;
            set;
        }

        [Parameter(Mandatory = false)]
        public SigfindType Type {
            get;
            set;
        } = SigfindType.Char;

        [Parameter(Mandatory = false)]
        public int MinimumDeltaLength {
            get;
            set;
        } = 6;

        protected override void BeginProcessing() {
            //Only handle scans here if no pipeline input is expected
            if (!this.MyInvocation.ExpectingInput) {
                this.HandleScans();
            }
        }

        protected override void ProcessRecord() {
            //Only handle scans here if pipeline input is expected
            if (this.MyInvocation.ExpectingInput) {
                this.HandleScans();
            }
        }

        protected override void StopProcessing() {
            this._worker.Stop();
        }

        private void HandleScans() {
            string text = this.Text;
            if (this.ParameterSetName.Equals(InvokeSigfind.PARAMETER_SET_FILE)) {
                text = Utility.ReadAllText(this.File.FullName);
            } else {
                string path;
                if (Utility.TryGetFullPath(this.SessionState.Path.CurrentFileSystemLocation.Path, this.Text, out path)) {
                    text = Utility.ReadAllText(path);
                }
            }

            if (this.Type == SigfindType.Char) {
                this.HandleSigfindByChar(text);
            } else if (this.Type == SigfindType.Token) {
                this.HandleSigfindByToken(text);
            }
        }

        private void HandleSigfindByChar(string text) {
            SignatureFinderByCharSettings settings = new SignatureFinderByCharSettings(minimumDeltaStepSize: this.MinimumDeltaLength);
            SignatureFinderByChar sigfind = new SignatureFinderByChar(text, settings);
            sigfind.VerboseIterationComplete += new VerboseIterationByCharCompleteEventHandler(this.OnSigfindByCharIntervalComplete);
            this._worker = sigfind;
            Stopwatch watch = Stopwatch.StartNew();
            this._worker.Start();
            while (this._worker.Status != BackgroundWorkerStatus.Complete) {
                this._worker.Wait(100);
                string message;
                while (this._vervoseMessageQueue.TryDequeue(out message)) {
                    this.WriteVerbose(message);
                }
            }
            watch.Stop();

            this.WriteVerbose("Signature finding complete.");
            this.WriteVerbose(string.Format("Time: {0}", new TimeSpan(watch.ElapsedTicks).ToPrettyFormat()));
            this.WriteVerbose(string.Format("AmsiCalls: {0}", sigfind.AmsiCallCount));
            this.WriteVerbose(string.Format("Characters: {0}", sigfind.ReducedString.Length));

            this.WriteVerbose("Reduced String:");
            this.WriteObject(sigfind.ReducedString);
        }

        private void HandleSigfindByToken(string text) {
            SignatureFinderByTokenSettings settings = new SignatureFinderByTokenSettings(minimumDeltaStepSize: this.MinimumDeltaLength);
            SignatureFinderByToken sigfind = new SignatureFinderByToken(text, settings);
            sigfind.VerboseIterationComplete += new VerboseIterationByTokenCompleteEventHandler(this.OnSigfindByTokenIntervalComplete);
            this._worker = sigfind;
            Stopwatch watch = Stopwatch.StartNew();
            this._worker.Start();
            while (this._worker.Status != BackgroundWorkerStatus.Complete) {
                this._worker.Wait(100);
                string message;
                while(this._vervoseMessageQueue.TryDequeue(out message)) {
                    this.WriteVerbose(message);
                }
            }
            watch.Stop();

            this.WriteVerbose("Signature finding complete.");
            this.WriteVerbose(string.Format("Time: {0}", new TimeSpan(watch.ElapsedTicks).ToPrettyFormat()));
            this.WriteVerbose(string.Format("AmsiCalls: {0}", sigfind.AmsiCallCount));
            this.WriteVerbose(string.Format("Characters: {0}", sigfind.ReducedString.Length));

            this.WriteVerbose("Reduced String:");
            this.WriteObject(new SigfindByTokenResults(sigfind.ReducedString, sigfind.ReducedTokens));
        }

        private void OnSigfindByTokenIntervalComplete(int iteration, int nSteps, int stepSize, string deltaString, Token[] deltaToken) {
            string output = string.Format("Iteration: {0}\tSteps: {1}\tStepSize: {2}\tLength: {3}\tDelta: ", iteration, nSteps, stepSize, deltaToken.Length);
            int max = Utility.ConsoleWidth - output.Length - 15;
            string final;
            if (max < 0) {
                final = output;
            } else if (deltaString.Length > max) {
                final = string.Format("{0}{1}...", output, deltaString.Substring(0, max - 3));
            } else {
                final = output + deltaString;
            }
            final = final.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
            this._vervoseMessageQueue.Enqueue(final);
        }

        private void OnSigfindByCharIntervalComplete(int iteration, int stepSize, int nSteps, string delta) {
            string output = string.Format("Iteration: {0} Steps: {1} StepSize: {2} Length: {3} Delta: ", iteration, nSteps, stepSize, delta.Length);
            int max = Utility.ConsoleWidth - output.Length - 15;
            string final;
            if (max < 0) {
                final = output;
            } else if (delta.Length > max) {
                final = string.Format("{0}{1}...", output, delta.Substring(0, max - 3));
            } else {
                final = output + delta;
            }
            final = final.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
            this._vervoseMessageQueue.Enqueue(final);
        }

        private BackgroundWorker _worker;
        private ConcurrentQueue<string> _vervoseMessageQueue = new ConcurrentQueue<string>();

        public const string PARAMETER_SET_FILE = "File";
        public const string PARAMETER_SET_TEXT = "Text";
    }
}
