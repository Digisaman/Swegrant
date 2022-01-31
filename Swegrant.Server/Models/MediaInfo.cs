using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swegrant.Server.Models
{
    public class MediaInfo
    {
        public MediaInfo()
        {
            AUDIO = new List<MediaFile>();
            THSUB = new List<MediaFile>();
            VDSUB = new List<MediaFile>();
        }
        public List<MediaFile> AUDIO { get; set; }

        public List<MediaFile> THSUB { get; set; }

        public List<MediaFile> VDSUB { get; set; }
    }

    public class MediaFile
    {
        public string FileName { get; set; }

        public string Version { get; set; }

        public string Url { get; set; }

        public bool IsAvailable { get; set; }
    }
}
