using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.Utilities {
    public class ProcessOutput {
        public string StandardOutput {
            get;
            private set;
        }

        public string StandardError {
            get;
            private set;
        }

        public int ExitCode {
            get;
            private set;
        }

        public static ProcessOutput Run(string name, string arguments) {
            ProcessOutput result = new ProcessOutput();
            Process process = new Process();
            try {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = name;
                process.StartInfo.Arguments = arguments;

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false)) {
                    using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false)) {
                        process.OutputDataReceived += (sender, e) => {
                            if (e.Data == null) {
                                outputWaitHandle.Set();
                            } else {
                                output.AppendLine(e.Data);
                            }
                        };
                        process.ErrorDataReceived += (sender, e) => {
                            if (e.Data == null) {
                                errorWaitHandle.Set();
                            } else {
                                error.AppendLine(e.Data);
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        outputWaitHandle.WaitOne();
                        errorWaitHandle.WaitOne();
                    }
                }

                process.WaitForExit();

                result.StandardOutput = output.ToString();
                result.StandardError = error.ToString();
                result.ExitCode =  process.ExitCode;
                return result;
            } finally {
                if (process != null && !process.HasExited) {
                    process.Kill();
                }
            }
        }
    }
}
