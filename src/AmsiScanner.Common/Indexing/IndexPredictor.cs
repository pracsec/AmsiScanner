using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Indexing {
    public abstract class IndexPredictor {
        public int Min {
            get;
            private set;
        }

        public int Max {
            get;
            private set;
        }

        public int Length {
            get;
            private set;
        }

        public IndexPredictor(int min, int max) {
            this.Min = min;
            this.Max = max;
            this.Length = max - min + 1;
        }

        public abstract bool HasNext {
            get;
        }

        public void Reset(int min, int max) {
            this.Min = min;
            this.Max = max;
            this.Length = max - min + 1;

            this.OnReset(min, max);
        }

        public int Next() {
            if (!this.HasNext) {
                throw new InvalidOperationException();
            }

            return this.OnNext();
        }

        protected abstract int OnNext();

        protected abstract void OnReset(int min, int max);
    }
}
