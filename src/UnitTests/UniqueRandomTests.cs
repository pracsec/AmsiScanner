using AmsiScanner.Common.ExtendedMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests {
    [TestClass]
    public class UniqueRandomTests {
        [TestMethod]
        public void Basic128() {
            this.RunTest(0, 127);
        }

        private void RunTest(int min, int max) {
            UniqueRandom random = new UniqueRandom(min, max);
            int length = max - min + 1;
            HashSet<long> set = new HashSet<long>();
            bool ordered = true;
            for (int i = min; i < length; i++) {
                long next = random.Next();
                Assert.IsTrue(set.Add(next));
                ordered &= next == i;
            }
            Assert.IsFalse(ordered);

            for (int i = min; i < length; i++) {
                Assert.IsTrue(set.Contains(i));
            }
        }
    }
}
