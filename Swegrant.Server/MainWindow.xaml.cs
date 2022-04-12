using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Swegrant.Server.Hubs;
using System.Threading;
using System.Diagnostics;
using System.Windows.Controls;
using Swegrant.Shared.Models;

namespace Swegrant.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static MainWindow Singleton
        {
            get;
            private set;
        }

        public Mode CurrentMode;
        private volatile bool continueAutoLine;

        private int theaterSceneSelectedIndex = 0;

        public static IHubContext<ChatHub> HUB { get; private set; }

        private SecondaryWindow _SecondaryWindow;

        private List<Subtitle> currentSub;
        private int currentSubIndex = 0;
        private int currentScene = 0;
        private Task currentSubTask;
        private CancellationTokenSource currentSubCancelationSource;
        private CancellationToken currentSubCancellationToken;

        private Task autoLine;



        //private HttpSelfHostServer restService;
        //private IDisposable apiServer;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            this.tabControl.IsEnabled = false;
            this.btnStartServer.IsEnabled = false;
            this.btnvdOpenSecondary.IsEnabled = false;
            this.btnvdCloseSecondary.IsEnabled = false;

            if (settings["SecondaryStyle"] != null)
            {
                string windowStyle = settings["SecondaryStyle"].Value.ToString();
                WindowStyle style = (WindowStyle)Enum.Parse(typeof(WindowStyle), windowStyle);
                if (style == WindowStyle.None)
                {
                    rdSecondaryWindowFull.IsChecked = true;
                }
                else if (style == WindowStyle.SingleBorderWindow)
                {
                    rdSecondaryWindowNoraml.IsChecked = true;
                }
            }

            string[] ips = Helpers.NetworkHelpers.GetAllIPs();
            cmbServerIP.ItemsSource = ips;
            object savedIP = Helpers.AppConfigHelpers.LoadConfig("ServerIP");
            if (settings["ServerIP"] != null)
            {   
                if (ips.Contains(savedIP.ToString()))
                {
                    cmbServerIP.SelectedItem = savedIP;
                }
            }           
            MainWindow.Singleton = this;
            
        }

        private async void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string localIP = Helpers.NetworkHelpers.GetLocalIPv4();
                string localIP = cmbServerIP.SelectedItem.ToString();
                string port = Swegrant.Shared.Models.ChatSettings.DefaultPort;
                if (!string.IsNullOrEmpty(localIP))
                {
                    
                    Task task = new Task(() => StartServer(localIP, port));
                    task.Start();

                    Helpers.AppConfigHelpers.SaveConfig("ServerIP", localIP);
                   
                    MessageBox.Show("Server Started");
                    this.btnvdOpenSecondary.IsEnabled = true;
                    this.btnvdCloseSecondary.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("IP NOT FOUND!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void StartServer(string ip, string port)
        {
            try
            {
                //CreateWebHostBuilder(new string[] { }).Build().Run();

                var host = CreateWebHostBuilder(new string[] { ip, port }).Build();
                HUB = (IHubContext<ChatHub>)host.Services.GetService(typeof(IHubContext<ChatHub>));

                HUB.Clients.Group(ChatSettings.ChatGroup).SendAsync(ChatSettings.RecieveCommand, ChatSettings.ServerUser, "Start");
                host.Run();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
          .ConfigureLogging(logging =>
          {
              logging.ClearProviders();
              logging.AddConsole();
          })
          .ConfigureWebHostDefaults(webBuilder =>
          {
              webBuilder.ConfigureKestrel((context, options) =>
              {
                  IPAddress address = IPAddress.Parse(args[0]);
                  int port = Int32.Parse(args[1]);
                  options.Listen(address, port);
              })
              .UseStartup<Startup>();
          });

        #region SecondaryDisplay
        public void ToggleSecondarySubVisibility(bool isSubtitleVisible)
        {
            try
            {
                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                          _SecondaryWindow.ToggleSubVisibility(isSubtitleVisible)));
            }
            catch (Exception ex)
            {
                throw new Exception("SecondaryWindowSub", ex);
            }
        }

        public void DisplaySecondarySub(string subtitleLine)
        {
            try
            {
                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                   _SecondaryWindow.DisplayCurrentSub(subtitleLine)));
            }
            catch (Exception ex)
            {
                throw new Exception("SecondaryWindowSub", ex);
            }
        }

        public void PlaySecondaryVideo(string videoFilePath, bool isMuted, bool showVideo, bool showSub)
        {
            try
            {
                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                    _SecondaryWindow.Play(videoFilePath, isMuted, showVideo, showSub)));
            }
            catch (Exception ex)
            {
                throw new Exception("SecondaryWindowVideo", ex);
            }

        }

        public void ToggleVideoVisibility(bool display)
        {
            try
            {
                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                    _SecondaryWindow.ToggleVideoVisibility(display)));
            }
            catch (Exception ex)
            {
                throw new Exception("SecondaryWindowVideo", ex);
            }

        }

        public string GetVideoTime()
        {
            string timer = "";
            try
            {
                timer = _SecondaryWindow.VideoTime;
            }
            catch (Exception ex)
            {
                throw new Exception("SecondaryWindowVideo", ex);
            }
            return timer;
        }

        public void PauseVideo()
        {
            try
            {
                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                    _SecondaryWindow.PauseVideo()));
            }
            catch (Exception ex)
            {
                throw new Exception("SecondaryWindowVideo", ex);
            }

        }

        public void ResumeVideo()
        {
            try
            {
                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                    _SecondaryWindow.ResumeVideo()));
            }
            catch (Exception ex)
            {
                throw new Exception("SecondaryWindowVideo", ex);
            }

        }

        private void btnvdOpenSecondary_Click(object sender, RoutedEventArgs e)
        {
            if (_SecondaryWindow != null)
            {
                _SecondaryWindow.Close();
            }
            _SecondaryWindow = new SecondaryWindow();

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings["SecondaryLeft"] != null && settings["SecondaryTop"] != null)
            {
                int left = Convert.ToInt32(settings["SecondaryLeft"].Value.ToString());
                int top = Convert.ToInt32(settings["SecondaryTop"].Value.ToString());
                _SecondaryWindow.Left = left;
                _SecondaryWindow.Top = top;
            }

            if (rdSecondaryWindowFull.IsChecked == true)
            {
                _SecondaryWindow.WindowStyle = WindowStyle.None;
                _SecondaryWindow.WindowState = WindowState.Maximized;
            }
            else if (rdSecondaryWindowNoraml.IsChecked == true)
            {
                _SecondaryWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                _SecondaryWindow.WindowState = WindowState.Normal;
            }
            _SecondaryWindow.Show();

            this.tabControl.IsEnabled = true;
        }

        private void btnvdCloseSecondary_Click(object sender, RoutedEventArgs e)
        {
            _SecondaryWindow.Close();
        }
        #endregion

        private void rdSecondaryWindowNoraml_Checked(object sender, RoutedEventArgs e)
        {
            if (_SecondaryWindow != null)
            {
                _SecondaryWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                _SecondaryWindow.WindowState = WindowState.Normal;
            }
        }

        private void rdSecondaryWindowFull_Checked(object sender, RoutedEventArgs e)
        {
            if (_SecondaryWindow != null)
            {
                _SecondaryWindow.WindowStyle = WindowStyle.None;
                _SecondaryWindow.WindowState = WindowState.Maximized;

            }
        }


        public async Task SendGroupMessage(ServiceMessage message)
        {
            try
            {
                await Task.Run(() =>
                {
                    string messageText = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                    if (HUB != null)
                    {
                        HUB.Clients.Group(ChatSettings.ChatGroup).SendAsync(ChatSettings.RecieveCommand, ChatSettings.ServerUser, messageText);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Hub Exception", ex);
            }
        }


        public void AddUserStatus(SubmitUserStatus submitUserStatus)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                    this.ucUserStatus.AddUserStatus(submitUserStatus)
                ));
            }
            catch (Exception ex)
            {
                
            }
        }

        public Character AutoAssignCharacter()
        {
            try
            {
                Character autoassigned = Character.None;
                this.Dispatcher.Invoke(new Action(() =>
                    autoassigned = this.ucUserStatus.AutoAssignCharachter()
                ));
                return autoassigned;
            }
            catch (Exception ex)
            {
                
                return Character.Lyla;
            }
        }

        public void AddQuestion(SubmitQuestion submitQuestion)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                    this.ucQuestionnaire.AddUserStatus(submitQuestion)
                ));
            }
            catch (Exception ex)
            {

            }
        }

        public void ShowSubtitle()
        {
            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.Dispatcher.Invoke(new Action(() =>
                    ucTheater.ShowSubtitle()
                ));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {

            }
        }

        public void HideSubtitle()
        {
            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.Dispatcher.Invoke(new Action(() =>
                    ucTheater.HideSubtitle()
                ));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {

            }
        }

        public void PauseAutoSub()
        {
            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.Dispatcher.Invoke(new Action(() =>
                    ucTheater.PauseAutoSub()
                ));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {

            }
        }

        public void ResumeAutoSub()
        {
            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.Dispatcher.Invoke(new Action(() =>
                    ucTheater.ResumeAutoSub()
                ));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {

            }
        }

        public void NextMaunualSub()
        {
            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.Dispatcher.Invoke(new Action(() =>
                    ucTheater.LoadNextSubtitleLine()
                ));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {

            }
        }

        private void cmbServerIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnStartServer.IsEnabled = true;
        }

        private async void btnNavigateTheater_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                await SendGroupMessage(new ServiceMessage
                {
                    Command = Command.NavigateTheater,
                    Mode = Mode.None
                });
            }
        }

        private async void btnResetMediaCache_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                await SendGroupMessage(new ServiceMessage
                {
                    Command = Command.ResetMediaCache,
                    Mode = Mode.None
                });
            }
        }
    }
}
