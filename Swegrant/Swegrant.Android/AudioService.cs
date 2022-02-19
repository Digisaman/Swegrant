using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Swegrant.Droid;
using Android.Media;
using Swegrant.Interfaces;
using System.IO;
using Swegrant.Shared.Models;

[assembly: Dependency(typeof(AudioService))]
namespace Swegrant.Droid
{
    public class AudioService : IAudio
    {
        private Language currentLanguage;

        private Dictionary<Language, MediaPlayer> mediaPlayers;

        private int currentPosition;

        private DateTime changeAudioDateTime;

        //private MediaPlayer player;

        public AudioService()
        {
            mediaPlayers = new Dictionary<Language, MediaPlayer>();
            if (!mediaPlayers.ContainsKey(Language.Farsi))
            {
                mediaPlayers.Add(Language.Farsi,
                    new MediaPlayer());
            }
            if (!mediaPlayers.ContainsKey(Language.Svenska))
            {
                mediaPlayers.Add(Language.Svenska,
                    new MediaPlayer());
            }
        }
        public void PrepareAudioFile(Language selectedLanguage, string fileName)
        {
            try
            {


                string appDataDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                string pathToAudioDirectory = Path.Combine(appDataDirectory, DownloadCategory.AUDIO.ToString());
                string pathToAudioFile = Path.Combine(pathToAudioDirectory, fileName);
                currentPosition = 0;
                //if (mediaPlayers[currentLanguage].IsPlaying)
                //{
                //    currentPosition = mediaPlayers[currentLanguage].CurrentPosition;
                //    mediaPlayers[currentLanguage].Stop();

                //   // mediaPlayers[currentLanguage] = new MediaPlayer();

                //    this.changeAudioDateTime = DateTime.Now;
                //    mediaPlayers[currentLanguage].Prepared += (s, e) =>
                //    {
                //        PlayAudioFile();
                //    };
                //}



                mediaPlayers[selectedLanguage].SetDataSource(pathToAudioFile);

                mediaPlayers[selectedLanguage].Prepare();

                this.currentLanguage = selectedLanguage;
                //}
            }
            catch (Exception ex)
            {

            }
        }


        public void PlayAudioFile(Language selectedLanguage)
        {
            try
            {


                if (mediaPlayers[currentLanguage].IsPlaying)
                {
                    this.changeAudioDateTime = DateTime.Now;
                    currentPosition = mediaPlayers[currentLanguage].CurrentPosition;
                    mediaPlayers[currentLanguage].Pause();

                    // mediaPlayers[currentLanguage] = new MediaPlayer();


                    //mediaPlayers[currentLanguage].Prepared += (s, e) =>
                    //{
                    //    PlayAudioFile();
                    //};
                }

                if (this.currentPosition != 0)
                {
                    int pauseTime = Convert.ToInt32((DateTime.Now - changeAudioDateTime).TotalMilliseconds);
                    mediaPlayers[selectedLanguage].SeekTo(currentPosition + pauseTime);
                }
                currentLanguage = selectedLanguage;
                mediaPlayers[currentLanguage].Start();

            }
            catch (Exception ex)
            {

            }
        }

        public void StopAudioFile()
        {
            try
            {
                if (mediaPlayers[currentLanguage] != null)
                {
                    mediaPlayers[currentLanguage].Stop();
                    mediaPlayers[currentLanguage] = new MediaPlayer();
                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}