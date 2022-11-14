using System;
using System.Collections.Generic;
using System.Text;

namespace AmsiScanner.Common {
    public class ScanResult {
        public AmsiResult Result {
            get;
            private set;
        }

        public int Value {
            get;
            private set;
        }

        public ScanResult(AmsiResult result, int value) {
            this.Result = result;
            this.Value = value;
        }

        public override string ToString() {
            AmsiResult result = this.Result;
            return string.Format("{0}({1})", result.ToString(), this.Value);
        }
    }
}
