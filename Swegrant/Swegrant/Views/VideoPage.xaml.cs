﻿using Swegrant.Interfaces;
using Swegrant.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swegrant.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage : ContentPage
    {
        VideoViewModel vm;
        VideoViewModel VM
        {
            get => vm ?? (vm = (VideoViewModel)BindingContext);
        }
        public VideoPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnAppearing()
        {
           

            try
            {
                base.OnAppearing();
                VM.ConnectCommand.Execute(null);
            }
            catch(Exception ex)
            {

            }
        }

        private void btnPlayEn_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("VD-LY-AUD-EN-SC-01.mp3");
        }

        private void btnStopPlayback_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().StopAudioFile();
        }

        private void btnPlayFa_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("VD-LY-AUD-FA-SC-01.mp3");
        }
    }
}