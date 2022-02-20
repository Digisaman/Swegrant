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
    /// Interaction logic for SubtitlePlayer.xaml
    /// </summary>
    public partial class SubtitlePlayer : UserControl
    {
        private Task subPlayBackTask;
        private CancellationTokenSource currentSubCancelationSource;
        Subtitle[] currentSub;

        public int CurrentIndex { get; private set; }

        public SubtitlePlayer()
        {
            InitializeComponent();
        }


        public void LoadSubtitle(Mode mode, Character character, Language language, int scene)
        {
            try
            {
                string subtitleDirectory = "";
                string subtitleFilePath = "";
                string lang = language.ToString().Substring(0, 2).ToUpper();
                string charachter = character.ToString().Substring(0, 2).ToUpper();
                if (mode == Mode.Theater)
                {
                    subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA\\VDSUB";
                    subtitleFilePath = $"{subtitleDirectory}\\TH-SUB-{lang}-SC-{scene.ToString("00")}.txt";
                }
                else if (mode == Mode.Video)
                {
                    subtitleDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA\\VDSUB";
                    subtitleFilePath = $"{subtitleDirectory}\\VD-{charachter}-SUB-{lang}-SC-{scene.ToString("00")}.txt";
                }



                if (File.Exists(subtitleFilePath))
                {
                    string text = System.IO.File.ReadAllText(subtitleFilePath);
                    currentSub = FillSubtitleListBox(text);
                    this.lstSub.ItemsSource = currentSub.Select(c => c.Text).ToArray();
                    this.CurrentIndex = 0;
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

        private Subtitle[] FillSubtitleListBox(string text)
        {
            List<string> list = text.Split(new string[] { Environment.NewLine + Environment.NewLine },
                              StringSplitOptions.RemoveEmptyEntries).ToList();
            List<Subtitle> subList = new List<Subtitle>();
            //this.currentSubIndex = 0;

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
                subList.Add(sub);
            }
            return subList.ToArray();
        }

        public void PlaySub()
        {
            try
            {

                this.currentSubCancelationSource = new CancellationTokenSource();
                this.subPlayBackTask = Task.Run(() =>
                {
                    this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                    PlaySubExecute();

                }, this.currentSubCancelationSource.Token);
            }
            catch (Exception ex)
            {
                throw new Exception("PlaySub", ex);
            }
        }

        public void StopSub()
        {
            int index = 0;
            if (this.subPlayBackTask != null && this.subPlayBackTask.Status == TaskStatus.Running)
            {
                this.currentSubCancelationSource.Cancel();
            }
            this.CurrentIndex = 0;
        }

        public void PauseSub()
        {
            int index = 0;
            if (this.subPlayBackTask != null && this.subPlayBackTask.Status == TaskStatus.Running)
            {
                this.currentSubCancelationSource.Cancel();
            }
        }
        private async Task PlaySubExecute()
        {
            if (this.CurrentIndex == 0)
            {
                TimeSpan initial = this.currentSub[this.CurrentIndex].StartTime;
                Thread.Sleep(initial);
            }

            for (int i = CurrentIndex; i < this.currentSub.Length; i++)
            {
                try
                {
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                        return;
                    }
                    this.CurrentIndex = i;

                    Thread.Sleep(this.currentSub[i].Duration);
                    if (this.currentSubCancelationSource.IsCancellationRequested)
                    {
                        this.currentSubCancelationSource.Token.ThrowIfCancellationRequested();
                        return;
                    }

                    this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.lstSub.SelectedIndex = this.lstSub.SelectedIndex + 1;
                    this.lstSub.ScrollIntoView(this.lstSub.Items[this.lstSub.SelectedIndex]);
                }));

                    if (i < this.currentSub.Length - 1)
                    {
                        TimeSpan gap = this.currentSub[i + 1].StartTime - this.currentSub[i].EndTime;
                        Thread.Sleep(gap);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SubtitleProcessing", ex);
                }
            }
        }

    }


}
