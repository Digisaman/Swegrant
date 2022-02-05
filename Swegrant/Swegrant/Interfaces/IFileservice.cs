using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace Swegrant.Interfaces
{
    public interface IFileservice
    {
        string ReadTextFile(DownloadCategory category, string fileName);
    }
}
