using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Swegrant.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenOwnerCommand = new Command(async () => await Browser.OpenAsync("https://www.chaharsoo.se/"));
            OpenDeveloperCommand = new Command(async () => await Browser.OpenAsync("http://samanpirooz.com/swegrant"));
            VersionTracking.Track();
        }

        public ICommand OpenOwnerCommand { get; }
        public ICommand OpenDeveloperCommand { get; }


        public string CurrentVersion
        {
            get
            {
                return VersionTracking.CurrentVersion; 
            }
        }

        public string CurrentBuild
        {
            get
            {
                return VersionTracking.CurrentBuild;
            }
        }
    }
}