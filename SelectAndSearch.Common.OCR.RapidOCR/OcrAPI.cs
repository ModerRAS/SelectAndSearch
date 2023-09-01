using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Process = System.Diagnostics.Process;
namespace SelectAndSearch.Common.OCR.RapidOCR {
    public class OcrAPI {
        private Process ret;
        private Process psutilProcess;
        private string initErrorMsg;

        const int InitTimeout = 15;  // 初始化超时时间，秒

        public OcrAPI(string exePath, string argsStr = "") {
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

            ret = new Process {
                StartInfo = startupInfo
            };
            ret.Start();

            AppDomain.CurrentDomain.ProcessExit += Stop;  // Register process termination handler
            psutilProcess = Process.GetProcessById(ret.Id);

            initErrorMsg = $"OCR init fail.\nEngine path: {exePath}\nStartup args: {args}";

            Timer checkTimer = null;
            TimerCallback cancelTimeout = _ => checkTimer?.Dispose();
            TimerCallback checkTimeout = _ => {
                initErrorMsg = $"OCR init timeout: {InitTimeout}s.\n{exePath}";
                ret.Kill();
            };
            checkTimer = new Timer(checkTimeout, null, InitTimeout * 1000, Timeout.Infinite);

            string initStr;
            while (true) {
                if (ret.HasExited) {
                    cancelTimeout(null);
                    throw new Exception(initErrorMsg);
                }

                initStr = ret.StandardOutput.ReadLine();
                if (initStr.Contains("OCR init completed.")) {
                    break;
                }
            }
            cancelTimeout(null);
        }

        public JObject Run(string imgPath) {
            if (ret.HasExited) {
                return new JObject(new JProperty("code", 400), new JProperty("data", "Subprocess has terminated."));
            }

            JObject writeDict = new JObject(new JProperty("imagePath", imgPath));
            string wirteStr = writeDict.ToString(Newtonsoft.Json.Formatting.None);
            byte[] wirteBytes = Encoding.ASCII.GetBytes(wirteStr + "\n");
            try {
                ret.StandardInput.BaseStream.Write(wirteBytes, 0, wirteBytes.Length);
                ret.StandardInput.Flush();
            } catch {
                return new JObject(new JProperty("code", 400), new JProperty("data", "Failed to write image path to subprocess."));
            }

            if (imgPath.EndsWith("\n")) {
                imgPath = imgPath.Substring(0, imgPath.Length - 1);
            }

            try {
                string getStr = ret.StandardOutput.ReadLine();
                JObject result = JObject.Parse(getStr);
                return result;
            } catch {
                return new JObject(new JProperty("code", 401), new JProperty("data", $"Failed to read output from OCR subprocess for image: {imgPath}"));
            }
        }

        public void Stop(object sender, EventArgs e) {
            Stop();
            AppDomain.CurrentDomain.ProcessExit -= Stop;
        }

        public void Stop() {
            ret?.Kill();
        }
    }
}
