using AmsiScanner.Common;
using AmsiScanner.Common.Indexing;
using AmsiScanner.Common.Threading;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.IO;

namespace AmsiScanner {
    public class Program {
        private static int Main(string[] args) {
            //Optional Arguments
            Option<bool> quiet = new Option<bool>(new[] { "--quiet", "-q" }, "Suppresses verbose output.");

            Argument<FileInfo> inputPathArgument = new Argument<FileInfo>("path", "Path to the file to analyze. The file may be a text file, zip file containing a single text file, or a file encrypted by crypt.exe.");
            Command scan = new Command("scan", "Use the Anti-Malware Scan Interface (AMSI) to determine if the installed AV finds the file malicious.") {
                inputPathArgument
            };
            scan.Handler = CommandHandler.Create<FileInfo>(new Func<FileInfo, int>(Program.HandleScanCommand));
            scan.TreatUnmatchedTokensAsErrors = true;

            Command encrypt = new Command("encrypt", "Encrypt a file to prevent accidental execution and to make it easier to work with.") {
                new Argument<FileInfo>("path", "A relative or absolute path to the file to encrypt."),
                new Argument<string>("outpath", "A relative or absolute path to export the encrypted file.")
            };
            encrypt.Handler = CommandHandler.Create(new Func<FileInfo, string, int>(Program.HandleEncrypt));
            encrypt.TreatUnmatchedTokensAsErrors = true;

            Command decrypt = new Command("decrypt", "Decrypt a file previously encrypted with this utility.") {
                new Argument<FileInfo>("path", "A relative or absolute path to the file to decrypt."),
                new Argument<string>("outpath", "A relative or absolute path to export the decrypted file.")
            };
            decrypt.Handler = CommandHandler.Create(new Func<FileInfo, string, int>(Program.HandleDecrypt));
            decrypt.TreatUnmatchedTokensAsErrors = true;

            Command sigfindByCharCommand = new Command("char", "Find the 1-minimal smallest set of characters identified by the installed AV as malicious.") {
                inputPathArgument,
                quiet
            };
            sigfindByCharCommand.Handler = CommandHandler.Create<FileInfo, bool>(new Func<FileInfo, bool, int>(Program.HandleSigFindPerChar));
            sigfindByCharCommand.TreatUnmatchedTokensAsErrors = true;

            Command sigfindByTokenCommand = new Command("token", "Find the 1-minimal smallest set of PowerShell tokens identified by the installed AV as malicious.") {
                inputPathArgument,
                quiet
            };
            sigfindByTokenCommand.Handler = CommandHandler.Create<FileInfo, bool>(new Func<FileInfo, bool, int>(Program.HandleSigFindPerToken));
            sigfindByTokenCommand.TreatUnmatchedTokensAsErrors = true;

            Command sigfind = new Command("sigfind", "Find signaturized components of the input file.") {
                sigfindByCharCommand,
                sigfindByTokenCommand
            };
            sigfind.TreatUnmatchedTokensAsErrors = true;

            Command scanBenchmark = new Command("scan", "Benchmark the speed of scanning with AMSI.") {
                new Argument<FileInfo>("path", "A relative or absolute path to the file to analyze. This file can be encrypted by this utility, a single file in a zip file, or a plaintext file. The plaintext can be either text or binary data."),
                new Argument<int>("iterations", "The number of iterations to measure.")
            };
            scanBenchmark.Handler = CommandHandler.Create(new Func<FileInfo, int, int>(Program.HandleBenchmarkScan));
            scanBenchmark.TreatUnmatchedTokensAsErrors = true;

            Command sigfindByCharBenchmark = new Command("char", "Benchmark the speed of sigfind by char with AMSI.") {
                new Argument<DirectoryInfo>("directory", "A relative or absolute path to a directory of input files.")
            };
            sigfindByCharBenchmark.Handler = CommandHandler.Create(new Func<DirectoryInfo, int>(Program.HandleBenchmarkSigfindByChar));
            sigfindByCharBenchmark.TreatUnmatchedTokensAsErrors = true;

            Command sigfindBenchmark = new Command("sigfind", "Benchmark the speed of sigfind with AMSI.") {
                sigfindByCharBenchmark
            };
            sigfindBenchmark.TreatUnmatchedTokensAsErrors = true;

            Command benchmark = new Command("benchmark", "Measure various performance metrics of the application.") {
                scanBenchmark,
                sigfindBenchmark
            };
            sigfind.TreatUnmatchedTokensAsErrors = true;

            RootCommand rootCommand = new RootCommand("Leverage the Anti-Malware Scan Interface (AMSI) to find the 1-minimal smallest set of strings or tokens that the installed Anti-Virus engine will flag as malicious. This tool is primarily designed for PowerShell scripts, but may be used on any type of text file.") {
                scan,
                sigfind,
                encrypt,
                decrypt,
                benchmark
            };
            rootCommand.TreatUnmatchedTokensAsErrors = true;

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Program.HandleCtrlC);

