using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.IO.Compression;
using System.Windows.Forms.Integration;
using System.Windows.Forms;

namespace WPFtoUnity
{
    public enum LauncherStatus
    {
        ready,
        failed,
        downloadingGame,
        downloadingUpdate
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        WindowsFormsHost windowsFormsHost;

        private string rootPath;
        private string versionFile;
        private string gameExe;

        private LauncherStatus _status;
        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;

                switch (_status)
                {
                    case LauncherStatus.ready:
                        PlayButton.Content = "Play";
                        break;
                    case LauncherStatus.failed:
                        PlayButton.Content = "Update Fail";
                        break;
                    case LauncherStatus.downloadingGame:
                        PlayButton.Content = "Downloading Update";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        PlayButton.Content = "Downloading Game";
                        break;
                    default:
                        break;
                }
            }
        }
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("UnityPlayer.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UnityMain")]
        public static extern int UnityMain(IntPtr hInstance, IntPtr hPrevInstance, [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, int nShowCmd);

        const int GWLP_HINSTANCE = -6;
        const int SW_SHOWDEFAULT = 0x0a;

        //메인 함수
        public MainWindow()
        {
            InitializeComponent();

            rootPath = Directory.GetCurrentDirectory();
            
            versionFile = System.IO.Path.Combine(rootPath, "Version.txt");
            
            gameExe = System.IO.Path.Combine(rootPath, "SprinklerTest.exe");
        }
        /// <summary>
        /// 업데이트 체크
        /// </summary>
        private void CheckForUpdates()
        {
            if (File.Exists(versionFile))
            {
                Version localVersion = new Version(File.ReadAllText(versionFile));
                VersionText.Text = localVersion.ToString();

                //웹 클라이언트 사용
                try
                {
                    WebClient webClient = new WebClient();
                    //버전 파일 링크
                    Version onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1mfUxeObvH_g-rVTA9YTgNXZMx1ooSJjy"));

                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallGameFiles(true, onlineVersion);
                    }
                    else
                    {
                        Status = LauncherStatus.ready;
                    }
                }
                catch (Exception ex)//다운 실패
                {
                    Status = LauncherStatus.failed;
                    System.Windows.MessageBox.Show($"Error checking for game updates: {ex}");
                }
            }
            else
            {
                InstallGameFiles(false, Version.zero);
            }
        }

        /// <summary>
        /// 게임 파일 설치 메서드
        /// </summary>
        /// <param name="isUpdate"></param>
        /// <param name="onlineVersion"></param>
        void InstallGameFiles(bool isUpdate, Version onlineVersion)
        {
            try
            {
                WebClient webClient = new WebClient();
                
                if (isUpdate)
                {
                    Status = LauncherStatus.downloadingUpdate;
                }
                else
                {
                    Status = LauncherStatus.downloadingGame;
                    //버전 파일 링크
                    onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1mfUxeObvH_g-rVTA9YTgNXZMx1ooSJjy"));
                }

                //다운로드 완료될때까지 대기
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                //zip파일 링크
                webClient.DownloadFileAsync(new Uri("https://drive.google.com/uc?export=download&id=12LDHj2WrSm7FMO9BnQBOKT8P7YlY6eDG"), gameExe, onlineVersion);
            }
            catch (Exception ex)//다운 실패
            {
                Status = LauncherStatus.failed;
                System.Windows.MessageBox.Show($"Error installing game files: {ex}");
            }
        }

        /// <summary>
        /// 콜백함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DownloadGameCompletedCallback(object sender,AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersion = ((Version)e.UserState).ToString();
                //ZipFile.ExtractToDirectory(gameZip,rootPath, true);
                //File.Delete(gameZip);

                File.WriteAllText(versionFile, onlineVersion);

                VersionText.Text = onlineVersion;
                Status = LauncherStatus.ready;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                System.Windows.MessageBox.Show($"Error finishing download: {ex}");
            }
           
        }

        //원도우가 로드된 시점
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.UserControl uc = new System.Windows.Forms.UserControl();
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            
            
        }

        void Window_ContentRendered(object ender, EventArgs e)
        {
            CheckForUpdates();
        }

        /// <summary>
        /// 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            //단순 외부 프로그램 실행
            //ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
            //startInfo.UseShellExecute = true;
            //startInfo.WorkingDirectory = rootPath;
            //startInfo.FileName = "SprinklerTest.exe";
            //Process.Start(startInfo);

            EmbedUnity();
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
           
        }
    }

    struct Version
    {
        internal static Version zero = new Version(0, 0, 0);

        private short major;
        private short minor;
        private short subMinor;

        internal Version(short _major, short _minor, short _sub)
        {
            major = _major;
            minor = _minor;
            subMinor = _sub;
        }
        //버전 확인
        internal Version(string _version)
        {
            string[] _versionStrings = _version.Split('.');
            if( _versionStrings.Length !=3)
            {
                major = 0;
                minor = 0;
                subMinor = 0;
                return;
            }

            major = short.Parse(_versionStrings[0]);
            minor = short.Parse(_versionStrings[1]);
            subMinor = short.Parse(_versionStrings[2]);
        }
        //전달된 버전과 다른지 확인
        internal bool IsDifferentThan(Version _otherVersion)
        {
            if (major != _otherVersion.major)
            {
                return true;
            }
            else
            {
                if(minor != _otherVersion.minor)
                {
                    return true;
                }
                else
                {
                    if(subMinor != _otherVersion.subMinor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //버전 구조체에 대한 to문자열 메서드 재정의
        public override string ToString()
        {

            return $"{major}.{minor}.{subMinor}";
        }
    }
}
