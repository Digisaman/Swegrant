using Newtonsoft.Json;
using Swegrant.Interfaces;
using Swegrant.Models;
using Swegrant.Shared.Models;
using Swegrant.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swegrant.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilePage : ContentPage
    {
        //FileViewModel vm;
        //FileViewModel VM
        //{
        //    get => vm ?? (vm = (FileViewModel)BindingContext);
        //}
        private MediaInfo mediaInfo;
        private int currentIndex;
        private IDownloader downloader = null;
        private float progress;

        public FilePage()
        {
            InitializeComponent();
            try
            {
                downloader = DependencyService.Get<IDownloader>();
                downloader.OnFileDownloaded += Downloader_OnFileDownloaded;
                this.mediaInfo = new MediaInfo();
            }
            catch (Exception ex)
            {

            }
        }

        private async void Downloader_OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            if (e.FileSaved)
            {
                //DisplayAlert("Swegrant", "File Saved Successfully", "Close");
                if (mediaInfo.CurrentCategory == DownloadCategory.AUDIO)
                {
                    if ((currentIndex+1) < mediaInfo.AUDIO.Count)
                    {
                        progress = ((float)(currentIndex + 1) / (float)mediaInfo.AUDIO.Count);
                        // directly set the new progress value
                        defaultProgressBar.Progress = progress;

                        // animate to the new value over 750 milliseconds using Linear easing
                        await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);

                        this.currentIndex++;
                        downloader.DownloadFile(mediaInfo.AUDIO[currentIndex].Url, mediaInfo.CurrentCategory.ToString());
                    }
                    else
                    {
                        await defaultProgressBar.ProgressTo(1, 500, Easing.Linear);
                        mediaInfo.CurrentCategory = DownloadCategory.THSUB;
                        currentIndex = 0;
                        downloader.DownloadFile(mediaInfo.THSUB[currentIndex].Url, mediaInfo.CurrentCategory.ToString());
                        this.lblTitle.Text = "Downloading Theater Subtitles...";
                    }
                }
                else if (mediaInfo.CurrentCategory == DownloadCategory.THSUB)
                {
                    if ((currentIndex + 1) < mediaInfo.THSUB.Count)
                    {
                        progress = ((float)(currentIndex + 1) / (float)mediaInfo.THSUB.Count);
                        // directly set the new progress value
                        defaultProgressBar.Progress = progress;

                        // animate to the new value over 750 milliseconds using Linear easing
                        await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);

                        this.currentIndex++;
                        downloader.DownloadFile(mediaInfo.THSUB[currentIndex].Url, mediaInfo.CurrentCategory.ToString());
                    }
                    else
                    {
                        await defaultProgressBar.ProgressTo(1, 500, Easing.Linear);
                        mediaInfo.CurrentCategory = DownloadCategory.VDSUB;
                        currentIndex = 0;
                        downloader.DownloadFile(mediaInfo.VDSUB[currentIndex].Url, mediaInfo.CurrentCategory.ToString());
                        this.lblTitle.Text = "Downloading Video Subtitles...";
                    }

                }
                else if (mediaInfo.CurrentCategory == DownloadCategory.VDSUB)
                {
                    if ((currentIndex + 1) < mediaInfo.VDSUB.Count)
                    {
                        progress = ((float)(currentIndex + 1) / (float)mediaInfo.VDSUB.Count);
                        // directly set the new progress value
                        defaultProgressBar.Progress = progress;

                        // animate to the new value over 750 milliseconds using Linear easing
                        await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);

                        this.currentIndex++;
                        downloader.DownloadFile(mediaInfo.VDSUB[currentIndex].Url, mediaInfo.CurrentCategory.ToString());
                    }
                    else
                    {
                        progress = 1;
                        currentIndex = 0;
                        
                        await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);
                        this.lblTitle.Text = "Download Completed";
                        Thread.Sleep(1000);
                        NavigateMain();
                    }
                }
            }
            else
            {
                //DisplayAlert("Swegrant", "Error while saving the file", "Close");
            }
        }

        //private void DownloadClicked(object sender, EventArgs e)
        //{
        //    string fileName = $"VD-{cmbCharachter.SelectedItem.ToString()}-AUD-{cmbLanguage.SelectedItem.ToString()}-SC-{cmbScene.SelectedItem.ToString()}.mp3";

        //    string url = $"{(Helpers.Settings.UseHttps ? "https" : "http")}://{Helpers.Settings.ServerIP}:{Helpers.Settings.ServerPort}/Video/Audio/{fileName}";

        //    downloader.DownloadFile(url, "Audio");
        //}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, Helpers.Settings.IsUserAdmin);
            this.mediaInfo = await GetMediaInfo();
            Helpers.Settings.Questionnaire = await GetQuestionnaire();

            this.currentIndex = 0;
            if (this.mediaInfo.AUDIO.Count > 0)
            {
                this.lblTitle.Text = "Downloading Audio files...";
                downloader.DownloadFile(mediaInfo.AUDIO[currentIndex].Url, mediaInfo.CurrentCategory.ToString());
            }

        }

        public async Task<MediaInfo> GetMediaInfo()
        {
            MediaInfo info = null;
            try
            {
                Uri uri = new Uri($"{(Helpers.Settings.UseHttps ? "https" : "http")}://{Helpers.Settings.ServerIP}:{Helpers.Settings.ServerPort}/api/media/GetMediaInfo");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    info = JsonConvert.DeserializeObject<MediaInfo>(content);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return info;
        }

        public async Task<Questionnaire> GetQuestionnaire()
        {
            Questionnaire info = null;
            try
            {
                Uri uri = new Uri($"{(Swegrant.Helpers.Settings.UseHttps ? "https" : "http")}://{Swegrant.Helpers.Settings.ServerIP}:{Swegrant.Helpers.Settings.ServerPort}/api/media/GetQuestionnaire");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    info = JsonConvert.DeserializeObject<Questionnaire>(content);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return info;
        }

        private async void NavigateMain()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}