using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Swegrant.Shared.Models
{
    public partial class MediaInfo
    {
        public MediaInfo()
        {
            Initialize();
        }

        public void Initialize()
        {
            AUDIO = new List<MediaFile>();
            THSUB = new List<MediaFile>();
            VDSUB = new List<MediaFile>();
        }

        public List<MediaFile> AUDIO { get; set; }

        public List<MediaFile> THSUB { get; set; }

        public List<MediaFile> VDSUB { get; set; }

        public bool HasFiles
        {
            get
            {
                return (AUDIO.Any() || THSUB.Any() || VDSUB.Any());
            }
        }

        public DownloadCategory CurrentCategory { get; set; }
    }

    public class MediaFile
    {
        public string FileName { get; set; }

        public string Version { get; set; }

        public string Url { get; set; }

        public bool IsAvailable { get; set; }
        public long FileSize { get; set; }
    }
}
