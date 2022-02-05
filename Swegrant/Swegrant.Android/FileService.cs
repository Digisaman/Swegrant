using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Swegrant.Droid;
using Android.Media;
using Android.Content.Res;
using Swegrant.Interfaces;
using System.IO;
using Swegrant.Shared.Models;

[assembly: Dependency(typeof(FileService))]
namespace Swegrant.Droid
{
    public class FileService : IFileservice
    {

        public FileService()
        {
            
        }

        public string ReadTextFile(DownloadCategory category, string fileName)
        {
            string content = "";
            try
            {
                string externalStorageDirectory = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
                string pathToDirectory = Path.Combine(externalStorageDirectory, category.ToString());
                string pathToFile = Path.Combine(pathToDirectory, fileName);

                if (File.Exists(pathToFile))
                {
                    using (StreamReader streamReader = new StreamReader(pathToFile))
                    {
                        content = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return content;
        }
    }
}