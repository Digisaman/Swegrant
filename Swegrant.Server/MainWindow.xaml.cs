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


namespace Swegrant.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private HttpSelfHostServer restService;
        //private IDisposable apiServer;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            CreateWebHostBuilder(new string[] { }).Build().Run();

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
