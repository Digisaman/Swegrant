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
        private Character currentCharacter = Shared.Models.Character.Leyla;
        private List<Subtitle> currentSub;
        private int currentSubIndex = 0;
        private Task currentSubTask;
        private CancellationTokenSource currentSubCancelationSource;
        private CancellationToken currentSubCancellationToken;
        private int theaterSceneSelectedIndex;
        #endregion

        public VideoControl()
        {
            InitializeComponent();
        }

        #region EventHandlers
        private async void btnLoadSubTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadSubtitle();
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
                await LoadSubtitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void cmbCharchter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string character = this.cmbCharchter.SelectionBoxItem.ToString();
            if (!string.IsNullOrEmpty(character))
            {
                currentCharacter = (Character)Enum.Parse(typeof(Character), character);
            }
        }

        private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string lang = this.cmbLanguage.SelectionBoxItem.ToString();
            if (!string.IsNullOrEmpty(lang))
            {
                currentLanguage = (Language)Enum.Parse(typeof(Language), lang);
            }
        }

        private void cmbScence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string scene = this.cmbScence.SelectionBoxItem.ToString();
            if (!string.IsNullOrEmpty(scene))
            {
                this.currentScene = Convert.ToInt32(scene);
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
                    string charachter = currentCharacter.ToString().Substring(0, 2).ToUpper();
                    string subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA\\VDSUB";
                    string subtitleFilePath = $"{subtitleDirectory}\\VD-{charachter}-SUB-{lang}-SC-{this.currentScene.ToString("00")}.txt";
                    string text = "";
                    if (File.Exists(subtitleFilePath))
                    {
                        text = System.IO.File.ReadAllText(subtitleFilePath);
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
                        MessageBox.Show("File Does NOT Exist");
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
        #endregion

        private void btnPlayVideo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
