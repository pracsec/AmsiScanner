using System;
using System.IO;
using System.Linq;

namespace UnitTests.Utilities {
    public class TemporaryFile : IDisposable,IComparable<TemporaryFile> {
        public FileInfo FileInfo {
            get;
            private set;
        }

        public byte[] OriginalContents {
            get;
            private set;
        }

        public byte[] CurrentContents {
            get {
                return File.ReadAllBytes(this.FileInfo.FullName);
            }
        }

        public TemporaryFile(int filesize = 4096) {
            this.Initialize(Path.GetTempPath(), filesize);
        }

        public TemporaryFile(string directory, int filesize = 4096) {
            this.Initialize(directory, filesize);
        }

        private void Initialize(string directory, int filesize) {
            if (filesize < 0) {
                throw new ArgumentOutOfRangeException("FileSize", "FileSize must be greater than or equal to zero.");
            }

            try {
                string path = Path.Combine(directory, Guid.NewGuid().ToString().Replace("-", string.Empty) + ".tmp");
                this.FileInfo = new FileInfo(path);

                if (filesize > 0) {
                    Random random = new Random();
                    this.OriginalContents = new byte[filesize];
                    random.NextBytes(this.OriginalContents);
                    File.WriteAllBytes(this.FileInfo.FullName, this.OriginalContents);
                } else {
                    this.OriginalContents = new byte[0];
                }
            } catch (Exception ex) {
                try {
                    if (this.FileInfo.Exists) {
                        this.FileInfo.Delete();
                    }
                } catch { }

                throw ex;
            }
        }

        public void Dispose() {
            File.Delete(this.FileInfo.FullName);
        }

        public int CompareTo(TemporaryFile other) {
            if (this.FileInfo.Length != other.FileInfo.Length) {
                return this.FileInfo.Length.CompareTo(other.FileInfo.Length);
            }

            byte[] src = File.ReadAllBytes(this.FileInfo.FullName);
            byte[] dst = File.ReadAllBytes(other.FileInfo.FullName);
            for (int i = 0; i < src.Length; i++) {
                if (src[i] != dst[i]) {
                    return src[i].CompareTo(dst[i]);
                }
            }

            return 0;
        }
    }
}
