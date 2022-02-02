using MvvmHelpers;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using Swegrant.Models;
using Swegrant.Helpers;
using Newtonsoft.Json;
using Swegrant.Interfaces;
using System.Collections.Generic;
using System.Threading;
using static Swegrant.Models.MediaInfo;
using System.IO;

namespace Swegrant.ViewModels
{
    public class VideoViewModel : BaseViewModel
    {
        public List<Subtitle> CurrentSub;
        public ChatMessage ChatMessage { get; }

        public ObservableCollection<ChatMessage> Messages { get; }
        public ObservableCollection<User> Users { get; }

        public ChatMessage LastReceivedMessage
        {
            get
            {
                return Messages.FirstOrDefault();
            }
        }

        public Language CurrnetLanguage { get; set; }

        public Charachter CurrentCharchter { get; set; }

        public int CurrentScene { get; set; }

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


        public MvvmHelpers.Commands.Command SendMessageCommand { get; }
        public MvvmHelpers.Commands.Command ConnectCommand { get; }
        public MvvmHelpers.Commands.Command DisconnectCommand { get; }

        public MvvmHelpers.Commands.Command ChangeAudioCommand { get; }

        private Task currentSubTask;
        private CancellationTokenSource currentSubCancelationSource;
        private CancellationToken currentSubCancellationToken;
        private int currentSubIndex;

        Random random;
        public VideoViewModel()
        {
            this.CurrnetLanguage = Language.Farsi;
            this.CurrentCharchter = Charachter.Leyla;
            this.CurrentScene = 1;

            if (DesignMode.IsDesignModeEnabled)
                return;

            Title = Settings.Group;

            ChatMessage = new ChatMessage();
            Messages = new ObservableCollection<ChatMessage>();
            Users = new ObservableCollection<User>();
            SendMessageCommand = new MvvmHelpers.Commands.Command(async () => await SendMessage());
            ConnectCommand = new MvvmHelpers.Commands.Command(async () => await Connect());
            DisconnectCommand = new MvvmHelpers.Commands.Command(async () => await Disconnect());
            ChangeAudioCommand = new MvvmHelpers.Commands.Command(async () => await PlayAudio())
; random = new Random();

            ChatService.Init(Settings.ServerIP, Settings.UseHttps);

            ChatService.OnReceivedMessage += (sender, args) =>
            {
                SendLocalMessage(args.Message, args.User);
                AddRemoveUser(args.User, true);
            };

            ChatService.OnEnteredOrExited += (sender, args) =>
            {
                AddRemoveUser(args.User, args.Message.Contains("entered"));
            };

            ChatService.OnConnectionClosed += (sender, args) =>
            {
                SendLocalMessage(args.Message, args.User);
            };


        }


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

                AddRemoveUser(Settings.UserName, true);
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
            SendLocalMessage("Disconnected...", Settings.UserName);
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
                    ChatMessage.Message);

                ChatMessage.Message = string.Empty;
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
                    var first = Users.FirstOrDefault(u => u.Name == user);

