using System.Collections.Generic;

namespace Swegrant.Shared.Models
{
    public partial class MediaInfo
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
