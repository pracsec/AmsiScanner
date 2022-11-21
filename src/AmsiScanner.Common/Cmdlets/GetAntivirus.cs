using AmsiScanner.Common.Classes;
using System.Management.Automation;

namespace AmsiScanner.Common.Cmdlets {
    [Cmdlet(VerbsCommon.Get, "Antivirus")]
    public class GetAntivirus : PSCmdlet {
        protected override void BeginProcessing() {
            this.WriteObject(Antivirus.GetInstalledAntivirus(), true);
        }
    }
}
