using Swegrant.Interfaces;
using Swegrant.Shared.Models;
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
                Shell.SetNavBarIsVisible(this, Helpers.Settings.IsUserAdmin);
                VM.ConnectCommand.Execute(null);
            }
            catch(Exception ex)
            {
                DependencyService.Get<IAudio>().StopAudioFile(VM.CurrnetAudioLanguage);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        

        //private void btnPlayEn_Clicked(object sender, EventArgs e)
        //{
        //    VM.CurrnetAudioLanguage = Language.English;
        //    VM.ChangeAudioCommand.Execute(null);
        //    //DependencyService.Get<IAudio>().PlayAudioFile("VD-LY-AUD-EN-SC-01.mp3");
        //}

        //private void btnStopPlayback_Clicked(object sender, EventArgs e)
        //{
        //    DependencyService.Get<IAudio>().StopAudioFile(VM.CurrnetAudioLanguage);
        //}

        //private void btnPlayFa_Clicked(object sender, EventArgs e)
        //{
        //    VM.CurrnetAudioLanguage = Swegrant.Shared.Models.Language.Farsi;
        //    VM.ChangeAudioCommand.Execute(null);
        //    //DependencyService.Get<IAudio>().PlayAudioFile("VD-LY-AUD-FA-SC-01.mp3");
        //}

        //private void btnPlaySV_Clicked(object sender, EventArgs e)
        //{
        //    VM.CurrnetAudioLanguage = Swegrant.Shared.Models.Language.Svenska;
        //    VM.ChangeAudioCommand.Execute(null);
        //    //DependencyService.Get<IAudio>().PlayAudioFile("VD-LY-AUD-EN-SC-01.mp3");
        //}
    }
}