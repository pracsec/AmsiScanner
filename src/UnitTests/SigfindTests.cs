using AmsiScanner.Common;
using AmsiScanner.Common.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using UnitTests.Utilities;

namespace UnitTests {
    [TestClass]
    public class SigfindTests {
        [TestMethod]
        public void SigfindByCharTest() {
            string contents = Utility.ReadAllText(TestPaths.Sample2);
            SignatureFinderByChar sigfind = new SignatureFinderByChar(contents);
            sigfind.VerboseIterationComplete += (iterations, stepSize, steps, delta) => {
                string temp = delta;
                if (delta.Length > 32) {
                    temp = delta.Substring(0, 32);
                }
                Debug.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", iterations, stepSize, steps, delta.Length, temp.Replace("\r\n", ""));
            };
            Assert.IsTrue(sigfind.Status == BackgroundWorkerStatus.NotStarted);
            sigfind.Start();
            sigfind.Wait();
            Assert.IsTrue(sigfind.Status == BackgroundWorkerStatus.Complete);
            Assert.IsTrue(!string.IsNullOrEmpty(sigfind.ReducedString));
            Assert.IsFalse(sigfind.ReducedString.Equals(contents));
            Assert.IsTrue(sigfind.ReducedString.Length < contents.Length);
        }

        [TestMethod]
        public void SigfindByTokenTest() {
            string contents = Utility.ReadAllText(TestPaths.Sample2);
            SignatureFinderByToken sigfind = new SignatureFinderByToken(contents);
            sigfind.VerboseIterationComplete += (iterations, stepSize, steps, delta, tokens) => {
                string temp = delta;
                if (delta.Length > 32) {
                    temp = delta.Substring(0, 32);
                }
                Debug.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", iterations, stepSize, steps, tokens.Length, delta.Length, temp.Replace("\r\n", ""));
            };
            Assert.IsTrue(sigfind.Status == BackgroundWorkerStatus.NotStarted);
            sigfind.Start();
            sigfind.Wait();
            Assert.IsTrue(sigfind.Status == BackgroundWorkerStatus.Complete);
            Assert.IsTrue(!string.IsNullOrEmpty(sigfind.ReducedString));
            Assert.IsFalse(sigfind.ReducedString.Equals(contents));
            Assert.IsTrue(sigfind.ReducedString.Length < contents.Length);
        }
    }
}
