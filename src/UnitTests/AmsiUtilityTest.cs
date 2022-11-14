using AmsiScanner.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using UnitTests.Utilities;

namespace UnitTests {
    [TestClass]
    public class AmsiUtilityTest {
        [TestMethod]
        public void InitialStateTest() {
            Assert.IsTrue(AmsiUtility.IsEnabled);
        }

        [TestMethod]
        public void DisableAmsiTest() {
            string contents = Utility.ReadAllText(TestPaths.Sample0);
            Assert.IsTrue(AmsiUtility.IsEnabled);

            try {
                AmsiUtility.DisableAmsi();
                Assert.IsFalse(AmsiUtility.IsEnabled);
                try {
                    Token[] tokens = AmsiUtility.Tokenize(contents);
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }

                AmsiUtility.EnableAmsi();
                Assert.IsTrue(AmsiUtility.IsEnabled);
            } finally {
                AmsiUtility.EnableAmsi();
            }
        }

        [TestMethod]
        public void BenignStringTest() {
            using (AmsiSession session = new AmsiSession()) {
                string contents = @"dir C:\ -Filter *.exe -Recurse -EA 0 | % { Write-Host $_.Fullname}";
                AmsiResult result = session.ScanString(contents);
                Assert.IsTrue(result == AmsiResult.NotDetected || result == AmsiResult.Clean, string.Format("Length: {0}\r\nContents:\r\n{1}", contents.Length, contents));
            }
        }

        [TestMethod]
        public void Sample0RepeatedTest() {
            using (AmsiSession session = new AmsiSession()) {
                HashSet<string> set = new HashSet<string>();
                for (int i = 0; i < 10; i++) {
                    string contents = Utility.ReadAllText(TestPaths.Sample0);
                    set.Add(contents);
                    AmsiResult result = session.ScanString(contents);
                    if (result == AmsiResult.Detected) {
                        return;
                    }
                }
            }
            Assert.Fail();
        }

        [TestMethod]
        public void Sample1RepeatedTest() {
            using (AmsiSession session = new AmsiSession()) {
                HashSet<string> set = new HashSet<string>();
                for (int i = 0; i < 10; i++) {
                    string contents = Utility.ReadAllText(TestPaths.Sample1);
                    set.Add(contents);
                    AmsiResult result = session.ScanString(contents);
                    Assert.IsTrue(result == AmsiResult.Detected, string.Format(@"
Iteration: {0}
Length: {1}
Unique: {2}
Contents: {3}", i, contents.Length, set.Count, contents));
                }
            }
        }

        [TestMethod]
        public void EICARTest() {
            using (AmsiSession session = new AmsiSession()) {
                AmsiResult result = session.ScanString(AmsiUtility.EICAR);
                Assert.IsTrue(result == AmsiResult.Detected);
            }
        }

        [TestMethod]
        public void TokenizerTest() {
            string script = @"Write-Host 'hello world!' -ForegroundColor Yellow";
            Token[] tokens = AmsiUtility.Tokenize(script);
            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Length == 7);
            foreach (Token token in tokens) {
                Assert.IsNotNull(token);
                Assert.IsTrue(token.Length > 0);
                Assert.IsFalse(string.IsNullOrEmpty(token.Contents));
            }
        }
    }
}