                    Messages.Clear();
                    if (message.StartsWith("{"))
                    {
                        ServiceMessage serviceMessage = JsonConvert.DeserializeObject<ServiceMessage>(message);
                        if (serviceMessage != null)
                        {
                            switch (serviceMessage.Command)
                            {
                                case Models.Command.Play:
                                    PlayAudio();
                                    BeginPlaySub();
                                    break;
                                case Models.Command.Prepare:
                                    BeginPrepareAudio();
                                    PrepareSubtitle();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Messages.Insert(0, new ChatMessage
                        {
                            Message = message,
                            User = user,
                            Color = first?.Color ?? Color.FromRgba(0, 0, 0, 0)
                        });
                    }

                });
            }
            catch (Exception ex)
            {

            }
        }

        void AddRemoveUser(string name, bool add)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            if (add)
            {
                if (!Users.Any(u => u.Name == name))
                {
                    var color = Messages.FirstOrDefault(m => m.User == name)?.Color ?? Color.FromRgba(0, 0, 0, 0);
                    Device.BeginInvokeOnMainThread(() =>
                        {
                            Users.Add(new User { Name = name, Color = color });
                        });
                }
            }
            else
            {
                var user = Users.FirstOrDefault(u => u.Name == name);
                if (user != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Users.Remove(user);
                    });
                }
            }
        }

        private async Task PlayAudio()
        {
            DependencyService.Get<IAudio>().PlayAudioFile(CurrnetLanguage);
        }


            private async void BeginPrepareAudio()
        {
            await Task.Run(() =>
           {

               PrepareAudio(Language.Farsi);
               PrepareAudio(Language.Swedish);
           });

            Messages.Insert(0, new ChatMessage
            {
                Message = "Audio Files Preapred",
                User = Helpers.Settings.UserName,
                Color = Color.FromRgba(0, 0, 0, 0)
            });
        }

        private async Task PrepareAudio(Language language)
        {
            string filename = "VD-";

            switch (CurrentCharchter)
            {
                case Charachter.Leyla:
                    filename += "LY-";
                    break;
                case Charachter.Sina:
                    filename += "SI-";
                    break;
                case Charachter.Tara:
                    filename += "TA-";
                    break;
            }

            filename += "AUD-";

            switch (language)
            {
                case Language.English:
                    filename += "EN-";
                    break;
                case Language.Farsi:
                    filename += "FA-";
                    break;
                case Language.Swedish:
                    filename += "SV-";
                    break;
            }

            filename += "SC-";

            filename += CurrentScene.ToString("00");

            filename += ".mp3";

            DependencyService.Get<IAudio>().PrepareAudioFile(language, filename);
        }


        private void BeginPlaySub()
        {
            this.currentSubCancelationSource = new CancellationTokenSource();
            this.currentSubTask = Task.Run(() =>
            {
                this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                PlaySub();

            }, this.currentSubCancelationSource.Token);
        }

        private void PlaySub()
        {
            if (this.currentSubIndex == 0)
            {
                TimeSpan initial = this.CurrentSub[this.currentSubIndex].StartTime;
                Thread.Sleep(initial);
            }
            for (int i = this.currentSubIndex; i < this.CurrentSub.Count - 1; i++)
            {
                try
                {
                    Messages.Clear();
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancellationToken.ThrowIfCancellationRequested();
                        return;
                    }

                    this.currentSubIndex = i;



                    Messages.Insert(0, new ChatMessage
                    {
                        Message = this.CurrentSub[this.currentSubIndex].Text,
                        User = Helpers.Settings.UserName,
                        Color = Color.FromRgba(0, 0, 0, 0)
                    });



                    Thread.Sleep(CurrentSub[i].Duration);
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancellationToken.ThrowIfCancellationRequested();
                        return;
                    }



                    Messages.Insert(0, new ChatMessage
                    {
                        Message = " ",
                        User = Helpers.Settings.UserName,
                        Color = Color.FromRgba(0, 0, 0, 0)
                    });



                    TimeSpan gap = CurrentSub[i + 1].StartTime - CurrentSub[i].EndTime;
                    Thread.Sleep(gap);
                }
                catch (Exception ex)
                {

                }
            }
        }


        async Task PrepareSubtitle()
        {
            try
            {
                string filename = "VD-";

                switch (CurrentCharchter)
                {
                    case Charachter.Leyla:
                        filename += "LY-";
                        break;
                    case Charachter.Sina:
                        filename += "SI-";
                        break;
                    case Charachter.Tara:
                        filename += "TA-";
                        break;
                }

                filename += "SUB-";

                switch (CurrnetLanguage)
                {
                    case Language.English:
                        filename += "EN-";
                        break;
                    case Language.Farsi:
                        filename += "FA-";
                        break;
                    case Language.Swedish:
                        filename += "SV-";
                        break;
                }

                filename += "SC-";

                filename += CurrentScene.ToString("00");

                filename += ".txt";

                string subtitleContent = ReadSubtitleFile(filename);
                PopulateSubtitle(subtitleContent);
              
            }
            catch (Exception ex)
            {

            }
        }

        private void PopulateSubtitle(string text)
        {
            this.CurrentSub = new List<Subtitle>();
            List<string> list = text.Split(new string[] { "\r\n" + "\r\n" },
                               StringSplitOptions.RemoveEmptyEntries).ToList();
            this.CurrentSub = new List<Subtitle>();

            //List<string> subLine = new List<string>();
            foreach (var item in list)
            {
                Subtitle sub = new Subtitle();
                string[] parts = item.Split(new string[] { Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToArray();
                sub.Id = Convert.ToInt32(parts[0]);
                string[] times = parts[1].Split(new string[] { "-->" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                sub.StartTime = TimeSpan.Parse(times[0].Replace(',', '.').Trim());
                sub.EndTime = TimeSpan.Parse(times[1].Replace(',', '.').Trim());
                string line = "";
                for (int i = 2; i < parts.Length; i++)
                {
                    line += parts[i];
                }
                sub.Text = line;
                this.CurrentSub.Add(sub);
            }
        }

        private string ReadSubtitleFile(string filename)
        {
            string content = "";
            try
            {
                string subtitleDirectoty = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DownloadCategory.VDSUB.ToString());
                string fileName = Path.Combine(subtitleDirectoty, filename);
                content = File.ReadAllText(fileName);
            }
            catch (Exception ex)
            {

            }
            return content;

        }
    }
}
