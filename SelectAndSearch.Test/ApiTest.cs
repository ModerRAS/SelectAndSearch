using Newtonsoft.Json;
using SelectAndSearch.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Test {
    [TestClass]
    public class ApiTest {
        [TestMethod]
        public void GetVersionTest() {
            var version = VersionHelper.GetVersion();
            Console.WriteLine(version.Major);
            Console.WriteLine(JsonConvert.SerializeObject(version));
            
        }
        static string GetWindowsVersion() {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption, Version FROM Win32_OperatingSystem")) {
                foreach (ManagementObject os in searcher.Get()) {
                    string caption = os["Caption"].ToString();
                    string version = os["Version"].ToString();

                    if (caption.Contains("Windows 7")) {
                        return "Windows 7";
                    } else if (caption.Contains("Windows 10")) {
                        return "Windows 10";
                    } else if (caption.Contains("Windows 11")) {
                        return "Windows 11";
                    }
                }
            }

            return "Unknown";
        }
        [TestMethod]
        public void GetComputerInfoTest() {
            Console.WriteLine(ComputerInfo.WinMajorVersion);
            Console.WriteLine(ComputerInfo.WinMinorVersion);
            Console.WriteLine(GetWindowsVersion());
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1);
            Console.WriteLine(Environment.OSVersion.Version.Minor);
        }
    }
}
