using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using UnitTests.Utilities;

namespace UnitTests {
    [TestClass]
    public class TestPathsTest {
        [TestMethod]
        public void AssemblyPathTest() {
            Assert.IsFalse(string.IsNullOrEmpty(TestPaths.AssemblyPath));
            Assert.IsTrue(File.Exists(TestPaths.AssemblyPath));
        }

        [TestMethod]
        public void AssemblyDirectoryTest() {
            Assert.IsFalse(string.IsNullOrEmpty(TestPaths.AssemblyDirectory));
            Assert.IsTrue(Directory.Exists(TestPaths.AssemblyDirectory));
        }

        [TestMethod]
        public void SamplesDirectoryTest() {
            Assert.IsFalse(string.IsNullOrEmpty(TestPaths.SamplesDirectory));
            Assert.IsTrue(Directory.Exists(TestPaths.SamplesDirectory));
        }

        [TestMethod]
        public void Sample0Test() {
            Assert.IsFalse(string.IsNullOrEmpty(TestPaths.Sample0));
            Assert.IsTrue(File.Exists(TestPaths.Sample0));
        }

        [TestMethod]
        public void Sample1Test() {
            Assert.IsFalse(string.IsNullOrEmpty(TestPaths.Sample1));
            Assert.IsTrue(File.Exists(TestPaths.Sample1));
        }

        [TestMethod]
        public void Sample2Test() {
            Assert.IsFalse(string.IsNullOrEmpty(TestPaths.Sample2));
            Assert.IsTrue(File.Exists(TestPaths.Sample2));
        }

        [TestMethod]
        public void Sample3Test() {
            Assert.IsFalse(string.IsNullOrEmpty(TestPaths.Sample3));
            Assert.IsTrue(File.Exists(TestPaths.Sample3));
        }
    }
}
