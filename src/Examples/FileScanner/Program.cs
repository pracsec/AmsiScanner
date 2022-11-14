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