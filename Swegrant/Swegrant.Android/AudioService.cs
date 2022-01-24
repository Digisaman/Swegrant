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
        private MediaPlayer player;
        public AudioService()
        {
            player = new MediaPlayer();
        }
        public void PlayAudioFile(string fileName)
        {
            try
            {

                int currentPosition = 0;
                if (player.IsPlaying)
                {
                    currentPosition = player.CurrentPosition;
                    player.Stop();
                    player = new MediaPlayer();

                }

                var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
                player.Prepared += (s, e) =>
                {
                    if (currentPosition != 0)
                    {
                        player.SeekTo(currentPosition+100);
                    }
                    player.Start();
                };
                player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                player.Prepare();
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