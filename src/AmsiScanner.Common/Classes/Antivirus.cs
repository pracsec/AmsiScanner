using System.Management;
using System.Runtime.InteropServices;

namespace AmsiScanner.Common.Classes {
    public static class Antivirus {
        /// <summary>
        /// Get AV names.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string[] GetInstalledAntivirus() {
            if (Antivirus.IsWindowsServer()) {
                throw new Exception("Cannot enumerate antivirus. root\\SecurityCenter2 WMI namespace is not available on Windows Servers");
            }

            List<string> results = new List<string>();

            try {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");

                foreach (ManagementBaseObject virusChecker in searcher.Get()) {
                    results.Add((string)virusChecker["displayName"]);
                }
            } catch { }

            return results.ToArray();
        }

        private static bool IsWindowsServer() {
            return IsOS(OS_ANYSERVER);
        }

        private const int OS_ANYSERVER = 29;

        [DllImport("shlwapi.dll", SetLastError = true, EntryPoint = "#437")]
        private static extern bool IsOS(int os);
    }
}
