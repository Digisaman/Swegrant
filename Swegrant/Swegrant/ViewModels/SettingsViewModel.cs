using MvvmHelpers.Commands;
using Swegrant.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Swegrant.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public MvvmHelpers.Commands.Command SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            SaveSettingsCommand = new MvvmHelpers.Commands.Command(() => SaveSettings());
            serverIP = Helpers.Settings.ServerIP;
            serverPort = Helpers.Settings.ServerPort;
        }


        string serverIP = "";
        public string ServerIP
        {
            get { return this.serverIP; }
            set { SetProperty(ref this.serverIP, value); }
        }

        string serverPort = "";
        public string ServerPort
        {
            get { return this.serverPort; }
            set { SetProperty(ref this.serverPort, value); }
        }

        private void SaveSettings()
        {
            Helpers.Settings.ServerIP = this.ServerIP;
            Helpers.Settings.ServerPort = this.ServerPort;
            NavigatFile();
        }

        private async void NavigatFile()
        {
            await Shell.Current.GoToAsync($"//{nameof(FilePage)}");
        }


    }
}
