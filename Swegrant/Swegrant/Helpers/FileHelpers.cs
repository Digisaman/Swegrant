using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Swegrant.Helpers
{
    public class FileHelpers
    {

        public static void ResetMediaCache()
        {
            string content = "";
            try
            {
                MediaInfo clientMediaInfo = Helpers.Settings.MediaInfo;
                clientMediaInfo.Initialize();
                Helpers.Settings.MediaInfo = clientMediaInfo;
            }
            catch (Exception ex)
            {

            }

        }

        
    }
}
