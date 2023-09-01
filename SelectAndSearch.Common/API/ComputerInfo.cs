using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.API {
    /// <summary>
    /// 操作系统信息
    /// </summary>
    public static class ComputerInfo {
        public static bool IsWin7 { 
            get {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\WindowsNT\CurrentVersion", "CurrentVersion", out var CurrentVersion)) {
                    if (((string)CurrentVersion).Equals("6.1")) {
                        return true;
                    } 
                }
                return false;
            } 
        }
        /// <summary>
        /// 操作系统名称
        /// </summary>
        public static string WinProductName {
            get {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\WindowsNT\CurrentVersion", "ProductName", out var productName)) {
                    return (string)productName;
                }
                return "未知";
            }
        }

        /// <summary>
        /// 操作系统主要版本号
        /// </summary>
        public static uint WinMajorVersion {
            get {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", out var major)) {
                    return (uint)major;
                }

                if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out var version))
                    return 0;

                var versionParts = ((string)version).Split('.');
                if (versionParts.Length != 2) return 0;
                return uint.TryParse(versionParts[0], out var majorAsUInt) ? majorAsUInt : 0;
            }
        }

        /// <summary>
        /// 操作系统次要版本号
        /// </summary>
        public static uint WinMinorVersion {
            get {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber", out var minor)) {
                    return (uint)minor;
                }

                if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out var version))
                    return 0;

                var versionParts = ((string)version).Split('.');
                if (versionParts.Length != 2) return 0;
                return uint.TryParse(versionParts[1], out var minorAsUInt) ? minorAsUInt : 0;
            }
        }

        /// <summary>
        /// 判断是否为Server系统
        /// </summary>
        public static uint IsServer {
            get {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "InstallationType",
                        out var installationType)) {
                    return (uint)(installationType.Equals("Client") ? 0 : 1);
                }

                return 0;
            }
        }

        /// <summary>
        /// 获取注册表值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        private static bool TryGetRegistryKey(string path, string key, out dynamic value) {
            value = null;
            try {
                using (var rk = Registry.LocalMachine.OpenSubKey(path)) {
                    if (rk == null) return false;
                    value = rk.GetValue(key);
                    return value != null;
                }
            } catch {
                return false;
            }
        }
    }
}
