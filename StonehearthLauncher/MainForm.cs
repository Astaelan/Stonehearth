using StonehearthCommon;
using StonehearthLauncher.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace StonehearthLauncher
{
    public partial class MainForm : Form
    {
        private LobbyClient mLobbyClient = new LobbyClient();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            EmailText.Text = Settings.Default.Email;
        }

        private void EmailText_TextChanged(object sender, EventArgs e)
        {
            LoginButton.Enabled = EmailText.TextLength > 0 && PasswordText.TextLength > 0;
        }

        private void PasswordText_TextChanged(object sender, EventArgs e)
        {
            LoginButton.Enabled = EmailText.TextLength > 0 && PasswordText.TextLength > 0;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(Settings.Default.ServerHost, Settings.Default.ServerPort, EndConnect, socket);
        }

        private void ConfigureButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Configuration is not implemented yet", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void SnifferButton_Click(object sender, EventArgs e)
        {
            string path = Path.GetFullPath("StonehearthLoader.dll");
            Process[] processes = Process.GetProcessesByName("Agent");
            if (processes == null || processes.Length == 0) return;
            Process process = Array.Find(processes, p => p.MainModule.FileVersionInfo.FileDescription == "Battle.net Update Agent");
            if (process == null) return;
            if (!process.Modules.Cast<ProcessModule>().Any(m => m.FileName == path)) Inject(process.Handle, path);
            Close();
        }

        private void EndConnect(IAsyncResult pResult)
        {
            Socket socket = (Socket)pResult.AsyncState;
            try { socket.EndConnect(pResult); }
            catch (Exception) { }
            if (socket != null)
            {
                mLobbyClient.Email = EmailText.Text;
                mLobbyClient.Password = PasswordText.Text;
                mLobbyClient.Session = Guid.Empty;
                mLobbyClient.Connect(socket);
                while (mLobbyClient.State != Client.SocketState.Offline) if (mLobbyClient.Pulse()) Thread.Sleep(1);
                if (mLobbyClient.Session != Guid.Empty)
                {
                    Win32API.STARTUPINFO startupInfo = new Win32API.STARTUPINFO();
                    Win32API.PROCESS_INFORMATION processInfo = new Win32API.PROCESS_INFORMATION();
                    Win32API.CreateProcess(
                        Settings.Default.ExecutablePath,
                        "-launch -host=" + Settings.Default.ServerHost + " -port=" + Settings.Default.ServerPort + " -session=" + mLobbyClient.Session,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        false,
                        Win32API.ProcessCreationFlags.CREATE_SUSPENDED,
                        IntPtr.Zero,
                        Path.GetDirectoryName(Settings.Default.ExecutablePath),
                        ref startupInfo,
                        out processInfo);

                    Inject(processInfo.hProcess, Path.GetFullPath("StonehearthLoader.dll"));
                    Win32API.ResumeThread(processInfo.hThread);
                }
                mLobbyClient.ProtocolState = LobbyClientProtocolState.Handshake;
                mLobbyClient.Email = null;
                mLobbyClient.Password = null;
                mLobbyClient.Session = Guid.Empty;
            }
        }

        private static void Inject(IntPtr pHandle, string pPath)
        {
            byte[] path = Encoding.ASCII.GetBytes(pPath);
            Array.Resize(ref path, path.Length + 1);
            IntPtr parameterAddress = Win32API.VirtualAllocEx(pHandle, IntPtr.Zero, (uint)path.Length, 0x1000, 0x40);
            uint outParam = 0;
            Win32API.WriteProcessMemory(pHandle, parameterAddress, path, (uint)path.Length, out outParam);
            UIntPtr loadLib = Win32API.GetProcAddress(Win32API.GetModuleHandleA("kernel32.dll"), "LoadLibraryA");
            IntPtr injectorThread = Win32API.CreateRemoteThread(pHandle, IntPtr.Zero, 0, loadLib, parameterAddress, 0, out outParam);
            Win32API.WaitForSingleObject(injectorThread, -1);
            Win32API.VirtualFreeEx(pHandle, parameterAddress, 0, 0x8000);
            Win32API.CloseHandle(injectorThread);
        }
    }
}
