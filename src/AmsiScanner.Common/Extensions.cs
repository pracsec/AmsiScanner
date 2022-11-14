using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common {
    public static class Extensions {
        public static string ToPrettyFormat(this TimeSpan span) {
            StringBuilder builder = new StringBuilder();
            
            if (span.Days > 0) {
                builder.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : String.Empty);
            }

            if (span.Hours > 0) {
                builder.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : String.Empty);
            }

            if (span.Minutes > 0) {
                builder.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : String.Empty);
            }

            if (span.Seconds > 0) {
                builder.AppendFormat("{0} second{1} ", span.Seconds, span.Seconds > 1 ? "s" : String.Empty);
            }

            if (span.Milliseconds > 0) {
                builder.AppendFormat("{0} millisecond{1} ", span.Milliseconds, span.Milliseconds > 1 ? "s" : String.Empty);
            }

            return builder.ToString();
        }

        public static void Shuffle<T>(this Random rng, T[] array) {
            int n = array.Length;
            while (n > 1) {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static string GetVersion(this Assembly assembly) {
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }
    }
}
