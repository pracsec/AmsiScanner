using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.ExtendedMath {
    public class UniqueRandom {
        public bool HasNext {
            get {
                return this._current <= this._length;
            }
        }
        public UniqueRandom(long min, long max) {
            this._min = min;
            this._length = max - min + 1;
            this._current = 1;
            this.Initialize(this._length);
        }

        public long Next() {
            if (!this.HasNext) {
                throw new InvalidOperationException();
            }

            long current = this._current;
            this._current++;
            return (current * this._x) % this._m + this._min - 1;
        }

        private void Initialize(long keys) {
            this._m = keys + 1;         // Number of keys + 1
            this._x = 387420489;   // must be coprime to m

            long maxx = (int)(long.MaxValue / ((float)this._m * 2));
            Random random = new Random();
            long factor = random.Next((int)this._m * 2, int.MaxValue);
            this._x = factor;
        }

        private long _current;
        private long _x;
        private long _m;
        private long _min;
        private long _length;
    }
}
