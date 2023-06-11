using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp.RuntimeBinder;

namespace RBXMSEAPI.Classes
{
    // Token: 0x02000005 RID: 5
    public static class fluxteam_net_api
    {
        // Token: 0x06000009 RID: 9
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint access, bool inhert_handle, int pid);

        // Token: 0x0600000A RID: 10
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        // Token: 0x0600000B RID: 11
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, int lpNumberOfBytesWritten);

        // Token: 0x0600000C RID: 12
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        // Token: 0x0600000D RID: 13
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Token: 0x0600000E RID: 14
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        // Token: 0x0600000F RID: 15
        [DllImport("Fluxteam_net_API.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool run_script(IntPtr proc, int pid, string path, [MarshalAs(UnmanagedType.LPWStr)] string script);

        // Token: 0x06000010 RID: 16
        [DllImport("Fluxteam_net_API.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool is_injected(IntPtr proc, int pid, string path);

        // Token: 0x06000011 RID: 17
        [DllImport("Fluxteam_net_API.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool inject_dll(int pid, [MarshalAs(UnmanagedType.LPWStr)] string script);

        // Token: 0x06000012 RID: 18 RVA: 0x00002164 File Offset: 0x00000364
        private static fluxteam_net_api.Result r_inject(string dll_path)
        {
            FileInfo fileInfo = new FileInfo(dll_path);
            FileSecurity accessControl = fileInfo.GetAccessControl();
            SecurityIdentifier identity = new SecurityIdentifier("S-1-15-2-1");
            accessControl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            fileInfo.SetAccessControl(accessControl);
            Process[] processesByName = Process.GetProcessesByName("Windows10Universal");
            if (processesByName.Length == 0)
            {
                return fluxteam_net_api.Result.ProcNotOpen;
            }
            uint num = 0U;
            while ((ulong)num < (ulong)((long)processesByName.Length))
            {
                Process process = processesByName[(int)num];
                if (fluxteam_net_api.pid != process.Id)
                {
                    IntPtr intPtr = fluxteam_net_api.OpenProcess(1082U, false, process.Id);
                    if (intPtr == fluxteam_net_api.NULL)
                    {
                        return fluxteam_net_api.Result.OpenProcFail;
                    }
                    IntPtr intPtr2 = fluxteam_net_api.VirtualAllocEx(intPtr, fluxteam_net_api.NULL, (IntPtr)((dll_path.Length + 1) * Marshal.SizeOf(typeof(char))), 12288U, 64U);
                    if (intPtr2 == fluxteam_net_api.NULL)
                    {
                        return fluxteam_net_api.Result.AllocFail;
                    }
                    byte[] bytes = Encoding.Default.GetBytes(dll_path);
                    int num2 = fluxteam_net_api.WriteProcessMemory(intPtr, intPtr2, bytes, (IntPtr)((dll_path.Length + 1) * Marshal.SizeOf(typeof(char))), 0);
                    if (num2 == 0 || (long)num2 == 6L)
                    {
                        return fluxteam_net_api.Result.Unknown;
                    }
                    if (fluxteam_net_api.CreateRemoteThread(intPtr, fluxteam_net_api.NULL, fluxteam_net_api.NULL, fluxteam_net_api.GetProcAddress(fluxteam_net_api.GetModuleHandle("kernel32.dll"), "LoadLibraryA"), intPtr2, 0U, fluxteam_net_api.NULL) == fluxteam_net_api.NULL)
                    {
                        return fluxteam_net_api.Result.LoadLibFail;
                    }
                    fluxteam_net_api.pid = process.Id;
                    fluxteam_net_api.phandle = intPtr;
                    return fluxteam_net_api.Result.Success;
                }
                else
                {
                    if (fluxteam_net_api.pid == process.Id)
                    {
                        return fluxteam_net_api.Result.AlreadyInjected;
                    }
                    num += 1U;
                }
            }
            return fluxteam_net_api.Result.Unknown;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x000022EC File Offset: 0x000004EC
        public static fluxteam_net_api.Result inject_custom()
        {
            fluxteam_net_api.Result result;
            try
            {
                if (!File.Exists(fluxteam_net_api.dll_path))
                {
                    result = fluxteam_net_api.Result.DLLNotFound;
                }
                else
                {
                    result = fluxteam_net_api.r_inject(fluxteam_net_api.dll_path);
                }
            }
            catch
            {
                result = fluxteam_net_api.Result.Unknown;
            }
            return result;
        }

        // Token: 0x06000014 RID: 20 RVA: 0x0000232C File Offset: 0x0000052C
        public static void inject()
        {
            switch (fluxteam_net_api.inject_custom())
            {
                case fluxteam_net_api.Result.DLLNotFound:
                    MessageBox.Show("Injection Failed! DLL not found!\n", "Injection");
                    return;
                case fluxteam_net_api.Result.OpenProcFail:
                    MessageBox.Show("Injection Failed - OpenProcFail failed!\n", "Injection");
                    return;
                case fluxteam_net_api.Result.AllocFail:
                    MessageBox.Show("Injection Failed - AllocFail failed!\n", "Injection");
                    return;
                case fluxteam_net_api.Result.LoadLibFail:
                    MessageBox.Show("Injection Failed - LoadLibFail failed!\n", "Injection");
                    return;
                case fluxteam_net_api.Result.AlreadyInjected:
                    break;
                case fluxteam_net_api.Result.ProcNotOpen:
                    MessageBox.Show("Failure to find UWP game!\n\nPlease make sure you are using the game from the Microsoft Store and not the browser!", "Injection");
                    return;
                case fluxteam_net_api.Result.Unknown:
                    MessageBox.Show("Injection Failed - Unknown!\n", "Injection");
                    break;
                default:
                    return;
            }
        }

        // Token: 0x06000015 RID: 21 RVA: 0x000023C9 File Offset: 0x000005C9
        public static bool is_injected(int pid)
        {
            fluxteam_net_api.phandle = fluxteam_net_api.OpenProcess(1082U, false, pid);
            return fluxteam_net_api.is_injected(fluxteam_net_api.phandle, pid, fluxteam_net_api.dll_path);
        }

        // Token: 0x06000016 RID: 22 RVA: 0x000023EC File Offset: 0x000005EC
        public static bool run_script(int pid, string script)
        {
            fluxteam_net_api.pid = pid;
            fluxteam_net_api.phandle = fluxteam_net_api.OpenProcess(1082U, false, pid);
            if (pid == 0)
            {
                MessageBox.Show("Please press Inject first!", "Fluxteam.net API Error");
                return false;
            }
            if (script == string.Empty)
            {
                return fluxteam_net_api.is_injected(pid);
            }
            return fluxteam_net_api.run_script(fluxteam_net_api.phandle, pid, fluxteam_net_api.dll_path, script);
        }

        // Token: 0x06000017 RID: 23 RVA: 0x0000244C File Offset: 0x0000064C
        public static void create_files(string dll_path_)
        {
            if (!File.Exists(dll_path_))
            {
                MessageBox.Show("Failure to initalize Fluxteam.net API!\nDLL path was invalid!", "Fatal Error");
                Environment.Exit(0);
            }
            fluxteam_net_api.dll_path = dll_path_;
            string text = "";
            foreach (string text2 in Directory.GetDirectories(Environment.GetEnvironmentVariable("LocalAppData") + "\\Packages"))
            {
                if (text2.Contains("OBLOXCORPORATION"))
                {
                    if (Directory.GetDirectories(text2 + "\\AC").Any((string dir) => dir.Contains("Temp")))
                    {
                        text = text2 + "\\AC";
                    }
                }
            }
            if (text == "")
            {
                return;
            }
            try
            {
                if (Directory.Exists("workspace"))
                {
                    Directory.Move("workspace", "old_workspace");
                }
                if (Directory.Exists("autoexec"))
                {
                    Directory.Move("autoexec", "old_autoexec");
                }
            }
            catch
            {
            }
            string text3 = Path.Combine(text, "workspace");
            string text4 = Path.Combine(text, "autoexec");
            if (!Directory.Exists(text3))
            {
                Directory.CreateDirectory(text3);
            }
            if (!Directory.Exists(text4))
            {
                Directory.CreateDirectory(text4);
            }
        }

        // Token: 0x04000004 RID: 4
        public static string dll_path;

        // Token: 0x04000005 RID: 5
        public static IntPtr phandle;

        // Token: 0x04000006 RID: 6
        public static int pid = 0;

        // Token: 0x04000007 RID: 7
        private static readonly IntPtr NULL = (IntPtr)0;

        // Token: 0x0200000B RID: 11
        public enum Result : uint
        {
            // Token: 0x04000009 RID: 9
            Success,
            // Token: 0x0400000A RID: 10
            DLLNotFound,
            // Token: 0x0400000B RID: 11
            OpenProcFail,
            // Token: 0x0400000C RID: 12
            AllocFail,
            // Token: 0x0400000D RID: 13
            LoadLibFail,
            // Token: 0x0400000E RID: 14
            AlreadyInjected,
            // Token: 0x0400000F RID: 15
            ProcNotOpen,
            // Token: 0x04000010 RID: 16
            Unknown
        }
    }
}
