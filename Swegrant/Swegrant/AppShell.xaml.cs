using Swegrant.ViewModels;
using Swegrant.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Swegrant
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        //public bool IsNavbarVisible { get; set; }
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            //Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Helpers.Settings.ResetSettings();
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        public void SetFlyoutBehavior(FlyoutBehavior flyoutBehavior)
        {
            //IsNavbarVisible = (flyoutBehavior == FlyoutBehavior.Flyout);
            //Shell.SetNavBarIsVisible(this, isNavBarVisibile);
            //isNavBarVisibile = isNavBarVisibile;
            Shell.SetFlyoutBehavior(this, flyoutBehavior);
        }


    }
}
