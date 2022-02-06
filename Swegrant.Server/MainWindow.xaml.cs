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
        private Mode CurrentMode;
        private volatile bool continueAutoLine;

        private int theaterSceneSelectedIndex = 0;

        public static IHubContext<ChatHub> HUB { get; set; }

        private List<Subtitle> currentSub;
        private int currentSubIndex = 0;
        private int currentScene = 0;
        private Task currentSubTask;
        private CancellationTokenSource currentSubCancelationSource;
        private CancellationToken currentSubCancellationToken;

        private Task autoLine;

        private SecondaryWindow _SecondaryWindow;

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


        }

        private void FillTHSbutitleListBox(string text)
        {
            List<string> list = text.Split(new string[] { Environment.NewLine + Environment.NewLine },
                              StringSplitOptions.RemoveEmptyEntries).ToList();
            this.currentSub = new List<Subtitle>();
            this.currentSubIndex = 0;

            //List<string> subLine = new List<string>();
            foreach (var item in list)
            {
                Subtitle sub = new Subtitle();
                string[] parts = item.Split(new string[] { Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToArray();
                sub.Id = Convert.ToInt32(parts[0]);
                string[] times = parts[1].Split("-->", StringSplitOptions.RemoveEmptyEntries).ToArray();
                sub.StartTime = TimeSpan.Parse(times[0].Replace(',', '.').Trim());
                sub.EndTime = TimeSpan.Parse(times[1].Replace(',', '.').Trim());
                string line = "";
                for (int i = 2; i < parts.Length; i++)
                {
                    line += parts[i];
                }
                sub.Text = line;
                currentSub.Add(sub);
            }
            this.lstthSub.ItemsSource = currentSub.Select(c => c.Text).ToArray();
            this.lstthSub.SelectedIndex = 0;
        }

        private void FillVideoSbutitleListBox(string text)
        {
            List<string> list = text.Split(new string[] { Environment.NewLine + Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToList();
            this.currentSub = new List<Subtitle>();

            //List<string> subLine = new List<string>();
            foreach (var item in list)
            {
                Subtitle sub = new Subtitle();
                string[] parts = item.Split(new string[] { Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToArray();
                sub.Id = Convert.ToInt32(parts[0]);
                string[] times = parts[1].Split("-->", StringSplitOptions.RemoveEmptyEntries).ToArray();
                sub.StartTime = TimeSpan.Parse(times[0].Replace(',', '.').Trim());
                sub.EndTime = TimeSpan.Parse(times[1].Replace(',', '.').Trim());
                string line = "";
                for (int i = 2; i < parts.Length; i++)
                {
                    line += parts[i];
                }
                sub.Text = line;
                currentSub.Add(sub);
            }
            this.lstvdSub.ItemsSource = currentSub.Select(c => c.Text).ToArray();
            this.lstvdSub.SelectedIndex = 0;
        }

        private async void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            string localIP = Helpers.NetworkHelpers.GetLocalIPv4();
            string port = Swegrant.Shared.Models.ChatSettings.DefaultPort;
            if (!string.IsNullOrEmpty(localIP))
            {
                txtServerAddress.Text = $"{localIP}:{port}";
                Task task = new Task(() => StartServer(localIP, port));
                task.Start();
                MessageBox.Show("Server Started");
            }
            else
            {
                MessageBox.Show("IP NOT FOUND!");
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

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string message = this.lstthSub.SelectedItem.ToString();
                this.lstthSub.SelectedIndex = this.lstthSub.SelectedIndex + 1;
                Task.Run(() =>
                {
                    _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                            _SecondaryWindow.DisplayCurrentSub(this.currentSub[this.currentSubIndex].Text)));
                    SendGroupMessage(new ServiceMessage
                    {
                        Command = Command.DisplayManualSub,
                        Index = currentSubIndex,
                        Scene = this.currentScene
                    });
                });
                this.currentSubIndex++;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnNextAuto_Click(object sender, RoutedEventArgs e)
        {
            this.continueAutoLine = true;
            //this.autoLine = new Task(() => AutoLine());
            //this.autoLine.Start();

            while (this.continueAutoLine)
            {
                string message = this.lstthSub.SelectedItem.ToString();
                this.lstthSub.SelectedIndex = this.lstthSub.SelectedIndex + 1;
                HUB.Clients.Group(ChatSettings.ChatGroup).SendAsync(ChatSettings.RecieveCommand, Swegrant.Shared.Models.ChatSettings.ServerUser, message);
                int delay = message.Length * 70;
                Thread.Sleep(delay);
            }

        }

        private void AutoLine()
        {
            while (this.continueAutoLine)
            {
                string message = this.lstthSub.SelectedItem.ToString();
                this.lstthSub.SelectedIndex = this.lstthSub.SelectedIndex + 1;
                HUB.Clients.Group(ChatSettings.ChatGroup).SendAsync(ChatSettings.RecieveCommand, Swegrant.Shared.Models.ChatSettings.ServerUser, message);
                int delay = message.Length * 70;
                Thread.Sleep(delay);
            }
        }

        private void btnNextAuto_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.continueAutoLine = false;
        }

        private void btnstopAutoLine_Click(object sender, RoutedEventArgs e)
        {
            this.continueAutoLine = false;
        }


        //private void FillVideoListBox()
        //{
        //    string videoDirectory = $"{Directory.GetCurrentDirectory()}\\Video\\Background";
        //    DirectoryInfo d = new DirectoryInfo(videoDirectory);
        //    FileInfo[] Files = d.GetFiles();
        //    //foreach( var file in  Files)
        //    //{
        //    //    this.lstthSub.Items.Add(file.Name);
        //    //}
        //    this.lstVideos.ItemsSource = Files.Select(c => c.Name).ToArray();
        //}




        private static void PLayVideo(string videoFilePath)
        {
            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = @"C:\Program Files\Combined Community Codec Pack 64bit\MPC\mpc-hc64.exe";
            process.StartInfo.Arguments = "/fullscreen " + videoFilePath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
            //process.WaitForExit();// Waits here for the process to exit.
        }

        private void btnLoadSubTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    string text = "";
                    string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
                    string scence = this.cmbthScence.SelectionBoxItem.ToString();
                    this.currentScene = Convert.ToInt32(scence);
                    string subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA\\THSUB";
                    string subtitleFilePath = $"{subtitleDirectory}\\TH-SUB-{lang}-SC-{scence}.txt";
                    if (File.Exists(subtitleFilePath))
                    {
                        text = System.IO.File.ReadAllText(subtitleFilePath);
                        FillTHSbutitleListBox(text);
                        Task.Run(() =>
                        {
                            SendGroupMessage(new ServiceMessage
                            {
                                Command = Command.Prepare,
                                Mode = Mode.Theater,
                                Scene = this.currentScene
                            });
                        });
                    }
                    else
                    {
                        MessageBox.Show("File Does NOT Exist");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnthPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    string text = "";
                    //string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
                    string scence = this.cmbthScence.SelectionBoxItem.ToString();
                    this.currentScene = Convert.ToInt32(scence);
                    string VideoDirectory = $"{Directory.GetCurrentDirectory()}\\Theater";
                    string videoFilePath = $"{VideoDirectory}\\TH-BK-SC-{scence}.mp4";
                    if (File.Exists(videoFilePath))
                    {
                        SendGroupMessage(new ServiceMessage
                        {
                            Command = Command.Play,
                            Mode = Mode.Theater,
                            Scene = this.currentScene
                        });
                        this.currentSubCancelationSource = new CancellationTokenSource();
                        this.currentSubTask = Task.Run(() =>
                       {
                           this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                           PlaySub();

                       }, this.currentSubCancelationSource.Token);
                        _SecondaryWindow.Play(videoFilePath);


                    }
                    else
                    {
                        MessageBox.Show("File Does NOT Exist");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnvdLoadSubTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = "";
                string charachter = this.cmbvdCharchter.SelectionBoxItem.ToString();
                string lang = this.cmbvdLanguage.SelectionBoxItem.ToString();
                string scence = this.cmbvdScence.SelectionBoxItem.ToString();
                string subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA\\VDSUB";
                string subtitleFilePath = $"{subtitleDirectory}\\VD-{charachter}-SUB-{lang}-SC-{scence}.txt";
                if (File.Exists(subtitleFilePath))
                {
                    text = System.IO.File.ReadAllText(subtitleFilePath);
                    FillVideoSbutitleListBox(text);
                }
                else
                {
                    MessageBox.Show("File Does NOT Exist");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnvdPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                string text = "";
                //string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
                string scence = this.cmbvdScence.SelectionBoxItem.ToString();
                this.currentScene = Convert.ToInt32(scence);
                string VideoDirectory = $"{Directory.GetCurrentDirectory()}\\Video";
                string videoFilePath = $"{VideoDirectory}\\VD-SC-{scence}.mp4";
                if (File.Exists(videoFilePath))
                {

                    SendGroupMessage(new ServiceMessage
                    {
                        Command = Command.Play,
                        Mode = Mode.Video,
                        Scene = this.currentScene
                    });

                    //this.currentSubCancelationSource = new CancellationTokenSource();
                    //this.currentSubTask = Task.Run(() =>
                    //{
                    //    this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                    //    PlaySub();

                    //}, this.currentSubCancelationSource.Token);

                    _SecondaryWindow.Play(videoFilePath);

                    //PLayVideo(videoFilePath);
                    //Task.Run(PlaySub);
                    //_SecondaryWindow.Play(videoFilePath);


                }
                else
                {
                    MessageBox.Show("File Does NOT Exist");
                }
            }
        }



        private void PlaySub()
        {
            if (this.currentSubIndex == 0)
            {
                TimeSpan initial = this.currentSub[this.currentSubIndex].StartTime;
                Thread.Sleep(initial);
            }
            for (int i = this.currentSubIndex; i < this.currentSub.Count - 1; i++)
            {
                try
                {
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancellationToken.ThrowIfCancellationRequested();
                        return;
                    }

                    this.currentSubIndex = i;

                    if (CurrentMode == Mode.Theater)
                    {
                        _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                           _SecondaryWindow.DisplayCurrentSub(currentSub[i].Text)
                            ));
                    }
                    else if (CurrentMode == Mode.Video)
                    {
                        txtCurrentLine.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            txtCurrentLine.Text = currentSub[i].Text;

                        }));
                    }

                    //HUB.Clients.Group(ChatSettings.ChatGroup).SendAsync(ChatSettings.RecieveCommand, ChatSettings.ServerUser, this.currentSub[i].Text);

                    Thread.Sleep(currentSub[i].Duration);
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancellationToken.ThrowIfCancellationRequested();
                        return;
                    }

                    if (CurrentMode == Mode.Theater)
                    {
                        _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                            _SecondaryWindow.DisplayCurrentSub(" ")
                            ));
                        this.Dispatcher.BeginInvoke(new Action(() =>
                           this.lstthSub.SelectedIndex = this.lstthSub.SelectedIndex + 1
                        ));
                    }
                    else if (CurrentMode == Mode.Video)
                    {
                        txtCurrentLine.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            txtCurrentLine.Text = "";
                        }));
                        this.Dispatcher.BeginInvoke(new Action(() =>
                           this.lstvdSub.SelectedIndex = this.lstvdSub.SelectedIndex + 1
                        ));
                    }

                    //HUB.Clients.Group(ChatSettings.ChatGroup).SendAsync(ChatSettings.RecieveCommand, ChatSettings.ServerUser, " ");

                    TimeSpan gap = currentSub[i + 1].StartTime - currentSub[i].EndTime;
                    Thread.Sleep(gap);
                }
                catch (Exception ex)
                {

                }
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

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabControl tab = (TabControl)e.Source;
                this.CurrentMode = (tab.SelectedIndex == 0 ? Mode.Theater : Mode.Video);
                SendGroupMessage(new ServiceMessage
                {
                    Command = Command.ChangeMode,
                    Mode = this.CurrentMode,
                });
                //do work when tab is changed
            }
        }

        private void btnthShowSub_Click(object sender, RoutedEventArgs e)
        {

            Task.Run(() =>
            {

                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                              _SecondaryWindow.ToggleSubVisibility(true)));
                SendGroupMessage(new ServiceMessage
                {
                    Command = Command.ShowSubtitle,
                    Mode = Mode.Theater,
                    Scene = currentScene
                });
            });
        }

        private void btnthHideSub_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {

                _SecondaryWindow.Dispatcher.BeginInvoke(new Action(() =>
                              _SecondaryWindow.ToggleSubVisibility(false)));
                SendGroupMessage(new ServiceMessage
                {
                    Command = Command.HideSubtitle,
                    Mode = Mode.Theater
                });
            });
        }

        private void btnthPlayVideo_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnthStopVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning");
                if (result == MessageBoxResult.OK)
                {
                    _SecondaryWindow.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnthPlayVideo_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void btnthChangeVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.cmbthScence.SelectedIndex != this.theaterSceneSelectedIndex)
                {
                    string text = "";
                    //string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
                    string scence = this.cmbthScence.SelectionBoxItem.ToString();
                    string VideoDirectory = $"{Directory.GetCurrentDirectory()}\\Theater\\Background";
                    string videoFilePath = $"{VideoDirectory}\\TH-BK-SC-{scence}.mp4";
                    if (File.Exists(videoFilePath))
                    {
                        //PLayVideo(videoFilePath);
                        //Task.Run(PlaySub);
                        _SecondaryWindow.Play(videoFilePath);
                        this.theaterSceneSelectedIndex = this.cmbthScence.SelectedIndex;

                    }
                    else
                    {
                        MessageBox.Show("File Does NOT Exist");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

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

        private void btnNextAuto_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnPauseAutoSub_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                Task.Run(() =>
                {
                    SendGroupMessage(new ServiceMessage
                    {
                        Command = Command.PauseAutoSub,
                        Mode = Mode.Theater,
                        Scene = this.currentScene
                    });
                    if (this.currentSubTask != null && this.currentSubTask.Status == TaskStatus.Running)
                    {
                        this.currentSubCancelationSource.Cancel();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRsumeAutoSub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.currentSubCancelationSource = new CancellationTokenSource();
                //this.currentSubCancellationToken = this.currentSubCancelationSource.Token;
                //this.currentSubTask = Task.Run(PlaySub);
                this.currentSubTask = Task.Run(() =>
                {
                    SendGroupMessage(new ServiceMessage
                    {
                        Command = Command.ResumeAutoSub,
                        Mode = Mode.Theater,
                        Scene = this.currentScene

                    });

                    this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                    PlaySub();


                }, this.currentSubCancelationSource.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbthScence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        public void SendGroupMessage(ServiceMessage message)
        {
            string messageText = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            if (HUB != null)
            {
                HUB.Clients.Group(ChatSettings.ChatGroup).SendAsync(ChatSettings.RecieveCommand, ChatSettings.ServerUser, messageText);
            }
        }

        private void btnvdprepareAudio_Click(object sender, RoutedEventArgs e)
        {
            string scence = this.cmbvdScence.SelectionBoxItem.ToString();
            SendGroupMessage(new ServiceMessage
            {
                Command = Command.Prepare,
                Mode = Mode.Video,
                Scene = Convert.ToInt32(scence)

            });
        }
    }
}
