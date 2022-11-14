using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Cmdlets {
    [Cmdlet(VerbsLifecycle.Invoke, "AmsiScan", DefaultParameterSetName = InvokeAmsiScan.PARAMETER_SET_FILE)]
    public class InvokeAmsiScan : PSCmdlet {
        [Parameter(ParameterSetName = InvokeAmsiScan.PARAMETER_SET_FILE, Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public FileInfo File {
            get;
            set;
        }

        [Parameter(ParameterSetName = InvokeAmsiScan.PARAMETER_SET_TEXT, Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNull]
        public string Text {
            get;
            set;
        }

        [Parameter(ParameterSetName = InvokeAmsiScan.PARAMETER_SET_BYTE, Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNull]
        public byte[] Bytes {
            get;
            set;
        }

        protected override void BeginProcessing() {
            //Only handle scans here if no pipeline input is expected
            if (!this.MyInvocation.ExpectingInput) {
                this.HandleScans();
            }
        }

        protected override void ProcessRecord() {
            //Only handle scans here if pipeline input is expected
            if (this.MyInvocation.ExpectingInput) {
                this.HandleScans();
            }
        }

        private void HandleScans() {
            using (AmsiSession session = new AmsiSession()) {
                if (this.ParameterSetName.Equals(InvokeAmsiScan.PARAMETER_SET_FILE)) {
                    this.WriteObject(session.ScanFile(this.File));
                } else if (this.ParameterSetName.Equals(InvokeAmsiScan.PARAMETER_SET_TEXT)) {
                    string path;
                    if (Utility.TryGetFullPath(this.SessionState.Path.CurrentFileSystemLocation.Path, this.Text, out path)) {
                        this.WriteObject(session.ScanFile(new FileInfo(path)));
                    } else {
                        this.WriteObject(session.ScanString(this.Text));
                    }
                } else if (this.ParameterSetName.Equals(InvokeAmsiScan.PARAMETER_SET_BYTE)) {
                    this.WriteObject(session.ScanData(this.Bytes));
                }
            }
        }

        public const string PARAMETER_SET_FILE = "File";
        public const string PARAMETER_SET_TEXT = "Text";
        public const string PARAMETER_SET_BYTE = "Byte";
    }
}
