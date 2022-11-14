# AmsiScanner.Common

Scan files with the Anti-Malware Scan Interface (AMSI) and minimize malicious text files and PowerShell scripts to see what the Antivirus is flagging as malicious.

**Features:**
- Scan binary and text files with the installed AV to see if they are malicious.
- Find the smallest subset of text that is still flagged as malicious by the installed AV.
- Find the smallest subset of PowerShell tokens that are still flagged as malicious by the installed AV.
- Encrypt files to mitigate accidental exection and detection by the installed AV.
- Read files encrypted with this utility.

# Dependencies

- Windows 10+
- .NET 6.0

# AmsiScanner.Common Usage

## Scan a File

Any type of file can be scanned with AmsiScanner; however, the type of text encoding may cause issues with some Antivirus engines. By default, Unicode encoding is used for text like files.

### Example

```csharp
using AmsiScanner.Common;

namespace FileScanner {

    internal class Program {
        /// <summary>
        /// Example command: .\FileScanner.exe ".\test-file.bin"
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) {
            //This method will read from files encrypted with this library, single files in a zip file, or plain files
            byte[] contents = Utility.ReadAllBytes(args[0]);

            //Initialize an AmsiSession to the installed AV
            AmsiResult result = default;
            using (AmsiSession session = new AmsiSession()) {
                //If file contains text, scan it using Unicode
                //I've noted that malicious PowerShell scripts won't match Windows Defender signatures
                //we don't use this ScanString method. If we just scan the raw bytes, then if the file was
                //encoded with anything other than Unicode, then signatures will not match.
                string text;
                if (Utility.TryGetText(contents, out text)) {
                    //Scan text in Unicode format
                    result = session.ScanString(text);
                } else {
                    //Scan the raw bytes
                    result = session.ScanData(contents);
                }
            }

            //Print the results
            Console.WriteLine(result);
        }
    }
}
```