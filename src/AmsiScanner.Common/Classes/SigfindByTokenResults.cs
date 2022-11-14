using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Classes {
    public class SigfindByTokenResults {
        public string ReducedString {
            get;
        }

        public Token[] ReducedTokens {
            get;
        }

        public SigfindByTokenResults(string reducedString, Token[] reducedTokens) {
            this.ReducedString = reducedString;
            this.ReducedTokens = reducedTokens;
        }
    }
}
