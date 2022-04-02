using Swegrant.Server.Helpers;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Swegrant.Server.UserControls
{
    /// <summary>
    /// Interaction logic for TheaterControl.xaml
    /// </summary>
    public partial class TheaterControl : UserControl
    {
        #region Properties
        private int currentScene = 1;
        private Language currentLanguage = Shared.Models.Language.Farsi;
        //private Character currentCharacter = Character.None;
        private List<Subtitle> currentSub;
        private int currentSubIndex = 0;
        private Task currentSubTask;
        private CancellationTokenSource currentSubCancelationSource;
        private CancellationToken currentSubCancellationToken;
        private string currentBackgroundVideo;
        private string selectedBackGroundVideo;
        private System.Timers.Timer VideoTimer;
        #endregion


        public TheaterControl()
        {
            InitializeComponent();
            this.txtVideoTime.Text = "00:00";
            this.VideoTimer = new System.Timers.Timer();
            this.VideoTimer.Interval = 1000;
            this.VideoTimer.Elapsed += VideoTimer_Elapsed;
            this.chkMuteVideo.IsChecked = false;
            this.btnPlayVideo.IsEnabled = false;   
            this.cmbLanguage.ItemsSource = new Language[]
            {
                Shared.Models.Language.Farsi,
                Shared.Models.Language.Svenska
            };
            this.cmbScence.ItemsSource = new int[]
            {
                1,
                2,
                3,
                4,
                5
            };
            
        }

        private void VideoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
                this.txtVideoTime.Text = MainWindow.Singleton.GetVideoTime() ));
            
        }


        #region EventHandlers

        private async void btnNextSub_Click(object sender, RoutedEventArgs e)
        {
            await LoadNextSubtitleLine();
        }

        private async void btnRsumeAutoSub_Click(object sender, RoutedEventArgs e)
        {
            await ResumeAutoSub();
        }
        private async void btnPauseAutoSub_Click(object sender, RoutedEventArgs e)
        {
            await PauseAutoSub();
        }

        private async void btnShowSelectCharchterText_Click(object sender, RoutedEventArgs e)
        {
            await ShowSelectCharacterText();
        }

        private async void btnHideSelectCharchter_Click(object sender, RoutedEventArgs e)
        {
            await HideSelectCharacter();
        }
        private async void btnShowSub_Click(object sender, RoutedEventArgs e)
        {
            await ShowSubtitle();
        }

        private async void btnPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            this.btnPlayVideo.IsEnabled = false;
            MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {

                PlayVideo();
            }
        }

        private async void btnLoadSubTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.btnPlayVideo.IsEnabled = true;
                await LoadSubtitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private async void btnHideSub_Click(object sender, RoutedEventArgs e)
        {
            await HideSubtitle();
        }


        private void btnChangeVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.currentBackgroundVideo!= this.selectedBackGroundVideo)
                {
                    string text = "";
                    //string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
                    string scence = this.cmbScence.SelectionBoxItem.ToString();
                    string VideoDirectory = $"{Directory.GetCurrentDirectory()}\\Theater";
                    string videoFilePath = $"{VideoDirectory}\\{selectedBackGroundVideo}";
                    if (File.Exists(videoFilePath))
                    {
                        //PLayVideo(videoFilePath);
                        //Task.Run(PlaySub);
                        MainWindow.Singleton.PlaySecondaryVideo(videoFilePath, false);
                        this.currentBackgroundVideo = this.selectedBackGroundVideo;
                    }
                    else
                    {
                        MessageBox.Show("File Does NOT Exist");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnStopVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning");
                if (result == MessageBoxResult.OK)
                {
                    MainWindow.Singleton.ToggleVideoVisibility(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task PlayVideo()
        {
            try
            {

                string text = "";
                string VideoDirectory = $"{Directory.GetCurrentDirectory()}\\Theater";
                string videoFilePath = $"{VideoDirectory}\\{selectedBackGroundVideo}";
                if (File.Exists(videoFilePath))
                {
                    await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
                    {
                        Command = Command.Play,
                        Mode = Mode.Theater,
                        Scene = this.currentScene
                    });
                    this.currentSubCancelationSource = new CancellationTokenSource();
                    this.currentSubTask = Task.Run(() =>
                    {
                        this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                        PlaySub();

                    }, this.currentSubCancelationSource.Token);
                    MainWindow.Singleton.PlaySecondaryVideo(videoFilePath, chkMuteVideo.IsChecked.Value);
                    this.currentBackgroundVideo = this.selectedBackGroundVideo;
                    this.VideoTimer.Start();

                }
                else
                {
                    MessageBox.Show("File Does NOT Exist");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbScence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object scene = this.cmbScence.SelectedItem;
            if (scene != null)
            {
                this.currentScene = Convert.ToInt32(scene);
                string[] videos = FileHelpers.GetFileList(Mode.Theater, this.currentScene);
                if (videos.Any())
                {
                    cmbVideo.ItemsSource = videos;
                    cmbVideo.SelectedIndex = 0;
                }
            }
        }

        private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string lang = this.cmbLanguage.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(lang))
            {
                currentLanguage = (Language)Enum.Parse(typeof(Language), lang);
            }
        }

        private void cmbVideo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbVideo.SelectedItem != null)
            {
                this.selectedBackGroundVideo = cmbVideo.SelectedItem.ToString();
                this.txtVideoTime.Text = "00:00";
                this.VideoTimer.Stop();
            }
        }



        #endregion

        #region Methods
        private async Task LoadSubtitle()
        {
            MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {

                try
                {
                    string lang = currentLanguage.ToString().Substring(0, 2).ToUpper();
                    string subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA\\THSUB";
                    string subtitleFilePath = $"{subtitleDirectory}\\TH-SUB-{lang}-SC-{this.currentScene.ToString("00")}.txt";
                    if (File.Exists(subtitleFilePath))
                    {
                        string text = System.IO.File.ReadAllText(subtitleFilePath);
                        FillSubtitleListBox(text);
                        await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
                        {
                            Command = Command.Prepare,
                            Mode = Mode.Theater,
                            Scene = this.currentScene
                        });
                    }
                    else
                    {
                        throw new Exception("File Does NOT Exist");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Load Subtitle error", ex);
                }
            }
        }

        private void FillSubtitleListBox(string text)
        {
            List<string> list = text.Split(new string[] { Environment.NewLine + Environment.NewLine },
                              StringSplitOptions.RemoveEmptyEntries).ToList();
            this.currentSub = new List<Subtitle>();
            this.currentSubIndex = 0;

            //List<string> subLine = new List<string>();
            foreach (var item in list)
            {
                Subtitle sub = new Subtitle();
                string[] parts = item.Split(new string[] { Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries).ToArray();
                sub.Id = Convert.ToInt32(parts[0]);
                string[] times = parts[1].Split("-->", StringSplitOptions.RemoveEmptyEntries).ToArray();
                sub.StartTime = TimeSpan.Parse(times[0].Replace(',', '.').Trim());
                sub.EndTime = TimeSpan.Parse(times[1].Replace(',', '.').Trim());
                string line = "";
                for (int i = 2; i < parts.Length; i++)
                {
                    line += parts[i];
                    if (i != parts.Length - 1)
                    {
                        line += Environment.NewLine;
                    }
                }
                sub.Text = line;
                currentSub.Add(sub);
            }
            this.lstSub.ItemsSource = currentSub.Select(c => c.Text).ToArray();
            this.lstSub.SelectedIndex = 0;
        }

        private void PlaySub()
        {
            if (this.currentSubIndex == 0)
            {
                TimeSpan initial = this.currentSub[this.currentSubIndex].StartTime;
                Thread.Sleep(initial);
            }
            for (int i = this.currentSubIndex; i < this.currentSub.Count; i++)
            {
                try
                {
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancellationToken.ThrowIfCancellationRequested();
                        return;
                    }
                    this.currentSubIndex = i;

                    MainWindow.Singleton.DisplaySecondarySub(currentSub[i].Text);

                    Thread.Sleep(currentSub[i].Duration);
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancellationToken.ThrowIfCancellationRequested();
                        return;
                    }
                    MainWindow.Singleton.DisplaySecondarySub(" ");

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.lstSub.SelectedIndex = this.lstSub.SelectedIndex + 1;
                        this.lstSub.ScrollIntoView(this.lstSub.Items[this.lstSub.SelectedIndex]);
                    }));

                    if (this.currentSubIndex < this.currentSub.Count - 1)
                    {
                        TimeSpan gap = currentSub[i + 1].StartTime - currentSub[i].EndTime;
                        Thread.Sleep(gap);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SubtitleProcessing", ex);
                }
            }
        }

        public async Task LoadNextSubtitleLine()
        {
            try
            {

                string message = this.lstSub.SelectedItem.ToString();
                this.currentSubIndex++;
                this.lstSub.SelectedIndex = currentSubIndex;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //this.lstSub.SelectedIndex = this.lstSub.SelectedIndex + 1;
                    this.lstSub.ScrollIntoView(this.lstSub.Items[this.lstSub.SelectedIndex]);
                }));

                MainWindow.Singleton.DisplaySecondarySub(this.currentSub[this.currentSubIndex].Text);
                await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
                {
                    Mode = Mode.Theater,
                    Command = Command.DisplayManualSub,
                    Index = currentSubIndex,
                    Scene = this.currentScene
                });

                
            }
            catch (Exception ex)
            {

            }
        }

        public async Task ResumeAutoSub()
        {
            try
            {
                this.currentSubCancelationSource = new CancellationTokenSource();
                //this.currentSubCancellationToken = this.currentSubCancelationSource.Token;
                //this.currentSubTask = Task.Run(PlaySub);
                this.currentSubTask = Task.Run(() =>
                {
                    MainWindow.Singleton.SendGroupMessage(new ServiceMessage
                    {
                        Command = Command.ResumeAutoSub,
                        Mode = Mode.Theater,
                        Scene = this.currentScene

                    });

                    this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                    PlaySub();
                }, this.currentSubCancelationSource.Token);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public async Task PauseAutoSub()
        {
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.PauseAutoSub,
                Mode = Mode.Theater,
                Scene = this.currentScene
            });
            if (this.currentSubTask != null && this.currentSubTask.Status == TaskStatus.Running)
            {
                this.currentSubCancelationSource.Cancel();
            }
        }


        public async Task ShowSelectCharacterText()
        {
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.ShowSelectCharacterText,
                Mode = Mode.Theater,
                Scene = this.currentScene
            });
        }

        public async Task ShowSelectCharacterImage()
        {
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.ShowSelectCharacterImage,
                Mode = Mode.Theater,
                Scene = this.currentScene
            });
        }


        private async Task HideSelectCharacter()
        {
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.HideSelectCharchter,
                Mode = Mode.Theater,
                Scene = this.currentScene

            });
        }

        public async Task ShowSubtitle()
        {

            MainWindow.Singleton.ToggleSecondarySubVisibility(true);
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.ShowSubtitle,
                Mode = Mode.Theater,
                Scene = currentScene
            });
        }

        public async Task HideSubtitle()
        {
            MainWindow.Singleton.ToggleSecondarySubVisibility(false);
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.HideSubtitle,
                Mode = Mode.Theater,
                Scene = currentScene
            });
        }

        private async Task SwitchToTheater()
        {
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.ChangeMode,
                Mode = Mode.Theater,
                Scene = currentScene
            });
        }



        #endregion

        private async void btnSwitchTheater_Click(object sender, RoutedEventArgs e)
        {
            await SwitchToTheater();
        }

        private void btnShowVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                
                    MainWindow.Singleton.ToggleVideoVisibility(true);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
        private void btnHideVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning");
                if (result == MessageBoxResult.OK)
                {
                    MainWindow.Singleton.ToggleVideoVisibility(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnShowSelectCharchterImage_Click(object sender, RoutedEventArgs e)
        {
            await ShowSelectCharacterImage();
        }

        private void btnPauseVideo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Singleton.PauseVideo();
            this.VideoTimer.Stop();
        }

        private void btnResumeVideo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Singleton.ResumeVideo();
            this.VideoTimer.Start();
        }
    }
}
