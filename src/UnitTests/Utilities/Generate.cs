using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Utilities {
    public static class Generate {
        public static byte[] ByteArray(int length = 256) {
            if (length < 0) {
                throw new ArgumentOutOfRangeException("Length", "Length must be greater than or equal to zero.");
            }

            byte[] bytes = new byte[length];
            Random random = new Random();
            random.NextBytes(bytes);
            return bytes;
        }
    }
}
