using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Utilities {
    public class TemporaryDirectory : IDisposable {
        public DirectoryInfo Directory {
            get;
            private set;
        }

        public TemporaryDirectory() {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            this.Directory = new DirectoryInfo(path);
            this.Directory.Create();
        }

        public void Dispose() {
            this.Directory.Delete(true);
        }
    }
}
