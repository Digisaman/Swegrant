using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swegrant.Server.Helpers
{
    public class FileHelpers
    {

        public static string[] GetFileList(Mode mode, int scene = 0)
        {
            DirectoryInfo mediaDirectory = null;
            if (mode == Mode.Video)
            {
                mediaDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\Video");
            }
            else if (mode == Mode.Theater)
            {
                mediaDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\Theater");
            }
            
            if (mediaDirectory != null)
            {
                FileInfo[] files = mediaDirectory.GetFiles("*.*", SearchOption.AllDirectories);
                if (scene == 0)
                    return files.Select( c=> c.Name ).ToArray();
                else
                    return files.Where( c => c.Name.Contains(scene.ToString("00"))).Select( c => c.Name ).ToArray();    
            }
            return null;
        }


    }
}
