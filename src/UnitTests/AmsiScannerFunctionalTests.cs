using AmsiScanner.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnitTests.Utilities;

namespace UnitTests {
    [TestClass]
    public class AmsiScannerFunctionalTests {
        [TestMethod]
        public void Encrypt() {
            using (TemporaryFile src = new TemporaryFile()) {
                using (TemporaryFile dst = new TemporaryFile()) {
                    string arguments = string.Format("encrypt \"{0}\" \"{1}\"", src.FileInfo.FullName, dst.FileInfo.FullName);
                    ProcessOutput output = ProcessOutput.Run("amsiscanner", arguments);
                    Assert.IsTrue(string.IsNullOrEmpty(output.StandardOutput), output.StandardOutput);
                    Assert.IsTrue(string.IsNullOrEmpty(output.StandardError), output.StandardError);
                    Assert.IsTrue(output.ExitCode == 0);
                    Assert.IsTrue(dst.FileInfo.Exists);
                    Assert.IsTrue(dst.FileInfo.Length >= src.FileInfo.Length);
                    Assert.IsFalse(src.CompareTo(dst) == 0);
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

                    string arguments = string.Format("decrypt \"{0}\" \"{1}\"", src.FileInfo.FullName, dst.FileInfo.FullName);
                    ProcessOutput output = ProcessOutput.Run("amsiscanner", arguments);
                    Assert.IsTrue(string.IsNullOrEmpty(output.StandardOutput), output.StandardOutput);
                    Assert.IsTrue(string.IsNullOrEmpty(output.StandardError), output.StandardError);
                    Assert.IsTrue(output.ExitCode == 0);
                    Assert.IsTrue(dst.FileInfo.Exists);
                    Assert.IsTrue(dst.FileInfo.Length <= src.FileInfo.Length);
                    Assert.IsTrue(dst.CurrentContents.SequenceEqual(plaintext));
                }
            }
        }

        [TestMethod()]
        public  void Scan() {
            string arguments = string.Format("scan \"{0}\"", TestPaths.Sample1);
            ProcessOutput output = ProcessOutput.Run("amsiscanner", arguments);
            Assert.IsTrue(string.IsNullOrEmpty(output.StandardError), output.StandardError);
            Assert.IsTrue(output.ExitCode == 0);
            Assert.IsTrue(output.StandardOutput.Contains(AmsiResult.Detected.ToString()));
        }

        [TestMethod()]
        public void SigfindByChar() {
            string arguments = string.Format("sigfind char \"{0}\"", TestPaths.Sample2);
            ProcessOutput output = ProcessOutput.Run("amsiscanner", arguments);
            Assert.IsTrue(string.IsNullOrEmpty(output.StandardError), output.StandardError);
            Assert.IsTrue(output.ExitCode == 0);
            Assert.IsTrue(output.StandardOutput.Contains("[*] Iteration:"));
            Assert.IsTrue(output.StandardOutput.Contains("[*] Signature finding complete."));
            Assert.IsTrue(output.StandardOutput.Contains("[*] Reduced String:"));
        }
    }
}
