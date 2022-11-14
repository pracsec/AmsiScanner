using AmsiScanner.Common.Indexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests {
    [TestClass]
    public class PredictorTests {
        [TestMethod]
        public void LinearPredictorBasicTest127() {
            PredictorTests.RunBasicTest(new LinearPredictor(0, 127));
        }

        [TestMethod]
        public void LinearPredictorBasicTest2() {
            PredictorTests.RunBasicTest(new LinearPredictor(0, 1));
        }

        [TestMethod]
        public void RandomPredictorBasicTest127() {
            PredictorTests.RunBasicTest(new RandomPredictor(0, 127));
        }

        [TestMethod]
        public void RandomPredictorBasicTest2() {
            PredictorTests.RunBasicTest(new RandomPredictor(0, 1));
        }

        [TestMethod]
        public void PreviousPredictorBasicTest127() {
            PredictorTests.RunBasicTest(new PreviousPredictor(0, 127));
        }

        [TestMethod]
        public void PreviousPredictorBasicTest2() {
            PredictorTests.RunBasicTest(new PreviousPredictor(0, 1));
        }

        [TestMethod]
        public void PreviousPredictorBasicTest4() {
            PredictorTests.RunBasicTest(new PreviousPredictor(0, 3));
        }

        [TestMethod]
        public void MaximalEntropyPredictorBasicTest2() {
            PredictorTests.RunBasicTest(new MaximalEntropyPredictor(0, 2));
        }

        [TestMethod]
        public void MaximalEntropyPredictorBasicTest11() {
            PredictorTests.RunBasicTest(new MaximalEntropyPredictor(0, 11));
        }

        [TestMethod]
        public void MaximalEntropyPredictorBasicTest12() {
            PredictorTests.RunBasicTest(new MaximalEntropyPredictor(0, 12));
        }

        [TestMethod]
        public void MaximalEntropyPredictorBasicTest13() {
            PredictorTests.RunBasicTest(new MaximalEntropyPredictor(0, 13));
        }

        [TestMethod]
        public void MaximalEntropyPredictorBasicTest14() {
            PredictorTests.RunBasicTest(new MaximalEntropyPredictor(0, 14));
        }

        [TestMethod]
        public void MaximalEntropyPredictorBasicTest15() {
            PredictorTests.RunBasicTest(new MaximalEntropyPredictor(0, 15));
        }

        private static void RunBasicTest(IndexPredictor predictor) {
            HashSet<int> indices = new HashSet<int>();
            for (int i = predictor.Min; i <= predictor.Max; i++) {
                int next = predictor.Next();
                Assert.IsTrue(indices.Add(next), string.Format("Duplicate index found: {0}", next));
            }
            Assert.IsFalse(predictor.HasNext, "The predictor should not have next.");

            for (int i = predictor.Min; i <= predictor.Max; i++) {
                Assert.IsTrue(indices.Contains(i), string.Format("Predictor missed value {0}.", i));
            }
        }
    }
}
