using Newtonsoft.Json;
using Swegrant.Helpers;
using Swegrant.Resources;
using Swegrant.Shared.Models;
using Swegrant.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Swegrant.ViewModels
{
    public class CatalogViewModel : BaseViewModel
    {
        public CatalogViewModel()
        {
            Title = Resources.MenuTitles.Catalog;

            SendMessageCommand = new MvvmHelpers.Commands.Command(async () => await SendMessage());
            ConnectCommand = new MvvmHelpers.Commands.Command(async () => await Connect());
            DisconnectCommand = new MvvmHelpers.Commands.Command(async () => await Disconnect());


            ChatService.Init(Settings.ServerIP, Settings.UseHttps);

            ChatService.OnReceivedMessage += (sender, args) =>
            {
                SendLocalMessage(args.Message, args.User);
            };

            ChatService.OnConnectionClosed += (sender, args) =>
            {
                SendLocalMessage(args.Message, args.User);
            };
        }

        #region Properties
        bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetProperty(ref isConnected, value);
                });
            }
        }
        #endregion

        #region Commands
        public MvvmHelpers.Commands.Command SendMessageCommand { get; }
        public MvvmHelpers.Commands.Command ConnectCommand { get; }
        public MvvmHelpers.Commands.Command DisconnectCommand { get; }
        #endregion

        #region Methods
        async Task Connect()
        {
            if (IsConnected)
                return;
            try
            {
                IsBusy = true;
                await ChatService.ConnectAsync();
                await ChatService.JoinChannelAsync(Settings.Group, Settings.UserName);
                IsConnected = true;
                await Task.Delay(500);
                SendLocalMessage("Connected...", Settings.UserName);
            }
            catch (Exception ex)
            {
                SendLocalMessage($"Connection error: {ex.Message}", Settings.UserName);
            }
            finally
            {
                IsBusy = false;
            }
        }


        async Task Disconnect()
        {
            if (!IsConnected)
                return;
            await ChatService.LeaveChannelAsync(Settings.Group, Settings.UserName);
            await ChatService.DisconnectAsync();
            IsConnected = false;
        }

        async Task SendMessage()
        {
            if (!IsConnected)
            {
                await DialogService.DisplayAlert("Not connected", "Please connect to the server and try again.", "OK");
                return;
            }
            try
            {
                IsBusy = true;
                await ChatService.SendMessageAsync(Settings.Group,
                    Settings.UserName,
                    "");
            }
            catch (Exception ex)
            {
                SendLocalMessage($"Send failed: {ex.Message}", Settings.UserName);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SendLocalMessage(string message, string user)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (message.StartsWith("{"))
                    {
                        ServiceMessage serviceMessage = JsonConvert.DeserializeObject<ServiceMessage>(message);

                        if (serviceMessage.Mode == Shared.Models.Mode.None)
                        {
                            switch (serviceMessage.Command)
                            {

                                case Shared.Models.Command.NavigateTheater:
                                    if ( Helpers.Settings.CurrentLanguage == Language.None)
                                    {
                                        Helpers.Settings.CurrentLanguage = Language.Farsi;
                                    }
                                    Shell.Current.GoToAsync($"//{nameof(TheaterPage)}");
                                    break;
                            }
                        }
                    }

                });
            }
            catch (Exception ex)
            {

            }
        }

        //public void Dispose()
        //{
        //    GC.Collect();
        //    //await Disconnect();
        //}




        #endregion
    }


}
