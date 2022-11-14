using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Indexing {
    public class LinearPredictor : IndexPredictor {
        public override bool HasNext {
            get {
                return this._next <= this.Max;
            }
        }

        public LinearPredictor(int min, int max) : base(min, max) {
            this._next = min;
        }

        protected override int OnNext() {
            int next = this._next;
            this._next++;
            return next;
        }

        protected override void OnReset(int min, int max) {
            this._next = min;
        }

        private int _next;
    }
}
