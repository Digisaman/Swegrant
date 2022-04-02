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
using System.IO;
using Swegrant.Shared.Models;
using Swegrant.Views;

namespace Swegrant.ViewModels
{
    public class VideoViewModel : BaseViewModel
    {
        public Subtitle[] CurrentSub;
        public MediaMessage MediaMessage { get; }

        //public ObservableCollection<ChatMessage> Messages { get; }
        public ObservableCollection<User> Users { get; }

       

        
        public Language CurrnetAudioLanguage
        {
            get
            {
                return Helpers.Settings.CurrentAudioLanguage;
            }
            set
            {
                Helpers.Settings.CurrentAudioLanguage = value;                
            }
        }

        public Language CurrnetSubtitleLanguage
        {
            get
            {
                return Helpers.Settings.CurrentLanguage;
            }
            set
            {
                Helpers.Settings.CurrentLanguage = value;
            }
        }


        public Character CurrentCharchter { get; set; }

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


        bool isLangugeVisible;
        public bool IsLangugeVisible
        {
            get => isLangugeVisible;
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetProperty(ref isLangugeVisible, value);
                });
            }
        }

        bool isHeadphonesVisible = false;
        public bool IsHeadphonesVisible
        {
            get { return isHeadphonesVisible; }
            set
            {
                SetProperty(ref isHeadphonesVisible, value);
            }
        }


      

        #region Language
        bool isAudioSV = false;
        public bool IsAudioSV
        {
            get { return isAudioSV; }
            set
            {
                SetProperty(ref isAudioSV, value);
                if (value)
                {
                    CurrnetAudioLanguage = Language.Svenska;
                    Helpers.ServerHelper.SubmitStatus(Shared.Models.UserEvent.AudioLanguageSelected, CurrnetAudioLanguage.ToString());
                }
            }
        }

        bool isAudioFA = false;
        public bool IsAudioFA
        {
            get { return isAudioFA; }
            set
            {
                SetProperty(ref isAudioFA, value);
                if (value)
                {
                    CurrnetAudioLanguage = Language.Farsi;
                    Helpers.ServerHelper.SubmitStatus(Shared.Models.UserEvent.AudioLanguageSelected, CurrnetAudioLanguage.ToString());
                }
            }
        }

        bool isAudioOR = false;
        public bool IsAudioOR
        {
            get { return isAudioOR; }
            set
            {
                SetProperty(ref isAudioOR, value);
                if (value)
                {
                    CurrnetAudioLanguage = Language.Original;
                    Helpers.ServerHelper.SubmitStatus(Shared.Models.UserEvent.AudioLanguageSelected, CurrnetAudioLanguage.ToString());
                }
            }
        }


        bool isSubSV = false;
        public bool IsSubSV
        {
            get { return isSubSV; }
            set
            {
                SetProperty(ref isSubSV, value);
                if (value)
                {
                    CurrnetSubtitleLanguage = Language.Svenska;
                    Helpers.ServerHelper.SubmitStatus(Shared.Models.UserEvent.SubtitleLanguageSelected, CurrnetSubtitleLanguage.ToString());
                }
            }
        }

        bool isSubFA = false;
        public bool IsSubFA
        {
            get { return isSubFA; }
            set
            {
                SetProperty(ref isSubFA, value);
                if (value)
                {
                    CurrnetSubtitleLanguage = Language.Farsi;
                    Helpers.ServerHelper.SubmitStatus(Shared.Models.UserEvent.SubtitleLanguageSelected, CurrnetSubtitleLanguage.ToString());
                }
            }
        }

        #endregion

        #region Commands
        public MvvmHelpers.Commands.Command SendMessageCommand { get; }
        public MvvmHelpers.Commands.Command ConnectCommand { get; }
        public MvvmHelpers.Commands.Command DisconnectCommand { get; }

        public MvvmHelpers.Commands.Command ChangeAudioCommand { get; }
        #endregion
        private Task currentSubTask;
        private CancellationTokenSource currentSubCancelationSource;
        private CancellationToken currentSubCancellationToken;
        private int currentSubIndex;

        Random random;
        public VideoViewModel()
        {
            Title = Resources.MenuTitles.About;
            InitilizeSettings();

            if (DesignMode.IsDesignModeEnabled)
                return;

           

            MediaMessage = new MediaMessage();
            //Messages = new ObservableCollection<ChatMessage>();
            Users = new ObservableCollection<User>();
            SendMessageCommand = new MvvmHelpers.Commands.Command(async () => await SendMessage());
            ConnectCommand = new MvvmHelpers.Commands.Command(async () => await Connect());
            DisconnectCommand = new MvvmHelpers.Commands.Command(async () => await Disconnect());
            ChangeAudioCommand = new MvvmHelpers.Commands.Command(async () => await PlayAudio());
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

        private async void InitilizeSettings(int currentScene = 1)
        {
            this.CurrentScene = currentScene;
            if (Helpers.Settings.CurrentLanguage != Language.None)
            {
                this.CurrnetSubtitleLanguage = Helpers.Settings.CurrentLanguage;
            }
            else
            {  
                this.CurrnetSubtitleLanguage = Language.Farsi;
            }

            if (Helpers.Settings.CurrentAudioLanguage != Language.None)
            {
                this.CurrnetAudioLanguage = Helpers.Settings.CurrentAudioLanguage;
            }
            else
            {
                this.CurrnetAudioLanguage = Language.Original;
            }

            if (Helpers.Settings.CurrentCharachter != Character.None)
            {
                this.CurrentCharchter = Helpers.Settings.CurrentCharachter;
            }
            else
            {
                this.CurrentCharchter = await ServerHelper.AutoAssignCharacterAsync();
                Helpers.Settings.CurrentCharachter = this.CurrentCharchter;
            }
            
            

            isAudioOR = (this.CurrnetAudioLanguage == Language.Original);
            isAudioFA = (this.CurrnetAudioLanguage == Language.Farsi);
            isAudioSV = (this.CurrnetAudioLanguage == Language.Svenska);

            isSubFA = (this.CurrnetSubtitleLanguage == Language.Farsi);
            isSubSV = (this.CurrnetSubtitleLanguage == Language.Svenska);

            IsLangugeVisible = true;
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
                    MediaMessage.Message);

                MediaMessage.Message = string.Empty;
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

                    
                    MediaMessage.Clear();

                    if (message.StartsWith("{"))
                    {
                        ServiceMessage serviceMessage = JsonConvert.DeserializeObject<ServiceMessage>(message);


                        if (serviceMessage.Mode == Shared.Models.Mode.Video ||
                        (serviceMessage.Mode == Shared.Models.Mode.Theater && serviceMessage.Command == Shared.Models.Command.ChangeMode))
                        {
                            this.CurrentScene = serviceMessage.Scene;
                            switch (serviceMessage.Command)
                            {
                                case Shared.Models.Command.ChangeMode:
                                    if (serviceMessage.Mode == Shared.Models.Mode.Theater)
                                    {
                                        StopAudio();
                                        Shell.Current.GoToAsync($"//{nameof(TheaterPage)}");
                                    }
                                    else
                                    {
                                        InitilizeSettings(serviceMessage.Scene);
                                        //Messages.Clear();
                                        //Messages.Insert(0, new ChatMessage
                                        //{
                                        //    Message = "Put on your Headphones.",
                                        //    User = user,
                                        //    Color = first?.Color ?? Color.FromRgba(0, 0, 0, 0)
                                        //});
                                        MediaMessage.Message = Resources.AppResources.PutonHeadphones;
                                        this.IsHeadphonesVisible = true;
                                    }
                                    break;
                                case Swegrant.Shared.Models.Command.Play:
                                    this.currentSubIndex = 0;
                                    PlayAudio();
                                    BeginPlaySub();
                                    break;
                                case Swegrant.Shared.Models.Command.Prepare:
                                    BeginPrepareAudio();
                                    PrepareSubtitle();
                                    break;
                            
                            }
                        }
                        else if ( serviceMessage.Mode == Mode.None && serviceMessage.Command == Shared.Models.Command.NavigateQuestionnaire)
                        {
                            Shell.Current.GoToAsync($"//{nameof(QuestionnairePage)}");
                        }

                    }
                    else
                    {
                        //Messages.Insert(0, new ChatMessage
                        //{
                        //    Message = message,
                        //    User = user,
                        //    Color = first?.Color ?? Color.FromRgba(0, 0, 0, 0)
                        //});
                        MediaMessage.Message = message;
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
                    //var color = Messages.FirstOrDefault(m => m.User == name)?.Color ?? Color.FromRgba(0, 0, 0, 0);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Users.Add(new User { Name = name });
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
            Task.Run(() =>
            {
                DependencyService.Get<IAudio>().PlayAudioFile(CurrnetAudioLanguage);
            });

        }

        private async Task StopAudio()
        {
            Task.Run(() =>
            {
                DependencyService.Get<IAudio>().StopAudioFile(CurrnetAudioLanguage);
            });
        }


        private async void BeginPrepareAudio()
        {
            await Task.Run(() =>
            {
                PrepareAudio(Language.Original);
                PrepareAudio(Language.Farsi);
                PrepareAudio(Language.Svenska);
            });

            //Messages.Insert(0, new ChatMessage
            //{
            //    Message = "Audio Files Preapred",
            //    User = Helpers.Settings.UserName,
            //    Color = Color.FromRgba(0, 0, 0, 0)
            //});
            MediaMessage.Message = "Audio Files Preapred";
            IsLangugeVisible = false;
        }

        private async Task PrepareAudio(Language language)
        {
            string filename = $"VD-{CurrentCharchter.ToString().Substring(0,2).ToUpper()}-AUD-{language.ToString().Substring(0,2).ToUpper()}-SC-{CurrentScene.ToString("00")}.mp3";

            //switch (CurrentCharchter)
            //{
            //    case Character.Lyla:
            //        filename += "LY-";
            //        break;
            //    case Character.Sina:
            //        filename += "SI-";
            //        break;
            //    case Character.Tara:
            //        filename += "TA-";
            //        break;
            //}

            //filename += "AUD-";

            //switch (language)
            //{
            //    case Language.English:
            //        filename += "OR-";
            //        break;
            //    case Language.Farsi:
            //        filename += "FA-";
            //        break;
            //    case Language.Svenska:
            //        filename += "SV-";
            //        break;
            //}

            //filename += "SC-";

            //filename += CurrentScene.ToString("00");

            //filename += ".mp3";

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
            for (int i = this.currentSubIndex; i < this.CurrentSub.Length; i++)
            {
                try
                {
                    MediaMessage.Clear();
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancellationToken.ThrowIfCancellationRequested();
                        return;
                    }

                    this.currentSubIndex = i;



                    //Messages.Insert(0, new ChatMessage
                    //{
                    //    Message = this.CurrentSub[this.currentSubIndex].Text,
                    //    User = Helpers.Settings.UserName,
                    //    Color = Color.FromRgba(0, 0, 0, 0)
                    //});
                    MediaMessage.Message = this.CurrentSub[this.currentSubIndex].Text;


                    Thread.Sleep(CurrentSub[i].Duration);
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancellationToken.ThrowIfCancellationRequested();
                        return;
                    }



                    //Messages.Insert(0, new ChatMessage
                    //{
                    //    Message = " ",
                    //    User = Helpers.Settings.UserName,
                    //    Color = Color.FromRgba(0, 0, 0, 0)
                    //});
                    MediaMessage.Clear();


                    if (this.currentSubIndex < CurrentSub.Length - 1)
                    {
                        TimeSpan gap = CurrentSub[i + 1].StartTime - CurrentSub[i].EndTime;
                        Thread.Sleep(gap);
                    }
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
                    case Character.Lyla:
                        filename += "LY-";
                        break;
                    case Character.Sina:
                        filename += "SI-";
                        break;
                    case Character.Tara:
                        filename += "TA-";
                        break;
                }

                filename += "SUB-";

                switch (CurrnetSubtitleLanguage)
                {
                    case Language.English:
                        filename += "EN-";
                        break;
                    case Language.Farsi:
                        filename += "FA-";
                        break;
                    case Language.Svenska:
                        filename += "SV-";
                        break;
                }

                filename += "SC-";

                filename += CurrentScene.ToString("00");

                filename += ".txt";

                string subtitleContent = Helpers.SubtitleHelper.ReadSubtitleFile(Shared.Models.Mode.Video, filename);
                this.CurrentSub = Helpers.SubtitleHelper.PopulateSubtitle(subtitleContent);
                this.IsHeadphonesVisible = false;
            }
            catch (Exception ex)
            {

            }
        }

        //private void PopulateSubtitle(string text)
        //{
        //    this.CurrentSub = new List<Subtitle>();
        //    List<string> list = text.Split(new string[] { "\r\n" + "\r\n" },
        //                       StringSplitOptions.RemoveEmptyEntries).ToList();
        //    this.CurrentSub = new List<Subtitle>();

        //    //List<string> subLine = new List<string>();
        //    foreach (var item in list)
        //    {
        //        Subtitle sub = new Subtitle();
        //        string[] parts = item.Split(new string[] { Environment.NewLine },
        //                       StringSplitOptions.RemoveEmptyEntries).ToArray();
        //        sub.Id = Convert.ToInt32(parts[0]);
        //        string[] times = parts[1].Split(new string[] { "-->" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        //        sub.StartTime = TimeSpan.Parse(times[0].Replace(',', '.').Trim());
        //        sub.EndTime = TimeSpan.Parse(times[1].Replace(',', '.').Trim());
        //        string line = "";
        //        for (int i = 2; i < parts.Length; i++)
        //        {
        //            line += parts[i];
        //            if ( i < parts.Length-1)
        //            {
        //                line += Environment.NewLine;
        //            }
        //        }
        //        sub.Text = line;
        //        this.CurrentSub.Add(sub);
        //    }
        //}

        //private string ReadSubtitleFile(string filename)
        //{
        //    string content = "";
        //    try
        //    {
        //        string subtitleDirectoty = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DownloadCategory.VDSUB.ToString());
        //        string fileName = Path.Combine(subtitleDirectoty, filename);
        //        content = File.ReadAllText(fileName);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return content;

        //}

        private async void NavigatTheater()
        {
            await Shell.Current.GoToAsync($"//{nameof(TheaterPage)}");
        }
    }
}
