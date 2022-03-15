using Swegrant.Helpers;
using Swegrant.Shared.Models;
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
    public partial class CatalogPage : ContentPage
    {
        public CatalogPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Language CurrnetLanguage = Helpers.Settings.CurrentLanguage;
            if ( CurrnetLanguage == Language.None)
            {
                CurrnetLanguage = Language.Farsi;
            }
            this.webBrowser.Source = $"http://{Settings.ServerIP}:{Settings.ServerPort}/{CurrnetLanguage.ToString().ToUpper().Substring(0,2)}/index.htm";
            this.webBrowser.Reload();
            //this.webBrowser.Reload();


        }

        private void btnNext_Clicked(object sender, EventArgs e)
        {
            NavigateTheater();
        }
        private void btnPrevious_Clicked(object sender, EventArgs e)
        {
            NavigateMain();
        }

        private async void NavigateTheater()
        {
            await Shell.Current.GoToAsync($"//{nameof(TheaterPage)}");
        }

        private async void NavigateMain()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

        private void webBrowser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            this.progressBar.IsVisible = false;
        }

        private void webBrowser_Navigating(object sender, WebNavigatingEventArgs e)
        {
            this.progressBar.IsVisible = true;
        }

      
    }
}