            return rootCommand.Invoke(args);
        }

        private static void HandleCtrlC(object sender, ConsoleCancelEventArgs e) {
            if(Program._worker != null) {
                Console.WriteLine("[!] Interrupt received. Stopping background worker.");
                Program._worker.Stop();
            } else {
                Environment.Exit(0);
            }
        }

        public static int HandleEncrypt(FileInfo path, string outpath) {
            byte[] contents = Utility.ReadAllBytes(path.FullName);
            byte[] encrypted = Utility.Encrypt(contents);
            File.WriteAllBytes(outpath, encrypted);
            return 0;
        }

        public static int HandleDecrypt(FileInfo path, string outpath) {
            byte[] contents = Utility.ReadAllBytes(path.FullName);
            File.WriteAllBytes(outpath, contents);
            return 0;
        }

        public static int HandleScanCommand(FileInfo path) {
            byte[] contents = Utility.ReadAllBytes(path.FullName);
            AmsiResult result = default;
            using (AmsiSession session = new AmsiSession()) {
                string text;
                if (Utility.TryGetText(contents, out text)) {
                    result = session.ScanString(text);
                } else {
                    result = session.ScanData(contents);
                }
            }
            Console.WriteLine(result);
            return 0;
        }

        public static int HandleSigFindPerChar(FileInfo path, bool quiet = false) {
            Program._verbose = !quiet;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            SignatureFinderByChar sigfind = new SignatureFinderByChar(Utility.ReadAllText(path.FullName));
            sigfind.VerboseIterationComplete += new VerboseIterationByCharCompleteEventHandler(Program.OnIterationByCharComplete);
            Program._worker = sigfind;
            sigfind.Start();
            sigfind.Wait();
            watch.Stop();

            Program.WriteVerbose("Signature finding complete.");
            Program.WriteVerbose(string.Format("Time: {0}", new TimeSpan(watch.ElapsedTicks).ToPrettyFormat()));
            Program.WriteVerbose(string.Format("AmsiCalls: {0}", sigfind.AmsiCallCount));
            Program.WriteVerbose(string.Format("Characters: {0}", sigfind.ReducedString.Length));

            Program.WriteVerbose("Reduced String:");
            Console.WriteLine(sigfind.ReducedString);
            
            return 0;
        }

        public static void OnIterationByCharComplete(int iteration, int stepSize, int nSteps, string delta) {
            string output = string.Format("Iteration: {0} Steps: {1} StepSize: {2} Length: {3} Delta: ", iteration, nSteps, stepSize, delta.Length);
            int max = Utility.ConsoleWidth - output.Length - 5;
            string final;
            if (max < 0) {
                final = output;
            } else if(delta.Length > max) {
                final = string.Format("{0}{1}...", output, delta.Substring(0, max - 3));
            } else {
                final = output + delta;
            }
            final = final.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
            Program.WriteVerbose(final);
        }

