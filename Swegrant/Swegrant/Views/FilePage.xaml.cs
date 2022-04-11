using Newtonsoft.Json;
using Swegrant.Helpers;
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
        private MediaInfo currentMediaInfo;
        private MediaInfo clientMediaInfo;
        private MediaInfo serverMediaInfo;
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
                this.currentMediaInfo = new MediaInfo();
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
                if (currentMediaInfo.CurrentCategory == DownloadCategory.AUDIO)
                {
                    if ((currentIndex+1) < currentMediaInfo.AUDIO.Count)
                    {
                        progress = ((float)(currentIndex + 1) / (float)currentMediaInfo.AUDIO.Count);
                        // directly set the new progress value
                        defaultProgressBar.Progress = progress;

                        // animate to the new value over 750 milliseconds using Linear easing
                        await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);

                        this.currentIndex++;
                        downloader.DownloadFile(currentMediaInfo.AUDIO[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                    }
                    else
                    {
                        await defaultProgressBar.ProgressTo(1, 500, Easing.Linear);
                        if (currentMediaInfo.THSUB.Any())
                        {
                            currentMediaInfo.CurrentCategory = DownloadCategory.THSUB;
                            currentIndex = 0;
                            downloader.DownloadFile(currentMediaInfo.THSUB[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                            this.lblTitle.Text = Swegrant.Resources.AppResources.DownloadTHSubFiles;
                        }
                        else if (currentMediaInfo.VDSUB.Any())
                        {
                            await defaultProgressBar.ProgressTo(1, 500, Easing.Linear);
                            currentMediaInfo.CurrentCategory = DownloadCategory.VDSUB;
                            currentIndex = 0;
                            downloader.DownloadFile(currentMediaInfo.VDSUB[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                            this.lblTitle.Text = Swegrant.Resources.AppResources.DownloadVDSubFiles;
                        }
                        else
                        {
                            progress = 1;
                            currentIndex = 0;
                            Helpers.Settings.MediaInfo = currentMediaInfo;
                            await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);
                            Helpers.Settings.MediaInfo = this.serverMediaInfo;
                            this.lblTitle.Text = Swegrant.Resources.AppResources.DownloadCompleted;
                        }


                    }
                }
                else if (currentMediaInfo.CurrentCategory == DownloadCategory.THSUB)
                {
                    if ((currentIndex + 1) < currentMediaInfo.THSUB.Count)
                    {
                        progress = ((float)(currentIndex + 1) / (float)currentMediaInfo.THSUB.Count);
                        // directly set the new progress value
                        defaultProgressBar.Progress = progress;

                        // animate to the new value over 750 milliseconds using Linear easing
                        await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);

                        this.currentIndex++;
                        downloader.DownloadFile(currentMediaInfo.THSUB[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                    }
                    else
                    {
                        await defaultProgressBar.ProgressTo(1, 500, Easing.Linear);

                        if (currentMediaInfo.VDSUB.Any())
                        {
                            await defaultProgressBar.ProgressTo(1, 500, Easing.Linear);
                            currentMediaInfo.CurrentCategory = DownloadCategory.VDSUB;
                            currentIndex = 0;
                            downloader.DownloadFile(currentMediaInfo.VDSUB[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                            this.lblTitle.Text = Swegrant.Resources.AppResources.DownloadVDSubFiles;
                        }
                        else
                        {
                            progress = 1;
                            currentIndex = 0;
                            Helpers.Settings.MediaInfo = currentMediaInfo;
                            await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);
                            Helpers.Settings.MediaInfo = this.serverMediaInfo;
                            this.lblTitle.Text = Swegrant.Resources.AppResources.DownloadCompleted;
                        }
                    }

                }
                else if (currentMediaInfo.CurrentCategory == DownloadCategory.VDSUB)
                {
                    if ((currentIndex + 1) < currentMediaInfo.VDSUB.Count)
                    {
                        progress = ((float)(currentIndex + 1) / (float)currentMediaInfo.VDSUB.Count);
                        // directly set the new progress value
                        defaultProgressBar.Progress = progress;

                        // animate to the new value over 750 milliseconds using Linear easing
                        await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);

                        this.currentIndex++;
                        downloader.DownloadFile(currentMediaInfo.VDSUB[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                    }
                    else
                    {
                        progress = 1;
                        currentIndex = 0;
                        Helpers.Settings.MediaInfo = currentMediaInfo;
                        await defaultProgressBar.ProgressTo(progress, 500, Easing.Linear);
                        Helpers.Settings.MediaInfo = this.serverMediaInfo;
                        this.lblTitle.Text = Swegrant.Resources.AppResources.DownloadCompleted;

                        Thread.Sleep(500);
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
            this.serverMediaInfo = await ServerHelper.GetMediaInfo();
            this.clientMediaInfo = Helpers.Settings.MediaInfo;
            currentMediaInfo = new MediaInfo();
            CalculateMediaInfo();
           
            Helpers.Settings.Questionnaire = await ServerHelper.GetQuestionnaire();

            this.currentIndex = 0;
            if (this.currentMediaInfo.HasFiles)
            {
                if (this.currentMediaInfo.AUDIO.Count > 0)
                {
                    this.lblTitle.Text = Swegrant.Resources.AppResources.DonwloadingAudioFiles;
                    currentMediaInfo.CurrentCategory = DownloadCategory.AUDIO;
                    downloader.DownloadFile(currentMediaInfo.AUDIO[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                    return;
                }

                if (this.currentMediaInfo.THSUB.Count > 0)
                {
                    this.lblTitle.Text = Swegrant.Resources.AppResources.DownloadTHSubFiles;
                    currentMediaInfo.CurrentCategory = DownloadCategory.AUDIO;
                    downloader.DownloadFile(currentMediaInfo.THSUB[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                    return;
                }

                if (this.currentMediaInfo.VDSUB.Count > 0)
                {
                    this.lblTitle.Text = Swegrant.Resources.AppResources.DownloadVDSubFiles;
                    downloader.DownloadFile(currentMediaInfo.VDSUB[currentIndex].Url, currentMediaInfo.CurrentCategory.ToString());
                    return;
                }
            }
            else
            {
                this.lblTitle.Text = Swegrant.Resources.AppResources.FilesUpToDate;
                await defaultProgressBar.ProgressTo(1, 500, Easing.Linear);
                Thread.Sleep(500);
                NavigateMain();
            }

        }

        private void CalculateMediaInfo()
        {
            foreach(var item in serverMediaInfo.AUDIO)
            {
                MediaFile currentServerFile = serverMediaInfo.AUDIO.FirstOrDefault(c => c.FileName == item.FileName);
                MediaFile currentClientFile = clientMediaInfo.AUDIO.FirstOrDefault(c => c.FileName == item.FileName);
                if (currentClientFile != null)
                {
                    if (currentClientFile.Version != currentServerFile.Version)
                    {
                        currentMediaInfo.AUDIO.Add(currentServerFile);
                    }
                }
                else
                {
                    currentMediaInfo.AUDIO.Add(currentServerFile);
                }
            }

            foreach (var item in serverMediaInfo.VDSUB)
            {
                MediaFile currentServerFile = serverMediaInfo.VDSUB.FirstOrDefault(c => c.FileName == item.FileName);
                MediaFile currentClientFile = clientMediaInfo.VDSUB.FirstOrDefault(c => c.FileName == item.FileName);
                if (currentClientFile != null)
                {
                    if (currentClientFile.Version != currentServerFile.Version)
                    {
                        currentMediaInfo.VDSUB.Add(currentServerFile);
                    }
                }
                else
                {
                    currentMediaInfo.VDSUB.Add(currentServerFile);
                }
            }

            foreach (var item in serverMediaInfo.THSUB)
            {
                MediaFile currentServerFile = serverMediaInfo.THSUB.FirstOrDefault(c => c.FileName == item.FileName);
                MediaFile currentClientFile = clientMediaInfo.THSUB.FirstOrDefault(c => c.FileName == item.FileName);
                if (currentClientFile != null)
                {
                    if (currentClientFile.Version != currentServerFile.Version)
                    {
                        currentMediaInfo.THSUB.Add(currentServerFile);
                    }
                }
                else
                {
                    currentMediaInfo.THSUB.Add(currentServerFile);
                }
            }
        }

        

        private async void NavigateMain()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}