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
                    txtServerAddress.Text = $"{localIP}:{port}";
                    Task task = new Task(() => StartServer(localIP, port));
                    task.Start();

                    Helpers.AppConfigHelpers.SaveConfig("ServerIP", localIP);

                    MessageBox.Show("Server Started");
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

        public void DisplaySecondaryVideo(string videoFilePath)
        {
            try
            {
                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                    _SecondaryWindow.Play(videoFilePath)));
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




    }
}
