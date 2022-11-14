using AmsiScanner.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using UnitTests.Utilities;

namespace UnitTests {
    [TestClass]
    public class UtilityUnitTests {
        [TestMethod]
        public void EncryptBytesDefaultPassword() {
            byte[] plaintext = Generate.ByteArray();
            byte[] encrypted = Utility.Encrypt(plaintext);
            Assert.IsNotNull(encrypted);
            Assert.IsTrue(encrypted.Length > 0);
            byte[] decrypted = Utility.Decrypt(encrypted);
            Assert.IsNotNull(decrypted);
            Assert.IsTrue(decrypted.SequenceEqual(plaintext));
        }

        [TestMethod]
        public void EncryptBytesCustomPassword() {
            string password = Guid.NewGuid().ToString();
            byte[] plaintext = Generate.ByteArray();
            byte[] encrypted = Utility.Encrypt(plaintext, password);
            Assert.IsNotNull(encrypted);
            Assert.IsTrue(encrypted.Length > 0);
            byte[] decrypted = Utility.Decrypt(encrypted, password);
            Assert.IsNotNull(decrypted);
            Assert.IsTrue(decrypted.SequenceEqual(plaintext));
        }

        [TestMethod]
        public void EncryptStringDefaultPassword() {
            string password = Guid.NewGuid().ToString();
            string plaintext = "hello world!";
            string encrypted = Utility.EncryptBase64(plaintext, password);
            Assert.IsNotNull(encrypted);
            Assert.IsTrue(encrypted.Length > 0);
            Assert.IsFalse(plaintext.Equals(encrypted));
            string decrypted = Utility.DecryptBase64(encrypted, password);
            Assert.IsNotNull(decrypted);
            Assert.IsTrue(decrypted.SequenceEqual(plaintext));
        }

        [TestMethod]
        public void IsPathTest() {
            Assert.IsFalse(Utility.IsPath("hello world!"));
            Assert.IsTrue(Utility.IsPath(@"C:\Windows\Temp\test.txt"));
            Assert.IsTrue(Utility.IsPath(@"\\.\C$\Windows\Temp\test.txt"));
        }
    }
}
