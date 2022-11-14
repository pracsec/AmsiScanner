using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Cmdlets {

    [Cmdlet("Decrypt", "File")]
    public class DecryptFile : PSCmdlet {
        [Parameter(ParameterSetName = EncryptFile.PARAMETER_SET_NAME_SINGLEFILE, Mandatory = true, Position = 0, ValueFromPipeline = false)]
        [ValidateNotNullOrEmpty]
        public string InputPath {
            get;
            set;
        }

        [Parameter(ParameterSetName = EncryptFile.PARAMETER_SET_NAME_SINGLEFILE, Mandatory = true, Position = 1, ValueFromPipeline = false)]
        [ValidateNotNullOrEmpty]
        public string OutputPath {
            get;
            set;
        }

        [Parameter(ParameterSetName = EncryptFile.PARAMETER_SET_NAME_MULTIPLEFILES, Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull]
        public FileInfo InputFile {
            get;
            set;
        }

        [Parameter(ParameterSetName = EncryptFile.PARAMETER_SET_NAME_MULTIPLEFILES, Mandatory = true, ValueFromPipeline = false)]
        [ValidateNotNullOrEmpty]
        public DirectoryInfo OutputDirectory {
            get;
            set;
        }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Password {
            get;
            set;
        } = Utility.DEFAULT_PASSWORD;

        protected override void BeginProcessing() {
            if (this.ParameterSetName.Equals(DecryptFile.PARAMETER_SET_NAME_SINGLEFILE)) {
                byte[] plaintext = Utility.ReadAllBytes(this.InputPath, this.Password);
                File.WriteAllBytes(this.OutputPath, plaintext);
            }
        }

        protected override void ProcessRecord() {
            if (this.ParameterSetName.Equals(DecryptFile.PARAMETER_SET_NAME_MULTIPLEFILES)) {
                string outfile = Path.Combine(this.OutputDirectory.FullName, Path.GetFileNameWithoutExtension(this.InputFile.FullName) + ".bin");
                byte[] plaintext = Utility.ReadAllBytes(this.InputFile.FullName, this.Password);
                File.WriteAllBytes(outfile, plaintext);
            }
        }

        public const string PARAMETER_SET_NAME_SINGLEFILE = "SingleFile";
        public const string PARAMETER_SET_NAME_MULTIPLEFILES = "MultipleFiles";
    }
}
