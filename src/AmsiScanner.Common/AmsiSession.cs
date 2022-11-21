using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmsiScanner.Common {
    public class AmsiSession : IDisposable {
        /// <summary>
        /// The number of calls to AMSI made during the session. This can be used to evaluate the performance of various algorithms. The less number of calls, the beter.
        /// </summary>
        public long AmsiCallCount {
            get {
                return Interlocked.Read(ref this._amsiCalls);
            }
        }

        /// <summary>
        /// AmsiSession contains native references used for executing scans of files, strings, and other data using the Anitmalware Scan Interface.
        /// </summary>
        public AmsiSession() : this(string.Format("{0}_{1}_{2}", "PowerShell", Utility.PowerShellPath, Utility.PowerShellVersion.ProductVersion)) { }

        /// <summary>
        /// AmsiSession contains native references used for executing scans of files, strings, and other data using the Anitmalware Scan Interface.
        /// </summary>
        /// <param name="application">A description of the application.</param>
        /// <param name="correlated">True if signatures should be applied to all data submitted or just the current data.</param>
        /// <param name="cache">True if results should be cached to reduce duplicate calls to AMSI. This can improve performance.</param>
        /// <exception cref="Exception">An exception will be thrown if AMSI cannot be initialized.</exception>
        public AmsiSession(string application, bool correlated = false, bool cache = true) {
            this._cache = cache;
            this._correlated = correlated;
            int resultCode = NativeMethods.AmsiInitialize(application, out this._context);
            if (resultCode != 0) {
                throw new Exception(string.Format("Call to AmsiInitialize failed with return code {0}.", resultCode));
            }

            if (correlated) {
                resultCode = NativeMethods.AmsiOpenSession(this._context, out this._session);
                if (resultCode != 0) {
                    this.Dispose();
                    throw new Exception(string.Format("Call to AmsiOpenSession failed with return code {0}.", resultCode));
                }
            }

            //We get different results from Windows Defender when the AmsiScanBuffer call comes from
            //the AmsiUtil class within System.Management.Automation.dll
            Type type = typeof(PSObject).Assembly.GetType("System.Management.Automation.AmsiUtils");
            this._scanContent = type.GetMethod("ScanContent", BindingFlags.NonPublic | BindingFlags.Static);

            this._cachedResults = new ConcurrentDictionary<string, AmsiResult>();
        }

        /// <summary>
        /// Scans the specified file and tries to determine heuristically if the file contains text. If it contains text, then it attempts to convert to UTF-16LE prior to scanning with AmsiScanBuffer.
        /// </summary>
        /// <param name="path">The file to scan.</param>
        /// <returns>Returns an AmsiResult enumeration (https://learn.microsoft.com/en-us/windows/win32/api/amsi/ne-amsi-amsi_result).</returns>
        public AmsiResult ScanFile(FileInfo path) {
            byte[] contents = Utility.ReadAllBytes(path.FullName);
            string text;
            if (Utility.TryGetText(contents, out text)) {
                return this.ScanString(text);
            } else {
                return this.ScanData(contents);
            }
        }

        /// <summary>
        /// Scans the specified file using the AmsiScanBuffer method. This method assumes that the data is text and attempts to convert it to UTF-16LE encoding prior to scanning. Many AVs assume text data coming in is UTF-16LE as that is the default encoding for strings stored in memory within .NET.
        /// </summary>
        /// <param name="path">The file to scan.</param>
        /// <returns>Returns an AmsiResult enumeration (https://learn.microsoft.com/en-us/windows/win32/api/amsi/ne-amsi-amsi_result).</returns>
        public AmsiResult ScanTextFile(FileInfo path) {
            string script = Utility.ReadAllText(path.FullName);
            return this.ScanString(script);
        }

        /// <summary>
        /// Scans the specified file using the AmsiScanBuffer method. This method assumes the file is binary data and does not try to interpret it as text.
        /// </summary>
        /// <param name="path">The file to scan.</param>
        /// <returns>Returns an AmsiResult enumeration (https://learn.microsoft.com/en-us/windows/win32/api/amsi/ne-amsi-amsi_result).</returns>
        public AmsiResult ScanBinaryFile(FileInfo path) {
            byte[] data = Utility.ReadAllBytes(path.FullName);
            return this.ScanData(data);
        }

        /// <summary>
        /// Scans a PowerShell script by attempting to execute it. Actual execution is prevented by inserting a small exit function at the beginning of the script.
        /// </summary>
        /// <param name="text">The script to scan.</param>
        /// <returns>Returns an AmsiResult enumeration (https://learn.microsoft.com/en-us/windows/win32/api/amsi/ne-amsi-amsi_result).</returns>
        public AmsiResult ScanPowerShellScript(string text) {
            AmsiResult result = AmsiResult.NotDetected;
            if (this._cache && this._cachedResults.TryGetValue(text, out result)) {
                return result;
            }

            Interlocked.Increment(ref this._amsiCalls);
            string neutered = AmsiSession.ESCAPE_STUB + text;
            PowerShell psh = PowerShell.Create();
            psh.Runspace.SessionStateProxy.SetVariable("execute", true);
            psh.AddScript(neutered);

            try {
                PSObject[] results = psh.Invoke().ToArray();
            } catch (ParseException ex) {
                if (ex.Message.Contains(AmsiSession.MALICIOUS_CONTENT)) {
                    result = AmsiResult.Detected;
                }
            } catch (Exception ex) { }

            if (this._cache) {
                this._cachedResults.TryAdd(text, result);
            }

            return result;
        }

        /// <summary>
        /// This method reflectively calls the System.Management.Automation.AmsiUtils.ScanContent method because it gives different results with Windows Defender than calling AmsiScanBuffer directly.
        /// </summary>
        /// <param name="text">The text to scan.</param>
        /// <returns>Returns an AmsiResult enumeration (https://learn.microsoft.com/en-us/windows/win32/api/amsi/ne-amsi-amsi_result).</returns>
        public AmsiResult ScanStringReflective(string text) {
            AmsiResult amsiResult = AmsiResult.NotDetected;
            if (this._cache && this._cachedResults.TryGetValue(text, out amsiResult)) {
                return amsiResult;
            }

            //We get different results from Windows Defender when the AmsiScanBuffer call comes from
            //the AmsiUtil class within System.Management.Automation.dll
            //Type type = typeof(PSObject).Assembly.GetType("System.Management.Automation.AmsiUtils");

            //Reset the AmsiSession field to prevent correlation between seperate scans
            if (!this._correlated) {
                Type type = typeof(PSObject).Assembly.GetType("System.Management.Automation.AmsiUtils");
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
                FieldInfo contextField = type.GetField("s_amsiContext", BindingFlags.NonPublic | BindingFlags.Static);
                IntPtr context = (IntPtr)contextField.GetValue(null);
                FieldInfo sessionField = type.GetField("s_amsiSession", BindingFlags.NonPublic | BindingFlags.Static);
                IntPtr session = (IntPtr)sessionField.GetValue(null);
                if(context != IntPtr.Zero && session != IntPtr.Zero) {
                    NativeMethods.AmsiCloseSession(context, session);
                    sessionField.SetValue(null, IntPtr.Zero);
                }
            }

            //MethodInfo method = type.GetMethod("ScanContent", BindingFlags.NonPublic | BindingFlags.Static);
            Interlocked.Increment(ref this._amsiCalls);
            int result = (int)this._scanContent.Invoke(null, new object[] { text, string.Empty });

            amsiResult = AmsiSession.IntToAmsiResult(result);

            if (this._cache) {
                this._cachedResults.TryAdd(text, amsiResult);
            }

            return amsiResult;
        }

        /// <summary>
        /// Scans the provided text using AmsiScanBuffer. The text is passed in with UTF-16LE encoding.
        /// </summary>
        /// <param name="text">The text to scan.</param>
        /// <returns>Returns an AmsiResult enumeration (https://learn.microsoft.com/en-us/windows/win32/api/amsi/ne-amsi-amsi_result).</returns>
        /// <exception cref="Exception"></exception>
        public unsafe AmsiResult ScanString(string text) {
            AmsiResult result = AmsiResult.NotDetected;
            if (this._cache && this._cachedResults.TryGetValue(text, out result)) {
                return result;
            }

            string meta = string.Empty;
            Interlocked.Increment(ref this._amsiCalls);

            int amsiResult = 0;
            int resultCode = 0;
            fixed (char* chPtr = text) {
                IntPtr buffer = new IntPtr((void*)chPtr);
                resultCode = NativeMethods.AmsiScanBuffer(this._context, buffer, (uint)(text.Length * 2), meta, this._session, out amsiResult);
            }

            if (resultCode != 0) {
                throw new Exception(string.Format("Call to AmsiScanString failed with return code {0}.", resultCode));
            }

            result = AmsiSession.IntToAmsiResult(amsiResult);

            if (this._cache) {
                this._cachedResults.TryAdd(text, result);
            }

            return result;
        }

        /// <summary>
        /// Scans the provided data using AmsiScanBuffer.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Returns an AmsiResult enumeration (https://learn.microsoft.com/en-us/windows/win32/api/amsi/ne-amsi-amsi_result).</returns>
        /// <exception cref="Exception"></exception>
        public AmsiResult ScanData(Byte[] data) {
            string str = Guid.NewGuid().ToString();
            Interlocked.Increment(ref this._amsiCalls);
            int amsiResult;
            int resultCode = NativeMethods.AmsiScanBuffer(this._context, data, (uint)data.Length, str, this._session, out amsiResult);
            if (resultCode != 0) {
                throw new Exception(string.Format("Call to AmsiScanString failed with return code {0}.", resultCode));
            }

            return AmsiSession.IntToAmsiResult(amsiResult);
        }

        public static bool IsDetected(int result) {
            return result >= AmsiUtility.AMSI_RESULT_DETECTED;
        }

        public void Dispose() {
            if (this._session != IntPtr.Zero && this._context != IntPtr.Zero) {
                NativeMethods.AmsiCloseSession(this._context, this._session);
            }
        }

        private static AmsiResult IntToAmsiResult(int amsiResult) {
            if (amsiResult >= AmsiUtility.AMSI_RESULT_DETECTED) {
                return AmsiResult.Detected; ;
            } else if (amsiResult >= AmsiUtility.AMSI_RESULT_BLOCKED_BY_ADMIN_START && amsiResult <= AmsiUtility.AMSI_RESULT_BLOCKED_BY_ADMIN_END) {
                return AmsiResult.BlockedByAdmin;
            } else if (amsiResult == AmsiUtility.AMSI_RESULT_CLEAN) {
                return AmsiResult.Clean;
            } else {
                return AmsiResult.NotDetected;
            }
        }

        private readonly IntPtr _context = IntPtr.Zero;
        private readonly IntPtr _session = IntPtr.Zero;
        private readonly ConcurrentDictionary<string, AmsiResult> _cachedResults;
        private readonly bool _cache;
        private readonly bool _correlated;
        private long _amsiCalls = 0;
        private MethodInfo _scanContent;

        private const string ESCAPE_STUB = "if($escape) { return; }\r\n";
        private const string MALICIOUS_CONTENT = "script contains malicious content";
    }
}
