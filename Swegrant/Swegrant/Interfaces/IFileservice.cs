using System;
using System.Collections.Generic;
using System.Text;
using static Swegrant.Models.MediaInfo;

namespace Swegrant.Interfaces
{
    public interface IFileservice
    {
        string ReadTextFile(DownloadCategory category, string fileName);
    }
}
