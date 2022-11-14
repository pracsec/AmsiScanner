using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Cmdlets {

    public enum EncryptedOutputFormat {
        Base64,
        Bytes
    }

    [Cmdlet("Encrypt", "String")]
    public class EncryptString : PSCmdlet {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNull]
        public string Plaintext {
            get;
            set;
        }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Password {
            get;
            set;
        } = Utility.DEFAULT_PASSWORD;

        [Parameter(Mandatory = false)]
        public EncryptedOutputFormat OutputFormat {
            get;
            set;
        } = EncryptedOutputFormat.Base64;

        protected override void BeginProcessing() {
            this.HandleInput();
        }

        protected override void ProcessRecord() {
            if (this.MyInvocation.ExpectingInput) {
                this.HandleInput();
            }
        }
        
        private void HandleInput() {
            if (this.Plaintext == null) {
                return;
            }

            if (this.OutputFormat == EncryptedOutputFormat.Base64) {
                this.WriteObject(Utility.EncryptBase64(this.Plaintext, this.Password));
            } else {
                this.WriteObject(Utility.Encrypt(Encoding.UTF8.GetBytes(this.Plaintext), this.Password));
            }
        }
    }
}
