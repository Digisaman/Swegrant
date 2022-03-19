using Swegrant.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Swegrant.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly string adminPassRef = "swegrant2022";
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            isAdmin = Helpers.Settings.IsUserAdmin;
            IsNormal = !Helpers.Settings.IsUserAdmin;
        }

        private async void OnLoginClicked(object obj)
        {
            try
            {
                // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
                if (IsAdmin && (AdminPass == adminPassRef))
                {
                    ((AppShell)App.Current.MainPage).SetFlyoutBehavior(FlyoutBehavior.Flyout);
                    AdminPass = "";
                    Helpers.Settings.IsUserAdmin = true;
                    await Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
                    
                }
                else if (IsNormal)
                {
                    ((AppShell)App.Current.MainPage).SetFlyoutBehavior(FlyoutBehavior.Disabled);
                    Helpers.Settings.IsUserAdmin = false;
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool isAdmin;
        public bool IsAdmin
        {
            get => isAdmin;
            set => SetProperty(ref isAdmin, value);
        }

        private bool isNormal;
        public bool IsNormal
        {
            get => isNormal;
            set => SetProperty(ref isNormal, value);
        }

        private string adminPass;
        public string AdminPass
        {
            get => adminPass;
            set => SetProperty(ref adminPass, value);
        }
    }
}
