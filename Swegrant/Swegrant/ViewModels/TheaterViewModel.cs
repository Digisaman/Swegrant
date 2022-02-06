using MvvmHelpers;
using Newtonsoft.Json;
using Swegrant.Helpers;
using Swegrant.Models;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Swegrant.ViewModels
{
    public class TheaterViewModel : BaseViewModel
    {

        public ChatMessage ChatMessage { get; }

        public ObservableCollection<ChatMessage> Messages { get; }



        public ObservableCollection<User> Users { get; }

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

        Random random;

        #region Charchter
        bool ischarchterVisible = false;
        public bool IsCharchterVisible
        {
            get { return ischarchterVisible; }
            set { SetProperty(ref ischarchterVisible, value); }
        }


        string selectedCharchter = "";
        public string SelectedCharchter
        {
            get { return selectedCharchter; }
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    CurrentCharchter = (Character)Enum.Parse(typeof(Character), value);
                    SetProperty(ref selectedCharchter, value); 
                }
            }
        }
        public string[] Charcters
        {
            get
            {
                return new string[]
                {
                    Character.Leyla.ToString(),
                    Character.Tara.ToString(),
                    Character.Sina.ToString()
                };
            }
        }
        #endregion

        #region Subtitle
        public Dictionary<Language, Subtitle[]> MultiSub;

        public Subtitle[] CurrentSub
        {
            get
            {
                if (MultiSub.ContainsKey(CurrnetLanguage))
                {
                    return MultiSub[CurrnetLanguage];
                }
                return null;
            }
        }

        public Language CurrnetLanguage { get; set; }

        private Character _CurnetCharcter;
        public Character CurrentCharchter 
        { 
            get
            {
                _CurnetCharcter = Helpers.Settings.CurrentCharachter;
                return _CurnetCharcter;
            }
            set
            {
                _CurnetCharcter = value;
                Helpers.Settings.CurrentCharachter = _CurnetCharcter;
            }
        }

        public int CurrentScene { get; set; }

        private Task currentSubTask;
        private CancellationTokenSource currentSubCancelationSource;
        private CancellationToken currentSubCancellationToken;
        private int currentSubIndex;



        bool isSubtitleVisible = false;
        public bool IsSubtitleVisible
        {
            get { return isSubtitleVisible; }
            set { SetProperty(ref isSubtitleVisible, value); }
        }
        #endregion

        public TheaterViewModel()
        {
            this.IsSubtitleVisible = true;
            this.CurrnetLanguage = Helpers.Settings.CurrentLanguage;
            this.CurrentCharchter = Helpers.Settings.CurrentCharachter;
            this.CurrentScene = 1;
            this.MultiSub = new Dictionary<Language, Subtitle[]>();

            if (DesignMode.IsDesignModeEnabled)
                return;

            Title = Settings.Group;

            ChatMessage = new ChatMessage();
            Messages = new ObservableCollection<ChatMessage>();
            Users = new ObservableCollection<User>();
            SendMessageCommand = new MvvmHelpers.Commands.Command(async () => await SendMessage());
            ConnectCommand = new MvvmHelpers.Commands.Command(async () => await Connect());
            DisconnectCommand = new MvvmHelpers.Commands.Command(async () => await Disconnect());
            random = new Random();

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
            Device.BeginInvokeOnMainThread(() =>
            {
                var first = Users.FirstOrDefault(u => u.Name == user);


                if (message.StartsWith("{"))
                {
                    ServiceMessage serviceMessage = JsonConvert.DeserializeObject<ServiceMessage>(message);
                    if (serviceMessage != null)
                    {
                        switch (serviceMessage.Command)
                        {
                            case Swegrant.Shared.Models.Command.Prepare:
                                Task.Run(() => PrepareSubtitle());
                                break;
                            case Swegrant.Shared.Models.Command.Play:
                            case Swegrant.Shared.Models.Command.ResumeAutoSub:
                                ResumeAutoSub();
                                break;
                            case Swegrant.Shared.Models.Command.ShowSubtitle:
                                Task.Run(() => ShowSubtitle());
                                break;
                            case Swegrant.Shared.Models.Command.HideSubtitle:
                                Task.Run(() => HideSubtitle());
                                break;
                            case Swegrant.Shared.Models.Command.PauseAutoSub:
                                Task.Run(() => PauseAutoSub());
                                break;
                            case Swegrant.Shared.Models.Command.DisplayManualSub:
                                Task.Run(() => DisplayManualSub(serviceMessage.Index));
                                break;
                            case Swegrant.Shared.Models.Command.ShowSelectCharacter:
                                Task.Run(() => SelectCharchter(true));
                                break;

                            case Swegrant.Shared.Models.Command.HideSelectCharchter:
                                Task.Run(() => SelectCharchter(false));
                                break;
                        }
                    }
                }
                else
                {
                    Messages.Clear();
                    Messages.Insert(0, new ChatMessage
                    {
                        Message = message,
                        User = user,
                        Color = first?.Color ?? Color.FromRgba(0, 0, 0, 0)
                    });
                }
            });
        }

        private void SelectCharchter(bool isVisible)
        {
            IsCharchterVisible = isVisible;
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

        private void BeginPauseAutoSub()
        {

        }

        private void ResumeAutoSub()
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
            for (int i = this.currentSubIndex; i < this.CurrentSub.Length - 1; i++)
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

                    Messages.Clear();

                    TimeSpan gap = CurrentSub[i + 1].StartTime - CurrentSub[i].EndTime;
                    Thread.Sleep(gap);
                }
                catch (Exception ex)
                {

                }
            }
        }


        private void PrepareSubtitle()
        {
            try
            {


                //filename += "SUB-";

                //switch (CurrnetLanguage)
                //{
                //    case Language.English:
                //        filename += "EN-";
                //        break;
                //    case Language.Farsi:
                //        filename += "FA-";
                //        break;
                //    case Language.Swedish:
                //        filename += "SV-";
                //        break;
                //}

                //filename += "SC-";

                //filename += CurrentScene.ToString("00");

                //filename += ".txt";
                string subtitleContent = "";

                if (!MultiSub.ContainsKey(Language.Farsi))
                {
                    string filename = $"TH-SUB-FA-SC-{CurrentScene.ToString("00")}.txt";
                    subtitleContent = Helpers.SubtitleHelper.ReadSubtitleFile(Shared.Models.Mode.Theater, filename);
                    MultiSub.Add(Language.Farsi, Helpers.SubtitleHelper.PopulateSubtitle(subtitleContent));
                }

                if (!MultiSub.ContainsKey(Language.Swedish))
                {
                    string filename = $"TH-SUB-SV-SC-{CurrentScene.ToString("00")}.txt";
                    subtitleContent = Helpers.SubtitleHelper.ReadSubtitleFile(Shared.Models.Mode.Theater, filename);
                    MultiSub.Add(Language.Swedish, Helpers.SubtitleHelper.PopulateSubtitle(subtitleContent));
                }
                Messages.Clear();

                Messages.Insert(0, new ChatMessage
                {
                    Message = "Subtitles Loaded",
                    User = Helpers.Settings.UserName,
                    Color = Color.FromRgba(0, 0, 0, 0)
                });

            }
            catch (Exception ex)
            {

            }
        }

        private void PauseAutoSub()
        {
            try
            {
                if (this.currentSubTask != null && this.currentSubTask.Status == TaskStatus.Running)
                {
                    this.currentSubCancelationSource.Cancel();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void HideSubtitle()
        {
            try
            {
                IsSubtitleVisible = false;
            }
            catch (Exception ex)
            {

            }
        }


        private void ShowSubtitle()
        {
            try
            {
                IsSubtitleVisible = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void DisplayManualSub(int index)
        {
            try
            {
                currentSubIndex = index;
                Messages.Clear();

                Messages.Insert(0, new ChatMessage
                {
                    Message = this.CurrentSub[this.currentSubIndex].Text,
                    User = Helpers.Settings.UserName,
                    Color = Color.FromRgba(0, 0, 0, 0)
                });
            }
            catch (Exception ex)
            {

            }
        }

    }
}
