using System;
using System.Collections.Generic;
using System.Text;

namespace AmsiScanner.Common {
    public enum AmsiResult {
        Clean,
        NotDetected,
        BlockedByAdmin,
        Detected
    }
}
