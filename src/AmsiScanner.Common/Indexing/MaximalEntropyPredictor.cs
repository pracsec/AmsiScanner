using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Indexing {
    public class MaximalEntropyPredictor : IndexPredictor {
        public override bool HasNext {
            get {
                return this._count < this._length;
            }
        }

        public MaximalEntropyPredictor(int min, int max) : base(min, max) {
            this.OnReset(min, max);
        }

        protected override int OnNext() {
            this._count++;

            if (this._current.Count <= 0) {
                this._current = this._next;
                this._next = this._current;
            }

            Node current = this._current[0];
            this._current.RemoveAt(0);

            if (current.Minimum == current.Maximum) {
                return current.Minimum;
            } else if (current.Maximum == current.Minimum + 1) {
                if (this._ncount < this._length) {
                    this._next.Add(new Node(current.Maximum, current.Maximum));
                    this._ncount++;
                }
                return current.Minimum;
            }

            int split = (int)((current.Maximum + current.Minimum) / 2.0f);

            if (this._ncount < this._length) {
                this._next.Add(new Node(current.Minimum, split - 1));
                this._next.Add(new Node(split + 1, current.Maximum));
                this._ncount += 2;
            }

            return split;
        }

        protected override void OnReset(int min, int max) {
            this._length = max - min + 1;
            this._count = 0;
            this._ncount = 0;
            this._current = new List<Node>();
            this._current.Add(new Node(min, max));
            this._next = new List<Node>();
        }

        private List<Node> _current;
        private List<Node> _next;
        private int _length;
        private int _count;
        private int _ncount;

        internal class Node {
            public int Minimum;
            public int Maximum;

            public Node() { }

            public Node(int min, int max) {
                this.Minimum = min;
                this.Maximum = max;
            }
        }
    }
}
