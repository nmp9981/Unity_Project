using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;


namespace WPFtoUnity
{
    /// <summary>
    /// MainWindowUnity.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindowUnity : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("UnityPlayer.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UnityMain")]
        public static extern int UnityMain(IntPtr hInstance, IntPtr hPrevInstance, [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, int nShowCmd);

        const int GWLP_HINSTANCE = -6;
        const int SW_SHOWDEFAULT = 0x0a;

        public MainWindowUnity()
        {
            InitializeComponent();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += attemptInit;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        void EmbedUnity()
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            IntPtr hInstFromHwnd = GetWindowLongPtr(hwnd, GWLP_HINSTANCE);
            string cmd = $"-parentHWND {hwnd}";

            UnityMain(hInstFromHwnd, IntPtr.Zero, cmd, SW_SHOWDEFAULT); // hInstFromHwnd는 IntPtr.Zero로 줘도 무방
        }
        private void UnityGrid_VisibleChanged(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ApplicationExit(sender, e);
        }
        // Close Unity application
        private void ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                DeactivateUnityWindow();
                _process.CloseMainWindow();

                Thread.Sleep(1000);
                while (!_process.HasExited)
                    _process.Kill();

                this.Close();
                Window.GetWindow(this).Close();
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }


        #region
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private Process _process;
        private IntPtr _unityHWND = IntPtr.Zero;
        bool initialized = false;

        private const int GWLP_WNDPROC = -4;
        private const int WM_INPUT = 0x00FF;
        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public uint dwFlags;
            public IntPtr hwndTarget;
        }
        const ushort HID_USAGE_PAGE_GENERIC = 0x01;
        const ushort HID_USAGE_GENERIC_KEYBOARD = 0x06;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtrW(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private new delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private MulticastDelegate originalWndProc;
        private WndProc myWndProc;

        void attemptInit(object sender, EventArgs e)
        {

            if (initialized)
                return;
            IntPtr unityHandle = UnityGrid.Handle;

            try
            {
                _process = new Process();
                _process.StartInfo.FileName = "SprinklerTest.exe";
                _process.StartInfo.Arguments = "-parentHWND " + unityHandle.ToInt32() + " " + Environment.CommandLine;
                _process.StartInfo.UseShellExecute = true;
                _process.StartInfo.CreateNoWindow = true;

                _process.Start();
                _process.WaitForInputIdle();

                EnumChildWindows(unityHandle, WindowEnum, IntPtr.Zero);
                SetupRawInput(unityHandle);

                UnityContentResize(this, EventArgs.Empty);
                PresentationSource s = PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow);

                initialized = true;
            }
            catch (Exception ex)
            {
            }
        }

        private void ActivateUnityWindow()
        {
            SendMessage(_unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(_unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            _unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

        private void UnityContentResize(object sender, EventArgs s)
        {
            MoveWindow(_unityHWND, 0, 0, (int)UnityGrid.Width, (int)UnityGrid.Height, true);
            Debug.WriteLine("RESIZED UNITY WINDOW TO: " + (int)UnityGrid.Width + "x" + (int)UnityGrid.Height);
            ActivateUnityWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EmbedUnity();
        }

        private void UnityContentActivate(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void UnityContentDeactivate(object sender, EventArgs e)
        {
            //DeactivateUnityWindow();
        }

        private IntPtr HookWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_INPUT)
            {
                SendMessage(_unityHWND, msg, wParam, lParam);
                return IntPtr.Zero;
            }

            return (IntPtr)originalWndProc.DynamicInvoke(new object[] { hwnd, msg, wParam, lParam });
        }

        private void SetupRawInput(IntPtr hostHWND)
        {
            myWndProc = HookWndProc;

            var originalWndProcPtr = SetWindowLongPtrW(hostHWND, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(myWndProc));
            if (originalWndProcPtr == null)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, "Failed to overwrite the original wndproc");
            }

            Type lel = typeof(MulticastDelegate);
            originalWndProc = (MulticastDelegate)Marshal.GetDelegateForFunctionPointer(originalWndProcPtr, lel);

            var rawInputDevices = new[]
            {
               new RAWINPUTDEVICE()
               {
                   usUsagePage = HID_USAGE_PAGE_GENERIC,
                   usUsage = HID_USAGE_GENERIC_KEYBOARD,
                   dwFlags = 0,
                   hwndTarget = hostHWND
               }
            };

            if (!RegisterRawInputDevices(rawInputDevices, 1, Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
            {
                var lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(lastError, "Failed to register raw input devices");
            }
        }
        #endregion
    }
}
