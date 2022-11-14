using AmsiScanner.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using UnitTests.Utilities;

namespace UnitTests {
    [TestClass]
    public class CmdletTests {
        [TestMethod]
        public void EncryptString() {
            string plaintext = "hello world!";
            string script = string.Format("Encrypt-String '{0}'", plaintext);
            string ciphertext = this.RunScriptEx<string>(script);
            string decrypted = Utility.DecryptBase64(ciphertext);
            Assert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        public void DecryptString() {
            string plaintext = "hello world!";
            string ciphertext = Utility.EncryptBase64(plaintext);
            string script = string.Format("Decrypt-String '{0}'", ciphertext);
            string decrypted = this.RunScriptEx<string>(script);
            Assert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        public void DecryptStringPipelineInput() {
            string plaintext = "hello world!";
            string ciphertext = Utility.EncryptBase64(plaintext);
            string script = string.Format("'{0}' | Decrypt-String", ciphertext);
            string decrypted = this.RunScriptEx<string>(script);
            Assert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        public void EncryptSingleFile() {
            using (TemporaryFile source = new TemporaryFile()) {
                using (TemporaryFile destination = new TemporaryFile()) {
                    string script = string.Format("Encrypt-File '{0}' '{1}'", source.FileInfo.FullName, destination.FileInfo.FullName);
                    this.RunScript(script);
                    byte[] decrypted = Utility.ReadAllBytes(destination.FileInfo.FullName);
                    Assert.IsTrue(source.CurrentContents.SequenceEqual(decrypted));
                }
            }
        }

        [TestMethod]
        public void EncryptMultipleFile() {
            using (TemporaryDirectory srcdir = new TemporaryDirectory()) {
                using (TemporaryDirectory dstdir = new TemporaryDirectory()) {
                    using (TemporaryFile s1 = new TemporaryFile(srcdir.Directory.FullName)) {
                        using (TemporaryFile s2 = new TemporaryFile(srcdir.Directory.FullName)) {
                            string script = string.Format("dir '{0}' | Encrypt-File -OutputDirectory '{1}'", srcdir.Directory.FullName, dstdir.Directory.FullName);
                            this.RunScript(script);
                            FileInfo[] files = dstdir.Directory.GetFiles();
                            Assert.IsTrue(files.Length == 2);
                            foreach (FileInfo file in files) {
                                byte[] contents = Utility.ReadAllBytes(file.FullName);
                                Assert.IsTrue(contents.SequenceEqual(s1.CurrentContents) || contents.SequenceEqual(s2.CurrentContents));
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void DecryptSingleFile() {
            using (TemporaryFile source = new TemporaryFile()) {
                using (TemporaryFile destination = new TemporaryFile()) {
                    byte[] ciphertext = Utility.Encrypt(source.OriginalContents);
                    File.WriteAllBytes(source.FileInfo.FullName, ciphertext);
                    string script = string.Format("Decrypt-File '{0}' '{1}'", source.FileInfo.FullName, destination.FileInfo.FullName);
                    this.RunScript(script);
                    byte[] decrypted = Utility.ReadAllBytes(destination.FileInfo.FullName);
                    Assert.IsTrue(source.OriginalContents.SequenceEqual(decrypted));
                }
            }
        }

        private PSObject[] RunScript(string script) {
            PowerShell powershell = PowerShell.Create();
            string modified = string.Format("Import-Module '{0}'; {1}", typeof(AmsiUtility).Assembly.Location, script);
            powershell.AddScript(modified);
            PSObject[] results = powershell.Invoke().ToArray<PSObject>();
            if (powershell.HadErrors) {
                Assert.Fail(powershell.Streams.Error[0].Exception.Message);
            }
            return results;
        }

        private T RunScriptEx<T>(string script) {
            PowerShell powershell = PowerShell.Create();
            string modified = string.Format("Import-Module '{0}'; {1}", typeof(AmsiUtility).Assembly.Location, script);
            powershell.AddScript(modified);
            T[] results = powershell.Invoke<T>().ToArray();
            if (powershell.HadErrors) {
                Assert.Fail(powershell.Streams.Error[0].Exception.Message);
            }
            Assert.IsTrue(results.Length == 1);
            return results[0];
        }
    }
}
