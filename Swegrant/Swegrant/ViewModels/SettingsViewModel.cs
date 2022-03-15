using MvvmHelpers.Commands;
using Swegrant.Shared.Models;
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

        #region Charchter
        bool isLeylaSelected = false;
        public bool IsLeylaSelected
        {
            get { return isLeylaSelected; }
            set
            {
                SetProperty(ref isLeylaSelected, value);
                if (value)
                {
                    CurrentCharchter = Character.Lyla;
                }
            }
        }

        bool isSinaSelected = false;
        public bool IsSinaSelected
        {
            get { return isSinaSelected; }
            set
            {
                SetProperty(ref isSinaSelected, value);
                if (value)
                {
                    CurrentCharchter = Character.Sina;
                }
            }
        }

        bool isTaraSelected = false;
        public bool IsTaraSelected
        {
            get { return isTaraSelected; }
            set
            {
                SetProperty(ref isTaraSelected, value);
                if (value)
                {
                    CurrentCharchter = Character.Tara;
                }
            }
        }

        public Character CurrentCharchter { get; private set; }
        #endregion

        private void SaveSettings()
        {
            Helpers.Settings.ServerIP = this.ServerIP;
            Helpers.Settings.ServerPort = this.ServerPort;
            Helpers.Settings.CurrentCharachter = this.CurrentCharchter;
            NavigatFile();
        }

        private async void NavigatFile()
        {
            await Shell.Current.GoToAsync($"//{nameof(FilePage)}");
        }


    }
}
