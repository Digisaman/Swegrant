using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Swegrant.Droid;
using Android.Media;
using Android.Content.Res;
using Swegrant.Interfaces;
using System.IO;
using static Swegrant.Models.MediaInfo;

[assembly: Dependency(typeof(AudioService))]
namespace Swegrant.Droid
{
    public class AudioService : IAudio
    {
        //private static MediaPlayer _Player;
        //public static MediaPlayer Player
        //{
        //    get
        //    {
        //        MediaPlayer mediaPlayer = new MediaPlayer();
        //        MediaPlayer.
        //        if (_Player == null)
        //        {
        //            _Player = new MediaPlayer();
        //        }
        //        return _Player;
        //    }
        //}

        private int currentPosition;

        private DateTime changeAudioDateTime;

        private MediaPlayer player;
        public AudioService()
        {
            player = new MediaPlayer();
        }
        public void PrepareAudioFile(string fileName)
        {
            try
            {
                string externalStorageDirectory = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
                string pathToAudioDirectory = Path.Combine(externalStorageDirectory, DownloadCategory.AUDIO.ToString());
                string pathToAudioFile = Path.Combine(pathToAudioDirectory, fileName);
                currentPosition = 0;
                if (player.IsPlaying)
                {
                    currentPosition = player.CurrentPosition;
                    player.Stop();
                    player = new MediaPlayer();
                    this.changeAudioDateTime = DateTime.Now;
                    player.Prepared += (s, e) =>
                    {
                        PlayAudioFile();
                    };

                }
                //using (FileStream stream = File.OpenRead(pathToAudioFile))
                //{
                //var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
                //player.Prepared += (s, e) =>
                //{
                //    if (currentPosition != 0)
                //    {
                //        player.SeekTo(currentPosition+100);
                //    }
                //    player.Start();
                //};
                player.SetDataSource(pathToAudioFile);

                //player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                player.Prepare();
                //}
            }
            catch (Exception ex)
            {

            }
        }


        public void PlayAudioFile()
        {
            try
            {
                if (this.currentPosition != 0)
                {
                    int pauseTime = Convert.ToInt32((DateTime.Now - changeAudioDateTime).TotalMilliseconds);
                    player.SeekTo(currentPosition + pauseTime);
                }
                player.Start();
            }
            catch (Exception ex)
            {

            }
        }

        public void StopAudioFile()
        {
            try
            {
                if (player != null)
                {
                    player.Stop();
                    player = new MediaPlayer();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}