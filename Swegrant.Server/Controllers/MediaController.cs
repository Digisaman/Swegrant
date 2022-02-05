using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swegrant.Shared.Models;
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
        [HttpGet]
        [Route(nameof(GetCurrentTime))]
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        [HttpGet]
        [Route(nameof(GetMediaInfo))]
        public MediaInfo GetMediaInfo()
        {
            DirectoryInfo mediaDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA");

            FileInfo[] files = mediaDirectory.GetFiles("*.*", SearchOption.AllDirectories);

            MediaInfo mediaInfo = null;
            string mediaInfoFilePath = $"{mediaDirectory}\\MediaInfo.json";
            using (StreamReader streamReader = new StreamReader(mediaInfoFilePath))
            {
                string content = streamReader.ReadToEnd();
                mediaInfo = JsonConvert.DeserializeObject<MediaInfo>(content);
            }

            string localIP = Helpers.NetworkHelpers.GetLocalIPv4();
            string port = "5000";
            string protocol = "http";

            List<string> urls = new List<string>();
            foreach (MediaFile mediafile in mediaInfo.AUDIO)
            {
                FileInfo file = files.FirstOrDefault(c => c.Name == mediafile.FileName);
                if (file != null)
                {
                    mediafile.Url = $"{protocol}://{localIP}:{port}/MEDIA/AUDIO/{file.Name}";
                    mediafile.IsAvailable = true;
                    mediafile.FileSize = file.Length;
                }
            }

            foreach (MediaFile mediafile in mediaInfo.THSUB)
            {
                FileInfo file = files.FirstOrDefault(c => c.Name == mediafile.FileName);
                if (file != null)
                {
                    mediafile.Url = $"{protocol}://{localIP}:{port}/MEDIA/THSUB/{file.Name}";
                    mediafile.IsAvailable = true;
                    mediafile.FileSize = file.Length;
                }
            }

            foreach (MediaFile mediafile in mediaInfo.VDSUB)
            {
                FileInfo file = files.FirstOrDefault(c => c.Name == mediafile.FileName);
                if (file != null)
                {
                    mediafile.Url = $"{protocol}://{localIP}:{port}/MEDIA/VDSUB/{file.Name}";
                    mediafile.IsAvailable = true;
                    mediafile.FileSize = file.Length;
                }
            }

            mediaInfo.AUDIO = mediaInfo.AUDIO.Where(c => c.IsAvailable).ToList();
            mediaInfo.THSUB = mediaInfo.THSUB.Where(c => c.IsAvailable).ToList();
            mediaInfo.VDSUB = mediaInfo.VDSUB.Where(c => c.IsAvailable).ToList();


            return mediaInfo;



        }


    }
}
