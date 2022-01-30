using Newtonsoft.Json;
using Swegrant.Interfaces;
using Swegrant.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        private string[] urls;
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
                urls = new string[0];
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
                this.currentIndex++;
                progress += ((float)currentIndex / (float)urls.Length);

                if (progress > 1)
                {
                    progress = 0;
                }

                // directly set the new progress value
                defaultProgressBar.Progress = progress;

                // animate to the new value over 750 milliseconds using Linear easing
                await defaultProgressBar.ProgressTo(progress, 750, Easing.Linear);
                downloader.DownloadFile(urls[this.currentIndex], "Audio");
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
            string[] urls = await GetFileUrls();

            downloader.DownloadFile(urls[0], "Audio");

        }

        public async Task<string[]> GetFileUrls()
        {
            string[] urls = new string[0];
            try
            {
                Uri uri = new Uri($"{(Helpers.Settings.UseHttps ? "https" : "http")}://{Helpers.Settings.ServerIP}:{Helpers.Settings.ServerPort}/api/media");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    urls = JsonConvert.DeserializeObject<string[]>(content);
                }
            }
            catch (Exception ex)
            {
                return new List<string>().ToArray();
            }
            return urls;
        }
    }
}