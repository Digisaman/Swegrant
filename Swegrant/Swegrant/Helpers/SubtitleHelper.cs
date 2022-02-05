using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Swegrant.Helpers
{
    public class SubtitleHelper
    {
        public static Subtitle[] PopulateSubtitle(string text)
        {
            List<Subtitle> subtitles = new List<Subtitle>();
            try
            {
                List<string> list = text.Split(new string[] { "\r\n" + "\r\n" },
                                   StringSplitOptions.RemoveEmptyEntries).ToList();


                //List<string> subLine = new List<string>();
                foreach (var item in list)
                {
                    Subtitle sub = new Subtitle();
                    string[] parts = item.Split(new string[] { Environment.NewLine },
                                   StringSplitOptions.RemoveEmptyEntries).ToArray();
                    sub.Id = Convert.ToInt32(parts[0]);
                    string[] times = parts[1].Split(new string[] { "-->" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    sub.StartTime = TimeSpan.Parse(times[0].Replace(',', '.').Trim());
                    sub.EndTime = TimeSpan.Parse(times[1].Replace(',', '.').Trim());
                    string line = "";
                    for (int i = 2; i < parts.Length; i++)
                    {
                        line += parts[i];
                    }
                    sub.Text = line;
                    subtitles.Add(sub);
                }
            }
            catch(Exception ex)
            {

            }
            return subtitles.ToArray();
        }

        public static string ReadSubtitleFile(Mode currentMode, string filename)
        {
            string content = "";
            try
            {
                string directory = (currentMode == Mode.Theater ? DownloadCategory.THSUB.ToString() : DownloadCategory.VDSUB.ToString());
                string subtitleDirectoty = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), directory);
                string fileName = Path.Combine(subtitleDirectoty, filename);
                content = File.ReadAllText(fileName);
            }
            catch (Exception ex)
            {

            }
            return content;

        }
    }
}
