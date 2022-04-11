using Swegrant.Resources;
using Swegrant.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swegrant.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        MainViewModel vm;
        MainViewModel VM
        {
            get => vm ?? (vm = (MainViewModel)BindingContext);
        }
        public MainPage()
        {
            InitializeComponent();
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, Helpers.Settings.IsUserAdmin);

            VM.InitilizeImages();
            VM.ConnectCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //VM.Dispose();
            //this.BindingContext = null;
        }

        //~MainPage()
        //{
        //    OnDisappearing();
        //}

        private async void btnPersian_Clicked(object sender, EventArgs e)
        {
            Helpers.LanguageHelper.ChangeLanguage(Shared.Models.Language.Farsi);
            bool answer = await DisplayAlert(AppResources.Information,
              AppResources.ConfirmLanguage,
              AppResources.Yes,
              AppResources.No);
            if (answer)
            {
                Helpers.Settings.CurrentLanguage = Shared.Models.Language.Farsi;
                
                await Helpers.ServerHelper.SubmitStatusAsync(Shared.Models.UserEvent.AppLanguageSelected, Helpers.Settings.CurrentLanguage.ToString());
                await Shell.Current.GoToAsync($"//{nameof(CatalogPage)}");
            }
        }

        private async void btnSweden_Clicked(object sender, EventArgs e)
        {
            Helpers.LanguageHelper.ChangeLanguage(Shared.Models.Language.Svenska);
            bool answer = await DisplayAlert(AppResources.Information, 
                AppResources.ConfirmLanguage, 
                AppResources.Yes, 
                AppResources.No);
            if (answer)
            {   
                Helpers.Settings.CurrentLanguage = Shared.Models.Language.Svenska;
                
                await Helpers.ServerHelper.SubmitStatusAsync(Shared.Models.UserEvent.AppLanguageSelected, Helpers.Settings.CurrentLanguage.ToString());
                await Shell.Current.GoToAsync($"//{nameof(CatalogPage)}");
            }
        }

        private async void btnEnglish_Clicked(object sender, EventArgs e)
        {
            Helpers.LanguageHelper.ChangeLanguage(Shared.Models.Language.None);
            bool answer = await DisplayAlert(AppResources.Information,
              AppResources.ConfirmLanguage,
              AppResources.Yes,
              AppResources.No);
            if (answer)
            {
                Helpers.Settings.CurrentLanguage = Shared.Models.Language.None;
                await Helpers.ServerHelper.SubmitStatusAsync(Shared.Models.UserEvent.AppLanguageSelected, Helpers.Settings.CurrentLanguage.ToString());
                await Shell.Current.GoToAsync($"//{nameof(CatalogPage)}");
            }
        }
    }
}