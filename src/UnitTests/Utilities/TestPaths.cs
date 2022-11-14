using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Utilities {
    public static class TestPaths {
        public static string AssemblyPath {
            get {
                return Assembly.GetExecutingAssembly().Location;
            }
        }

        public static string AssemblyDirectory {
            get {
                return Directory.GetParent(TestPaths.AssemblyPath).FullName;
            }
        }

        public static string SamplesDirectory {
            get {
                string top = Directory.GetParent(TestPaths.AssemblyDirectory).Parent.Parent.Parent.FullName;
                return Path.Combine(top, "dependencies", "samples");
            }
        }

        public static string Sample0 {
            get {
                return Path.Combine(TestPaths.SamplesDirectory, "00e05857a7e441a0ac139e5d92df642d.bin");
            }
        }

        public static string Sample1 {
            get {
                return Path.Combine(TestPaths.SamplesDirectory, "0dae8f156ded412a88d1244dd48c5a06.bin");
            }
        }

        public static string Sample2 {
            get {
                return Path.Combine(TestPaths.SamplesDirectory, "0f1af4e2b55a4446b16bdf5cb6061dd1.bin");
            }
        }

        public static string Sample3 {
            get {
                return Path.Combine(TestPaths.SamplesDirectory, "69dafba15635b58f0add36a550b82d81.bin");
            }
        }
    }
}
