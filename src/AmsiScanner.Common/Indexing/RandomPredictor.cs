using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Indexing {
    public class RandomPredictor : IndexPredictor {
        public override bool HasNext {
            get {
                return this._next <= this.Max;
            }
        }

        public RandomPredictor(int min, int max) : base(min, max) {
            this.OnReset(min, max);
        }

        protected override int OnNext() {
            int next = this._next;
            this._next++;
            return this._indices[next];
        }

        protected override void OnReset(int min, int max) {
            int length = max - min + 1;
            this._indices = new int[length];
            for (int i = 0; i < length; i++) {
                this._indices[i] = min + i;
            }
            Random random = new Random();
            random.Shuffle(this._indices);
            this._next = 0;
        }

        private int[] _indices;
        private int _next;
    }
}
