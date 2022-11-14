using AmsiScanner;
using AmsiScanner.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using UnitTests.Utilities;

namespace UnitTests {
    [TestClass]
    public class AmsiScannerUnitTests {
        [TestMethod]
        public void Encrypt() {
            using (TemporaryFile src = new TemporaryFile()) {
                using (TemporaryFile dst = new TemporaryFile()) {
                    Program.HandleEncrypt(src.FileInfo, dst.FileInfo.FullName);
                    Assert.IsTrue(dst.FileInfo.Exists);
                    Assert.IsTrue(dst.FileInfo.Length >= src.FileInfo.Length);
                    Assert.IsFalse(src.CompareTo(dst) == 0);
                    byte[] decrypted = Utility.Decrypt(dst.CurrentContents);
                    Assert.AreEqual(decrypted.Length, src.FileInfo.Length);
                    Assert.IsTrue(src.OriginalContents.SequenceEqual(decrypted));
                }
            }
        }

        [TestMethod]
        public void Decrypt() {
            using (TemporaryFile src = new TemporaryFile()) {
                using (TemporaryFile dst = new TemporaryFile()) {
                    byte[] plaintext = Generate.ByteArray();
                    byte[] encrypted = Utility.Encrypt(plaintext);
                    File.WriteAllBytes(src.FileInfo.FullName, encrypted);
                    Program.HandleDecrypt(src.FileInfo, dst.FileInfo.FullName);
                    Assert.IsTrue(dst.CurrentContents.SequenceEqual(plaintext));
                }
            }
        }

        [TestMethod]
        public void Scan() {
            Program.HandleScanCommand(new FileInfo(TestPaths.Sample0));
        }

        [TestMethod]
        public void ScanPowerShellScript() {
            string text = Utility.ReadAllText(TestPaths.Sample3);
            using (AmsiSession session = new AmsiSession()) {
                Assert.IsTrue(session.ScanPowerShellScript(text) == AmsiResult.Detected);
            }
        }

        [TestMethod]
        public void ScanPowerShellScriptReflective() {
            string text = Utility.ReadAllText(TestPaths.Sample3);
            using (AmsiSession session = new AmsiSession()) {
                Assert.IsTrue(session.ScanStringReflective(text) == AmsiResult.Detected);
            }
        }

        [TestMethod]
        public void SigfindByCharClass() {
            string text = Utility.ReadAllText(TestPaths.Sample3);
            SignatureFinderByChar sigfind = new SignatureFinderByChar(text);
            sigfind.Start();
            while (sigfind.Status != AmsiScanner.Common.Threading.BackgroundWorkerStatus.Complete) {
                Thread.Sleep(250);
            }
            Assert.IsTrue(!string.IsNullOrEmpty(sigfind.ReducedString));
            Assert.IsFalse(text.Equals(sigfind.ReducedString));
            int max = (int)(0.25f * text.Length);
            Assert.IsTrue(sigfind.ReducedString.Length <= max);
        }

        [TestMethod]
        public void SigfindByTokenClass() {
            string text = Utility.ReadAllText(TestPaths.Sample3);
            SignatureFinderByToken sigfind = new SignatureFinderByToken(text);
            sigfind.Start();
            while(sigfind.Status != AmsiScanner.Common.Threading.BackgroundWorkerStatus.Complete) {
                Thread.Sleep(250);
            }
            Assert.IsTrue(!string.IsNullOrEmpty(sigfind.ReducedString));
            Assert.IsFalse(text.Equals(sigfind.ReducedString));
            int max = (int)(0.25f * text.Length);
            Assert.IsTrue(sigfind.ReducedString.Length <= max);
        }

        [TestMethod]
        public void SigfindByChar() {
            Program.HandleSigFindPerChar(new FileInfo(TestPaths.Sample2));
        }

        [TestMethod]
        public void SigfindByToken() {
            Program.HandleSigFindPerToken(new FileInfo(TestPaths.Sample2));
        }
    }
}
