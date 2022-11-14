using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Cmdlets {

    [Cmdlet("Decrypt", "String")]
    public class DecryptString : PSCmdlet {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNull]
        public string Ciphertext {
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
            this.HandleInput();
        }

        protected override void ProcessRecord() {
            if (this.MyInvocation.ExpectingInput) {
                this.HandleInput();
            }
        }

        private void HandleInput() {
            if (this.Ciphertext == null) {
                return;
            }
            
            this.WriteObject(Utility.DecryptBase64(this.Ciphertext, this.Password));
        }
    }
}
