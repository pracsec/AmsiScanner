using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AmsiScanner.Common {

    public static class NativeMethods {
        [DllImport("Amsi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void AmsiCloseSession(IntPtr amsiContext, IntPtr session);

        [DllImport("Amsi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int AmsiInitialize(string appName, out IntPtr amsiContext);

        [DllImport("Amsi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int AmsiOpenSession(IntPtr amsiContext, out IntPtr session);

        [DllImport("Amsi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int AmsiScanBuffer(IntPtr amsiContext, byte[] buffer, uint length, string contentName, IntPtr session, out int result);

        [DllImport("Amsi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int AmsiScanBuffer(IntPtr amsiContext, IntPtr buffer, uint length, string contentName, IntPtr session, out int result);

        [DllImport("Amsi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int AmsiScanString(IntPtr amsiContext, [In] string content, [In] string contentName, IntPtr session, out int result);

        [DllImport("Amsi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void AmsiUninitialize(IntPtr amsiContext);
    }
}
