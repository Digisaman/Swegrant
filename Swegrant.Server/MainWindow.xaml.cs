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

namespace Swegrant.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Timers.Timer timer;
        public static IHubContext<ChatHub> HUB { get; set; }
        //private HttpSelfHostServer restService;
        //private IDisposable apiServer;
        public MainWindow()
        {
            InitializeComponent();
            FillListBox();
            timer = new System.Timers.Timer();
            timer.Interval = TimeSpan.FromMilliseconds(1000).TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string message = this.lstBox.SelectedItem.ToString();
            this.lstBox.SelectedIndex = this.lstBox.SelectedIndex + 1;
            HUB.Clients.Group("Xamarin").SendAsync("ReceiveMessage", "User1", message);
        }

        private void FillListBox()
        {
            string text = Properties.Recources.Theater_01_EN;
            
            List<string> list = text.Split(new string[] { Environment.NewLine + Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> list2 = new List<string>();
            foreach(var item in list)
            {
                string[] parts = item.Split(new string[] { Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToArray();
                string line = "";
                for( int i = 2; i < parts.Length; i++)
                {
                    line += parts[i];
                }
                list2.Add(line);
            }
            this.lstBox.ItemsSource = list2;
            this.lstBox.SelectedIndex = 0;
        }

        private async void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() => StartServer());
            task.Start();
            MessageBox.Show("Server Started");
        }

        private static void StartServer()
        {
            try
            {
                //CreateWebHostBuilder(new string[] { }).Build().Run();

                var host = CreateWebHostBuilder(new string[] { }).Build();
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
#if DEBUG
                  options.Listen(IPAddress.Loopback, 5000);
#endif
              })
              .UseStartup<Startup>();
          });

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = this.lstBox.SelectedItem.ToString();
                this.lstBox.SelectedIndex = this.lstBox.SelectedIndex + 1;
                HUB.Clients.Group("Xamarin").SendAsync("ReceiveMessage", "User1", message);
            }
            catch(Exception ex)
            {

            }
        }

        private void btnNextAuto_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                string message = this.lstBox.SelectedItem.ToString();
                this.lstBox.SelectedIndex = this.lstBox.SelectedIndex + 1;
                HUB.Clients.Group("Xamarin").SendAsync("ReceiveMessage", "User1", message);
                int delay = message.Length * 70;
                Thread.Sleep(delay);
            }
        }

        //private bool StartWebApp(string baseAddress)
        //{
        //    string responseText = "";
        //    try
        //    {

        //        //    //this.apiServer = WebApp.Start<Startup>(baseAddress);

        //        //    ////Create HttpClient and make a request to api/ values
        //        //    //HttpClient client = new HttpClient();

        //        //    //HttpResponseMessage response = await client.GetAsync(baseAddress + "api/media/GetCurrentTime");


        //        //    var config = new HttpSelfHostConfiguration(baseAddress);
        //        //    config.Routes.MapHttpRoute(
        //        //        "Scale Comm API Default", "api/{controller}/{action}",
        //        //        new { id = RouteParameter.Optional });

        //        //    config.Formatters.Clear();
        //        //    config.Formatters.Add(new JsonMediaTypeFormatter());
        //        //    config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
        //        //    {
        //        //        ContractResolver = new CamelCasePropertyNamesContractResolver()
        //        //    };
        //        //    config.MapHttpAttributeRoutes();
        //        //    config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
        //        //    //ConfigureSwagger(config);
        //        //    restService = new HttpSelfHostServer(config);
        //        //    restService.OpenAsync().Wait();


        //        //}
        //        //catch(Exception ex)
        //        //{
        //        //    MessageBox.Show(ex.Message);
        //        //    return false;
        //        //}
        //        //return true;
        //    }


        ////private static void ConfigureSwagger(HttpSelfHostConfiguration config)
        ////{
        ////    //Configure swagger
        ////    config.EnableSwagger((c) =>
        ////    {
        ////        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        ////        var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
        ////        var commentsFile = Path.Combine(baseDirectory, commentsFileName);

        //    //        c.SingleApiVersion("v1", "Scale Communication API");
        //    //        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        //    //        //c.IncludeXmlComments(comments
        //    //        c.RootUrl(req => { return "http://localhost:2016"; });
        //    //    }).EnableSwaggerUi();
        //    //}



        //    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //    {
        //        if (this.restService != null)
        //        {
        //            this.restService.Dispose();
        //        }
        //    }
        //}
    }
}
