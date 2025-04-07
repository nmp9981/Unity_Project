using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace WPFtoUnity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("UnityPlayer.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UnityMain")]
        public static extern int UnityMain(IntPtr hInstance, IntPtr hPrevInstance, [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, int nShowCmd);

        const int GWLP_HINSTANCE = -6;
        const int SW_SHOWDEFAULT = 0x0a;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void EmbedUnity()
        {
            IntPtr hwnd = host.Handle;

            IntPtr hInstFromHwnd = GetWindowLongPtr(hwnd, GWLP_HINSTANCE);
            string cmd = $"-parentHWND {hwnd}";
            System.Diagnostics.Trace.WriteLine($"---- {hwnd:x} ----");

            UnityMain(IntPtr.Zero, IntPtr.Zero, cmd, SW_SHOWDEFAULT);
        }

        Thread _unityThread;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_unityThread != null)
            {
                return;
            }

            _unityThread = new Thread(EmbedUnity);
            _unityThread.Start();
        }
    }

    
}
