using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.ExtendedMath {
    public static class Functions {
        public static long MultiplicativeInverse(long x, long modulus) {
            return Functions.ExtendedEuclideanDivision(x, modulus).Item1 % modulus;
        }

        private static Tuple<long, long> ExtendedEuclideanDivision(long a, long b) {
            if (a < 0) {
                var result = Functions.ExtendedEuclideanDivision(-a, b);
                return Tuple.Create(-result.Item1, result.Item2);
            }
            if (b < 0) {
                var result = Functions.ExtendedEuclideanDivision(a, -b);
                return Tuple.Create(result.Item1, -result.Item2);
            }
            if (b == 0) {
                return Tuple.Create(1L, 0L);
            }
            var q = a / b;
            var r = a % b;
            var rslt = Functions.ExtendedEuclideanDivision(b, r);
            var s = rslt.Item1;
            var t = rslt.Item2;
            return Tuple.Create(t, s - q * t);
        }

        public static long GenerateCoprime(long a, int min, Random random) {
            long x = (long)random.Next(min, int.MaxValue);
            while(Functions.GCD(a, x) != 1) {
                x = (long)random.Next(min, int.MaxValue);
            }
            return x;
        }

        public static int Mod(int k, int n) {
            return ((k %= n) < 0) ? k + n : k;
        }

        public static ulong GCD(long a, long b) {
            return Functions.GCD((ulong)a, (ulong)b);
        }

        public static ulong GCD(ulong a, ulong b) {
            while (a != 0 && b != 0) {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }
    }
}
