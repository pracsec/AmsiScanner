using AmsiScanner.Common.ExtendedMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Indexing {
    public class PreviousPredictor : IndexPredictor {
        public override bool HasNext {
            get {
                return this._count < this._length;
            }
        }

        public PreviousPredictor(int min, int max) : base(min, max) {
            this.OnReset(min, max);
        }

        protected override int OnNext() {
            int next = 0;
            if (this._count % 2 == 0) {
                next = this._upper;
                this._upper++;
            } else {
                next = this._lower;
                this._lower--;
            }
            this._count++;
            this._previous = next;
            return Functions.Mod(next, this._length) + this.Min;
        }

        protected override void OnReset(int min, int max) {
            this._length = max - min + 1;
            this._upper = this._previous % this._length;
            if (this._upper == 0) {
                this._lower = this._length - 1;
            } else {
                this._lower = this._upper - 1;
            }
            this._count = 0;
        }

        private int _upper;
        private int _lower;
        private int _previous;
        private int _length;
        private int _count;
    }
}
