using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Swegrant.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        //[HttpGet, Route("api/media/GetCurrentTime")]
        //public DateTime GetCurrentTime()
        //{
        //    return DateTime.Now;
        //}

        [HttpGet]
        public string[] GetFileURL()
        {
            DirectoryInfo AudioDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\Video\\Audio");
            //string[] files = Directory.GetFiles(AudioDirectory);
            FileInfo[] Files = AudioDirectory.GetFiles();

            string localIP = Helpers.NetworkHelpers.GetLocalIPv4();
            string port = "5000";

            List<string> urls = new List<string>();
            foreach (FileInfo file in Files)
            {
                
                urls.Add(
                $"http://{localIP}:{port}/Video/Audio/{file.Name}"
                );
            }
            return urls.ToArray();



        }
    }
}
