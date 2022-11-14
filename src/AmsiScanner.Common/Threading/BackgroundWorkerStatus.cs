using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Threading {
    public enum BackgroundWorkerStatus {
        NotStarted,
        Starting,
        Running,
        Stopping,
        Complete
    }
}