        public static int HandleSigFindPerToken(FileInfo path, bool quiet = false) {
            Program._verbose = !quiet;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            string script = Utility.ReadAllText(path.FullName);
            SignatureFinderByToken sigfind = new SignatureFinderByToken(script);
            sigfind.VerboseIterationComplete += new VerboseIterationByTokenCompleteEventHandler(Program.OnIterationByTokenComplete);
            Program._worker = sigfind;
            sigfind.Start();
            sigfind.Wait();
            watch.Stop();

            Program.WriteVerbose("Signature finding complete.");
            Program.WriteVerbose(string.Format("Time: {0}", new TimeSpan(watch.ElapsedTicks).ToPrettyFormat()));
            Program.WriteVerbose(string.Format("AmsiCalls: {0}", sigfind.AmsiCallCount));
            Program.WriteVerbose(string.Format("Tokens: {0}", sigfind.ReducedTokens.Length));
            Program.WriteVerbose(string.Format("Characters: {0}", sigfind.ReducedString.Length));
            
            Program.WriteVerbose("Reduced String:");
            Console.WriteLine(sigfind.ReducedString);
            Console.WriteLine();
            Program.WriteVerbose("Reduced Tokens:");
            for (int i = 0; i < sigfind.ReducedTokens.Length; i++) {
                Token current = sigfind.ReducedTokens[i];
                string output = string.Format("Index: {0} Type: {1} Start: {2} Content: ", i, current.TokenType, current.Start);
                int max = Utility.ConsoleWidth - output.Length - 5;
                string trimmed = null;
                if (current.Length > max) {
                    trimmed = string.Format("{0}...", script.Substring(current.Start, max));
                } else {
                    trimmed = script.Substring(current.Start, current.Length);
                }
                Console.WriteLine("{0}{1}", output, trimmed);
            }

            return 0;
        }

        private static void OnIterationByTokenComplete(int iteration, int nSteps, int stepSize, string deltaString, Token[] deltaToken) {
            string output = string.Format("Iteration: {0}\tSteps: {1}\tStepSize: {2}\tLength: {3}\tDelta: ", iteration, nSteps, stepSize, deltaToken.Length);
            int max = Utility.ConsoleWidth - output.Length - 5;
            string final;
            if (max < 0) {
                final = output;
            } else if (deltaString.Length > max) {
                final = string.Format("{0}{1}...", output, deltaString.Substring(0, max - 3));
            } else {
                final = output + deltaString;
            }
            final = final.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
            Program.WriteVerbose(final);
        }

        public static int HandleBenchmarkScan(FileInfo path, int iterations) {
            ScanBenchmarkResults results = Benchmark.Scan(path, iterations);
            Console.WriteLine("[*] Total:   {0}", results.Total.ToPrettyFormat());
            Console.WriteLine("[*] Average: {0}", results.Average.ToPrettyFormat());
            Console.WriteLine("[*] Best:    {0}", results.Best.ToPrettyFormat());
            Console.WriteLine("[*] Worst:   {0}", results.Worst.ToPrettyFormat());

            return 0;
        }

        public static int HandleBenchmarkSigfindByChar(DirectoryInfo directory) {
            SignatureFinderByCharSettings settings = new SignatureFinderByCharSettings();
            IndexPredictor[] deltas = new IndexPredictor[] {
                new LinearPredictor(0,1),
                new RandomPredictor(0,1),
                new MaximalEntropyPredictor(0,1),
                new PreviousPredictor(0,1)
            };

            IndexPredictor[] nablas = new IndexPredictor[] {
                new LinearPredictor(0,1),
                new RandomPredictor(0,1),
                new MaximalEntropyPredictor(0,1),
                new PreviousPredictor(0,1)
            };

            for (int i = 0; i < deltas.Length; i++) {
                settings.DeltaPredictor = deltas[i];
                settings.NablaPredictor = nablas[i];
                SigfindByCharBenchmarkResults results = Benchmark.SigfindPerChar(directory, settings);

                Console.WriteLine("[*] Predictor: {0}", deltas[i].GetType().Name);
                Console.WriteLine("[*] Total:     {0}", results.Total.ToPrettyFormat());
                Console.WriteLine("[*] Average:   {0}", results.Average.ToPrettyFormat());
                Console.WriteLine("[*] Best:      {0}", results.Best.ToPrettyFormat());
                Console.WriteLine("[*] Worst:     {0}", results.Worst.ToPrettyFormat());
                Console.WriteLine("[*] Iters:     {0}", results.TotalIterations);
                Console.WriteLine("[*] AvgIters:  {0}", results.AverageIterations);
                Console.WriteLine("[*] AmsiCalls:    {0}", results.AmsiCalls);
                Console.WriteLine("[*] AvgAmsiCalls: {0}", results.AverageAmsiCalls);
                Console.WriteLine();
            }

            return 0;
        }

        private static void WriteVerbose(string message) {
            if (Program._verbose) {
                Console.WriteLine("[*] {0}", message);
            }
        }

        private static BackgroundWorker _worker = null;
        private static bool _verbose = true;
    }
}
