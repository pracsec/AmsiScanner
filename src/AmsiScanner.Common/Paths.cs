using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AmsiScanner.Common {
    public static class Paths {
        public static string AmsiScanPath {
            get {
                return Path.Combine(Paths.ProgramDirectory, "amsiscan.exe");
            }
        }

        public static string ProgramDirectory {
            get {
                return Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            }
        }

        public static string PSParsePath {
            get {
                return Path.Combine(Paths.ProgramDirectory, "psparse.exe");
            }
        }

        public static string SamplesDirectory {
            get {
                try {
                    string str = Path.Combine((new DirectoryInfo(Paths.ProgramDirectory)).Parent.Parent.FullName, "samples");
                    if (Directory.Exists(str)) {
                        return str;
                    }
                } catch { }
                return Path.Combine(Paths.ProgramDirectory, "samples");
            }
        }
    }
}
