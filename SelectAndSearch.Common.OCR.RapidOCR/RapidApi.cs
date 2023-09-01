using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.OCR.RapidOCR {
    public class RapidApi {
        public string exePath { get; set; }
        public string argsStr { get; set; }
        public RapidApi() {

        }
        public void Execute() {
            string cwd = Path.GetDirectoryName(exePath);  // Get parent folder of exe
            string args = " ";
            if (!string.IsNullOrEmpty(argsStr)) {
                args += " " + argsStr;
            }
            if (!args.Contains("ensureAscii")) {
                args += " --ensureAscii=1";
            }
            ProcessStartInfo startupInfo = null;
            if (Environment.OSVersion.Platform.ToString().Contains("Win32")) {
                startupInfo = new ProcessStartInfo {
                    FileName = exePath,
                    WorkingDirectory = cwd,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                };
            }
        }
    }
}
