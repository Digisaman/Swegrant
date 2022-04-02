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
    /// Interaction logic for VideoControl.xaml
    /// </summary>
    public partial class VideoControl : UserControl
    {
        #region Properties
        private int currentScene = 1;
        private Language currentLanguage = Shared.Models.Language.Farsi;
        //private Character currentCharacter = Shared.Models.Character.Leyla;
        private Dictionary<Character, Subtitle[]> currentSub;
        //private int currentSubIndex = 0;
        private Task currentSubTask;
        private CancellationTokenSource currentSubCancelationSource;
        private CancellationToken currentSubCancellationToken;
        private int theaterSceneSelectedIndex;
        #endregion

        public VideoControl()
        {
            InitializeComponent();
            this.chkMuteVideo.IsChecked = true;
            this.currentSub = new Dictionary<Character, Subtitle[]>();
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

        #region EventHandlers
        private async void btnLoadSubTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                subLeyla.LoadSubtitle(Mode.Video, Character.Lyla, currentLanguage, currentScene);
                subSina.LoadSubtitle(Mode.Video, Character.Sina, currentLanguage, currentScene);
                subTara.LoadSubtitle(Mode.Video, Character.Tara, currentLanguage, currentScene);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private async void btnPrepareAudio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
                {
                    Command = Command.Prepare,
                    Mode = Mode.Video,
                    Scene = currentScene

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //private void cmbCharchter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string character = this.cmbCharchter.SelectionBoxItem.ToString();
        //    if (!string.IsNullOrEmpty(character))
        //    {
        //        currentCharacter = (Character)Enum.Parse(typeof(Character), character);
        //    }
        //}

        private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBoxItem item = (ComboBoxItem) this.cmbLanguage.SelectedItem;
            if (this.cmbLanguage.SelectedItem != null)
            {
                currentLanguage = (Language)Enum.Parse(typeof(Language), cmbLanguage.SelectedItem.ToString());
            }
        }

        private void cmbScence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbScence.SelectedItem != null)
            {
                this.currentScene = Convert.ToInt32(this.cmbScence.SelectedItem);
            }
        }
        #endregion

        #region Methods
        //private async Task LoadSubtitle(Character currentCharacter)
        //{


        //    try
        //    {
        //        string lang = currentLanguage.ToString().Substring(0, 2).ToUpper();
        //        string charachter = currentCharacter.ToString().Substring(0, 2).ToUpper();
        //        string subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA\\VDSUB";
        //        string subtitleFilePath = $"{subtitleDirectory}\\VD-{charachter}-SUB-{lang}-SC-{this.currentScene.ToString("00")}.txt";
        //        string text = "";
        //        if (File.Exists(subtitleFilePath))
        //        {
        //            text = System.IO.File.ReadAllText(subtitleFilePath);
        //            Subtitle[] subtitles = FillSubtitleListBox(text);
        //            if (currentSub.ContainsKey(currentCharacter))
        //            {
        //                currentSub[currentCharacter] = subtitles;
        //            }
        //            else
        //            {
        //                currentSub.Add(currentCharacter, subtitles);
        //            }
        //            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
        //            {
        //                Command = Command.Prepare,
        //                Mode = Mode.Theater,
        //                Scene = this.currentScene
        //            });
        //        }
        //        else
        //        {
        //            MessageBox.Show("File Does NOT Exist");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Load Subtitle error", ex);
        //    }
        //}
        //private Subtitle[] FillSubtitleListBox(string text)
        //{
        //    List<string> list = text.Split(new string[] { Environment.NewLine + Environment.NewLine },
        //                      StringSplitOptions.RemoveEmptyEntries).ToList();
        //    List<Subtitle> subList = new List<Subtitle>();
        //    //this.currentSubIndex = 0;

        //    //List<string> subLine = new List<string>();
        //    foreach (var item in list)
        //    {
        //        Subtitle sub = new Subtitle();
        //        string[] parts = item.Split(new string[] { Environment.NewLine },
        //                       StringSplitOptions.RemoveEmptyEntries).ToArray();
        //        sub.Id = Convert.ToInt32(parts[0]);
        //        string[] times = parts[1].Split("-->", StringSplitOptions.RemoveEmptyEntries).ToArray();
        //        sub.StartTime = TimeSpan.Parse(times[0].Replace(',', '.').Trim());
        //        sub.EndTime = TimeSpan.Parse(times[1].Replace(',', '.').Trim());
        //        string line = "";
        //        for (int i = 2; i < parts.Length; i++)
        //        {
        //            line += parts[i];
        //        }
        //        sub.Text = line;
        //        subList.Add(sub);
        //    }
        //    return subList.ToArray();
        //}

        //private void PlaySub(Character currentCharacter)
        //{
        //    //int currentSubIndex = 0;

        //    TimeSpan initial = this.currentSub[currentCharacter][0].StartTime;
        //    Thread.Sleep(initial);
        //    for (int i = 0; i < this.currentSub.Count; i++)
        //    {
        //        try
        //        {
        //            if (this.currentSubCancelationSource.IsCancellationRequested)
        //            {
        //                this.currentSubCancellationToken.ThrowIfCancellationRequested();
        //                return;
        //            }

        //            Thread.Sleep(this.currentSub[currentCharacter][i].Duration);
        //            if (this.currentSubCancelationSource.IsCancellationRequested)
        //            {
        //                this.currentSubCancellationToken.ThrowIfCancellationRequested();
        //                return;
        //            }
        //            switch (currentCharacter)
        //            {
        //                case Character.Lyla:
        //                    this.Dispatcher.BeginInvoke(new Action(() =>
        //                        this.lstSubLeyla.SelectedIndex = this.lstSubLeyla.SelectedIndex + 1));
        //                    break;
        //                case Character.Sina:
        //                    this.Dispatcher.BeginInvoke(new Action(() =>
        //                        this.lstSubSina.SelectedIndex = this.lstSubSina.SelectedIndex + 1));
        //                    break;
        //                case Character.Tara:
        //                    this.Dispatcher.BeginInvoke(new Action(() =>
        //                        this.lstSubTara.SelectedIndex = this.lstSubTara.SelectedIndex + 1));
        //                    break;
        //            }

        //            if (i < this.currentSub.Count - 1)
        //            {
        //                TimeSpan gap = this.currentSub[currentCharacter][i + 1].StartTime - this.currentSub[currentCharacter][i].EndTime;
        //                Thread.Sleep(gap);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("SubtitleProcessing", ex);
        //        }
        //    }
        //}
        #endregion

        private async void btnPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {

                //string lang = this.cmbthLanguage.SelectionBoxItem.ToString();
                //string scence = this.cmbvdScence.SelectionBoxItem.ToString();
                //this.currentScene = Convert.ToInt32(scence);
                string text = "";
                string VideoDirectory = $"{Directory.GetCurrentDirectory()}\\Video";
                string videoFilePath = $"{VideoDirectory}\\VD-SC-{this.currentScene.ToString("00")}.mp4";
                if (File.Exists(videoFilePath))
                {

                    await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
                    {
                        Command = Command.Play,
                        Mode = Mode.Video,
                        Scene = this.currentScene
                    });

                    this.currentSubCancelationSource = new CancellationTokenSource();
                    this.currentSubTask = Task.Run(() =>
                    {
                        this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();

                        this.subLeyla.PlaySub();
                        this.subSina.PlaySub();
                        this.subTara.PlaySub();
                        MainWindow.Singleton.PlaySecondaryVideo(videoFilePath, chkMuteVideo.IsChecked.Value);

                    }, this.currentSubCancelationSource.Token);

                    

                    //PLayVideo(videoFilePath);
                    //Task.Run(PlaySub);
                    //_SecondaryWindow.Play(videoFilePath);


                }
                else
                {
                    MessageBox.Show("File Does NOT Exist");
                }
            }
        }

        private async void btnSwitchVideo_Click(object sender, RoutedEventArgs e)
        {
            await SwitchToVideo();
        }

        private async Task SwitchToVideo()
        {
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.ChangeMode,
                Mode = Mode.Video,
                Scene = currentScene
            });
        }
    }
}
