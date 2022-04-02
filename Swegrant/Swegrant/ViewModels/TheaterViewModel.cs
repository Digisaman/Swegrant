using MvvmHelpers;
using Newtonsoft.Json;
using Swegrant.Helpers;
using Swegrant.Models;
using Swegrant.Shared.Models;
using Swegrant.Views;
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

        public MediaMessage MediaMessage { get; }

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
        public MvvmHelpers.Commands.Command SelectLeyla { get; }
        public MvvmHelpers.Commands.Command SelectSina { get; }
        public MvvmHelpers.Commands.Command SelectTara { get; }

        private ImageSource leylaImage;
        public ImageSource LeylaImage
        {
            get { return leylaImage; }
            set { SetProperty(ref leylaImage, value); }
        }

        private ImageSource sinaImage;
        public ImageSource SinaImage
        {
            get { return sinaImage; }
            set { SetProperty(ref sinaImage, value); }
        }

        private ImageSource taraImage;
        public ImageSource TaraImage
        {
            get { return taraImage; }
            set { SetProperty(ref taraImage, value); }
        }

        bool isTextCharchterVisible = false;
        public bool IsTextCharchterVisible
        {
            get { return isTextCharchterVisible; }
            set { SetProperty(ref isTextCharchterVisible, value); }
        }

        bool isImageCharchterVisible = false;
        public bool IsImageCharchterVisible
        {
            get { return isImageCharchterVisible; }
            set { SetProperty(ref isImageCharchterVisible, value); }
        }

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
                    Helpers.ServerHelper.SubmitStatus(UserEvent.CharacterSelected, CurrentCharchter.ToString());
                    LeylaImage = ImageSource.FromResource("Swegrant.Resources.Leyla_Selected.jpg");
                    TaraImage = ImageSource.FromResource("Swegrant.Resources.Tara.jpg");
                    SinaImage = ImageSource.FromResource("Swegrant.Resources.Sina.jpg");
                }
                else
                {
                    LeylaImage = ImageSource.FromResource("Swegrant.Resources.Leyla.jpg");
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
                    Helpers.ServerHelper.SubmitStatus(UserEvent.CharacterSelected, CurrentCharchter.ToString());
                    SinaImage = ImageSource.FromResource("Swegrant.Resources.Sina_Selected.jpg");
                    TaraImage = ImageSource.FromResource("Swegrant.Resources.Tara.jpg");
                    LeylaImage = ImageSource.FromResource("Swegrant.Resources.Leyla.jpg");
                }
                else
                {
                    SinaImage = ImageSource.FromResource("Swegrant.Resources.Sina.jpg");
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
                    Helpers.ServerHelper.SubmitStatus(UserEvent.CharacterSelected, CurrentCharchter.ToString());
                    TaraImage = ImageSource.FromResource("Swegrant.Resources.Tara_Selected.jpg");
                    SinaImage = ImageSource.FromResource("Swegrant.Resources.Sina.jpg");
                    LeylaImage = ImageSource.FromResource("Swegrant.Resources.Leyla.jpg");
                }
                else
                {
                    TaraImage = ImageSource.FromResource("Swegrant.Resources.Tara.jpg");
                }
            }
        }


        //string selectedCharchter = "";
        //public string SelectedCharchter
        //{
        //    get { return selectedCharchter; }
        //    set 
        //    {
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            CurrentCharchter = (Character)Enum.Parse(typeof(Character), value);
        //            SetProperty(ref selectedCharchter, value); 
        //        }
        //    }
        //}
        //public string[] Charcters
        //{
        //    get
        //    {
        //        return new string[]
        //        {
        //            Character.Lyla.ToString(),
        //            Character.Tara.ToString(),
        //            Character.Sina.ToString()
        //        };
        //    }
        //}
        #endregion

        #region Subtitle
        public ImageSource HeadphonesImage { get; set; }

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

        bool isHeadphonesVisible = false;
        public bool IsHeadphonesVisible
        {
            get { return isHeadphonesVisible; }
            set
            {
                SetProperty(ref isHeadphonesVisible, value);
            }
        }


        bool isSubtitleVisible = false;
        public bool IsSubtitleVisible
        {
            get { return isSubtitleVisible; }
            set
            {
                SetProperty(ref isSubtitleVisible, value);
            }
        }
        #endregion

        public TheaterViewModel()
        {
            Title = Resources.MenuTitles.Theater;
            InitializeSettings();

            if (DesignMode.IsDesignModeEnabled)
                return;

            //Title = Settings.Group;

            MediaMessage = new MediaMessage();
            Messages = new ObservableCollection<ChatMessage>();
            Users = new ObservableCollection<User>();
            SendMessageCommand = new MvvmHelpers.Commands.Command(async () => await SendMessage());
            ConnectCommand = new MvvmHelpers.Commands.Command(async () => await Connect());
            DisconnectCommand = new MvvmHelpers.Commands.Command(async () => await Disconnect());

            SelectLeyla = new MvvmHelpers.Commands.Command(async () => await SelectCharchter(Character.Lyla));
            SelectSina = new MvvmHelpers.Commands.Command(async () => await SelectCharchter(Character.Sina));
            SelectTara = new MvvmHelpers.Commands.Command(async () => await SelectCharchter(Character.Tara));
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

            if (isLeylaSelected)
                LeylaImage = ImageSource.FromResource("Swegrant.Resources.Leyla_Selected.jpg");
            else
                LeylaImage = ImageSource.FromResource("Swegrant.Resources.Leyla.jpg");
            if (isSinaSelected)
                SinaImage = ImageSource.FromResource("Swegrant.Resources.Sina_Selected.jpg");
            else
                SinaImage = ImageSource.FromResource("Swegrant.Resources.Sina.jpg");

            if (IsTaraSelected)
                TaraImage = ImageSource.FromResource("Swegrant.Resources.Tara_Selected.jpg");
            else
                TaraImage = ImageSource.FromResource("Swegrant.Resources.Tara.jpg");
            HeadphonesImage = ImageSource.FromResource("Swegrant.Resources.Headphones_Off.gif");
        }

        private void InitializeSettings(int currentScene = 1)
        {
            this.MultiSub = new Dictionary<Language, Subtitle[]>();
            this.IsSubtitleVisible = true;
            this.CurrnetLanguage = Helpers.Settings.CurrentLanguage;
            this.CurrentCharchter = Helpers.Settings.CurrentCharachter;
            isLeylaSelected = (CurrentCharchter == Character.Lyla);
            isSinaSelected = (CurrentCharchter == Character.Sina);
            isTaraSelected = (CurrentCharchter == Character.Tara);

            this.CurrentScene = currentScene;

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
                SendLocalMessage("Connected", Settings.UserName);
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
            Device.BeginInvokeOnMainThread(() =>
            {
                var first = Users.FirstOrDefault(u => u.Name == user);


                if (message.StartsWith("{"))
                {
                    ServiceMessage serviceMessage = JsonConvert.DeserializeObject<ServiceMessage>(message);
                    if (serviceMessage != null)
                    {
                        if (serviceMessage.Mode == Shared.Models.Mode.Theater ||
                        (serviceMessage.Mode == Shared.Models.Mode.Video && serviceMessage.Command == Shared.Models.Command.ChangeMode))
                        {
                            this.CurrentScene = serviceMessage.Scene;
                            switch (serviceMessage.Command)
                            {
                                case Shared.Models.Command.ChangeMode:
                                    if (serviceMessage.Mode == Shared.Models.Mode.Video)
                                    {
                                        Shell.Current.GoToAsync($"//{nameof(VideoPage)}");
                                    }
                                    else
                                    {
                                        InitializeSettings(serviceMessage.Scene);
                                        //Messages.Clear();
                                        //Messages.Insert(0, new ChatMessage
                                        //{
                                        //    Message = "Remove your Headphones.",
                                        //    User = user,
                                        //    Color = first?.Color ?? Color.FromRgba(0, 0, 0, 0)
                                        //});

                                        MediaMessage.Message = Resources.AppResources.RemoveHeadphones;
                                        this.IsHeadphonesVisible = true;
                                    }
                                    break;
                                case Swegrant.Shared.Models.Command.Prepare:
                                    Task.Run(() => PrepareSubtitle());
                                    break;
                                case Swegrant.Shared.Models.Command.Play:
                                    this.currentSubIndex = 0;
                                    ResumeAutoSub();
                                    break;
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
                                case Swegrant.Shared.Models.Command.ShowSelectCharacterText:
                                    Task.Run(() => SelectTextCharchterVisibility(true));
                                    break;
                                case Swegrant.Shared.Models.Command.ShowSelectCharacterImage:
                                    Task.Run(() => SelectImageCharchterVisibility(true));
                                    break;
                                case Swegrant.Shared.Models.Command.HideSelectCharchter:
                                    Task.Run(() =>
                                    {
                                        SelectTextCharchterVisibility(false);
                                        SelectImageCharchterVisibility(false);
                                    });
                                    break;
                                case Swegrant.Shared.Models.Command.NavigateQuestionnaire:
                                    Shell.Current.GoToAsync($"//{nameof(QuestionnairePage)}");
                                    break;
                            }
                        }
                        else if (serviceMessage.Mode == Mode.None && serviceMessage.Command == Shared.Models.Command.NavigateQuestionnaire)
                        {
                            Shell.Current.GoToAsync($"//{nameof(QuestionnairePage)}");
                        }
                    }
                }
                else
                {
                    //Messages.Clear();
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

        private async void NavigateVideo()
        {
            await Shell.Current.GoToAsync($"//{nameof(VideoPage)}");
        }



        private void SelectTextCharchterVisibility(bool isVisible)
        {
            IsTextCharchterVisible = isVisible;
        }

        private void SelectImageCharchterVisibility(bool isVisible)
        {
            IsImageCharchterVisible = isVisible;
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
            for (int i = this.currentSubIndex; i < this.CurrentSub.Length; i++)
            {
                try
                {
                    //Messages.Clear();
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

                    //Messages.Clear();
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
                string filename = $"TH-SUB-FA-SC-{CurrentScene.ToString("00")}.txt";
                subtitleContent = Helpers.SubtitleHelper.ReadSubtitleFile(Shared.Models.Mode.Theater, filename);
                if (!MultiSub.ContainsKey(Language.Farsi))
                {
                    MultiSub.Add(Language.Farsi, Helpers.SubtitleHelper.PopulateSubtitle(subtitleContent));
                }
                else
                {
                    MultiSub[Language.Farsi] = Helpers.SubtitleHelper.PopulateSubtitle(subtitleContent);
                }

                filename = $"TH-SUB-SV-SC-{CurrentScene.ToString("00")}.txt";
                subtitleContent = Helpers.SubtitleHelper.ReadSubtitleFile(Shared.Models.Mode.Theater, filename);
                if (!MultiSub.ContainsKey(Language.Svenska))
                {

                    MultiSub.Add(Language.Svenska, Helpers.SubtitleHelper.PopulateSubtitle(subtitleContent));
                }
                else
                {
                    MultiSub[Language.Svenska] = Helpers.SubtitleHelper.PopulateSubtitle(subtitleContent);
                }


                this.IsHeadphonesVisible = false;
                MediaMessage.Message = Resources.AppResources.SubtitlesLoaded;
                //Messages.Clear();
                //Messages.Insert(0, new ChatMessage
                //{
                //    Message = "Subtitles Loaded",
                //    User = Helpers.Settings.UserName,
                //    Color = Color.FromRgba(0, 0, 0, 0)
                //});

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
                //Messages.Clear();

                //Messages.Insert(0, new ChatMessage
                //{
                //    Message = this.CurrentSub[this.currentSubIndex].Text,
                //    User = Helpers.Settings.UserName,
                //    Color = Color.FromRgba(0, 0, 0, 0)
                //});
                MediaMessage.Message = this.CurrentSub[this.currentSubIndex].Text;
            }
            catch (Exception ex)
            {

            }
        }

        private async Task SelectCharchter(Character character)
        {
            switch (character)
            {
                case Character.Lyla:
                    IsLeylaSelected = true;
                    break;
                case Character.Sina:
                    IsSinaSelected = true;
                    break;
                case Character.Tara:
                    IsTaraSelected = true;
                    break;
            }
        }

    }
}
