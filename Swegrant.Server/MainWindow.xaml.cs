﻿using Microsoft.Extensions.Hosting;
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

namespace Swegrant.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private volatile bool continueAutoLine;
        private System.Timers.Timer timer;
        public static IHubContext<ChatHub> HUB { get; set; }

        private Task autoLine;
        //private HttpSelfHostServer restService;
        //private IDisposable apiServer;
        public MainWindow()
        {
            InitializeComponent();
            timer = new System.Timers.Timer();
            timer.Interval = TimeSpan.FromMilliseconds(1000).TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string message = this.lstthSub.SelectedItem.ToString();
            this.lstthSub.SelectedIndex = this.lstthSub.SelectedIndex + 1;
            HUB.Clients.Group("Xamarin").SendAsync("ReceiveMessage", "User1", message);
        }

        private void FillSbutitleListBox(string text)
        {
            List<string> list = text.Split(new string[] { Environment.NewLine + Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> list2 = new List<string>();
            foreach (var item in list)
            {
                string[] parts = item.Split(new string[] { Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToArray();
                string line = "";
                for (int i = 2; i < parts.Length; i++)
                {
                    line += parts[i];
                }
                list2.Add(line);
            }
            this.lstthSub.ItemsSource = list2;
            this.lstthSub.SelectedIndex = 0;
        }

        private void FillVideoSbutitleListBox(string text)
        {
            List<string> list = text.Split(new string[] { Environment.NewLine + Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> list2 = new List<string>();
            foreach (var item in list)
            {
                string[] parts = item.Split(new string[] { Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToArray();
                string line = "";
                for (int i = 2; i < parts.Length; i++)
                {
                    line += parts[i];
                }
                list2.Add(line);
            }
            this.lstvdSub.ItemsSource = list2;
            this.lstvdSub.SelectedIndex = 0;
        }

        private async void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            string localIP = Helpers.NetworkHelpers.GetLocalIPv4();
            string port = "5000";
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

                var host = CreateWebHostBuilder(new string[] { ip, port}).Build();
                HUB = (IHubContext<ChatHub>)host.Services.GetService(typeof(IHubContext<ChatHub>));

                HUB.Clients.Group("Xamarin").SendAsync("ReceiveMessage", "User1", "Start");
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
                HUB.Clients.Group("Xamarin").SendAsync("ReceiveMessage", "User1", message);
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
                HUB.Clients.Group("Xamarin").SendAsync("ReceiveMessage", "User1", message);
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
                HUB.Clients.Group("Xamarin").SendAsync("ReceiveMessage", "User1", message);
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
                string text = "";
                string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
                string scence = this.cmbthScence.SelectionBoxItem.ToString();
                string subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\Theater\\Subtitle";
                string subtitleFilePath = $"{subtitleDirectory}\\TH-SUB-{lang}-SC-{scence}.srt";
                if ( File.Exists(subtitleFilePath) )
                {
                    text = System.IO.File.ReadAllText(subtitleFilePath);
                    FillSbutitleListBox(text);
                }
                else
                {
                    MessageBox.Show("File Does NOT Exist");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnthPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = "";
                //string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
                string scence = this.cmbthScence.SelectionBoxItem.ToString();
                string VideoDirectory = $"{Directory.GetCurrentDirectory()}\\Theater\\Background";
                string videoFilePath = $"{VideoDirectory}\\TH-BK-SC-{scence}.mp4";
                if (File.Exists(videoFilePath))
                {
                    PLayVideo(videoFilePath);
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

        private void btnvdLoadSubTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = "";
                string charachter = this.cmbvdCharchter.SelectionBoxItem.ToString();
                string lang = this.cmbvdLanguage.SelectionBoxItem.ToString();
                string scence = this.cmbvdScence.SelectionBoxItem.ToString();
                string subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\Video\\Subtitle";
                string subtitleFilePath = $"{subtitleDirectory}\\VD-{charachter}-SUB-{lang}-SC-{scence}.srt";
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
            string text = "";
            //string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
            string scence = this.cmbvdScence.SelectionBoxItem.ToString();
            string VideoDirectory = $"{Directory.GetCurrentDirectory()}\\Video\\Background";
            string videoFilePath = $"{VideoDirectory}\\VD-SC-{scence}.mp4";
            if (File.Exists(videoFilePath))
            {
                PLayVideo(videoFilePath);
            }
            else
            {
                MessageBox.Show("File Does NOT Exist");
            }
        }
    }
}
