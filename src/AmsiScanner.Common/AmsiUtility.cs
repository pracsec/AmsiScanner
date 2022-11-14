using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmsiScanner.Common {
    public static class AmsiUtility {
        public const int AMSI_RESULT_CLEAN = 0;
        public const int AMSI_RESULT_NOT_DETECTED = 1;
        public const int AMSI_RESULT_BLOCKED_BY_ADMIN_START = 16384;
        public const int AMSI_RESULT_BLOCKED_BY_ADMIN_END = 20479;
        public const int AMSI_RESULT_DETECTED = 32768;

        public static string EICAR {
            get;
            private set;
        }

        static AmsiUtility() {
            AmsiUtility.EICAR = Utility.DecryptBase64(AmsiUtility.EICAR_ENCRYPTED);
        }

        public static Token[] Tokenize(string script) {
            try {
                AmsiUtility.DisableAmsi();

                return Token.Tokenize(script).ToArray();
            } finally {
                AmsiUtility.EnableAmsi();
            }
        }

        public static bool IsEnabled {
            get {
                lock (AmsiUtility._lock) {
                    return AmsiUtility._enabled;
                }
            }
        }

        public static void DisableAmsi() {
            lock (AmsiUtility._lock) {
                if (AmsiUtility._enabled) {
                    AmsiUtility.Swap();
                }
            }
        }

        public static void EnableAmsi() {
            lock (AmsiUtility._lock) {
                if (!AmsiUtility._enabled) {
                    AmsiUtility.Swap();
                }
            }
        }

        private static void Swap() {
            Type? type = typeof(PSObject).Assembly.GetType(Utility.DecryptBase64("vx8Ds0LlV2hYEWs6/PFi25WYYDc8RD41EQb41u8mvkj/dQMrc0K0ij3rYA+KTSe3qCTayJVRV/64Ix8cB4TTKyzTKGcvgH/3DX0GcBw59l4="));
            //FieldInfo amsiSessionField = type.GetField("amsiSession", BindingFlags.NonPublic | BindingFlags.Static);
            
            if(type == null) {
                throw new ItemNotFoundException();
            }
            
            FieldInfo? amsiSessionField = type.GetField("s_amsiSession", BindingFlags.NonPublic | BindingFlags.Static);
            IntPtr session = AmsiUtility._session;
            object? tsession = amsiSessionField.GetValue(null);
            AmsiUtility._session = (IntPtr)tsession;
            amsiSessionField.SetValue(null, null);
            amsiSessionField.SetValue(null, session);
            AmsiUtility._enabled = !AmsiUtility._enabled;
        }

        private static void SetAmsiEnabled(bool enabled) {
            //[Ref].Assembly.GetType('System.Management.Automation.AmsiUtils').GetField('amsiInitFailed','NonPublic,Static').SetValue($null,$true)
            Type type = typeof(PSObject).Assembly.GetType(Utility.DecryptBase64("vx8Ds0LlV2hYEWs6/PFi25WYYDc8RD41EQb41u8mvkj/dQMrc0K0ij3rYA+KTSe3qCTayJVRV/64Ix8cB4TTKyzTKGcvgH/3DX0GcBw59l4="));
            FieldInfo amsiSessionField = type.GetField("amsiInitFailed", BindingFlags.NonPublic | BindingFlags.Static);
            amsiSessionField.SetValue(null, enabled);
        }

        private static IntPtr _session = Marshal.AllocHGlobal(9076);
        private static bool _enabled = true;
        private static readonly object _lock = new object();
        private const string EICAR_ENCRYPTED = "lAXU7x6NlbNHXwGXSTia99s591AMJYEVVVtf64V6lHWFgeIRTk4Dn2XWWYcnDriXRHX1a3ExbAze8uuy0jSPwDwFCQ5JUGsoZ5YIIIVXOr8snFK3F03swg6aO+mB5S2bESiqZLLzG/Wj7E0CRe1NGA==";
    }
}
