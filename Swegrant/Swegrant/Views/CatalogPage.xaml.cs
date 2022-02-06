using Swegrant.Helpers;
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
            this.webBrowser.Source = $"http://{Settings.ServerIP}:{Settings.ServerPort}/index.html";
            this.webBrowser.Reload();
            //this.webBrowser.Reload();


        }

        private void btnNext_Clicked(object sender, EventArgs e)
        {
            NavigateTheater();
        }
        private async void NavigateTheater()
        {
            await Shell.Current.GoToAsync($"//{nameof(TheaterPage)}");
        }

    }
}