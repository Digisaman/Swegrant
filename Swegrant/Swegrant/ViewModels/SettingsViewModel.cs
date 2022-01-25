using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swegrant.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public Command SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            SaveSettingsCommand = new Command(() => SaveSettings());
            serverIP = Helpers.Settings.ServerIP;
        }


        string serverIP = "";
        public string ServerIP
        {
            get { return this.serverIP; }
            set { SetProperty(ref this.serverIP, value); }
        }

        private void SaveSettings()
        {
            Helpers.Settings.ServerIP = this.ServerIP;
        }

        
    }
}
