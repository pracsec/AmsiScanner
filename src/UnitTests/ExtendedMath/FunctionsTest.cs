using AmsiScanner.Common.ExtendedMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace UnitTests.ExtendedMath {
    [TestClass]
    public class FunctionsTest {
        [TestMethod]
        public void GenerateCoprimesTest() {
            Random random = new Random(0);
            long a = 128;
            long result = Functions.GenerateCoprime(a, 257854854, random);
            Assert.IsTrue(Functions.GCD(a, result) == 1);
        }

        [TestMethod]
        public void GenerateCoprimesTimingTest() {
            Random random = new Random(0);
            long a = 156;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int count = 1000;
            for (int i = 0; i < count; i++) {
                long result = Functions.GenerateCoprime(a, 257854854, random);
            }
            watch.Stop();
            Debug.WriteLine("[*] Avg: {0}", watch.ElapsedMilliseconds / (float)count);
        }
    }
}
