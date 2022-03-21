using Swegrant.Helpers;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Swegrant.ViewModels
{
    public class SubtitleControllerViewModel : BaseViewModel
    {
        public SubtitleControllerViewModel()
        {
            Title = Resources.MenuTitles.SubtitleController;
            HideSubtitle = new Command(async () => await ServerHelper.HideSubtitle());
            ShowSubtitle = new Command(async () => await ServerHelper.ShowSubtitle());
            ResumeAutoSub = new Command(async () => await ServerHelper.ResumeAutoSub());
            PauseAutoSub = new Command(async () => await ServerHelper.PauseAutoSub());
            NextMaunualSub = new Command(async () => await ServerHelper.NextMaunualSub());
        }

        public ICommand HideSubtitle { get; }

        public ICommand ShowSubtitle { get; }

        public ICommand ResumeAutoSub { get; }

        public ICommand PauseAutoSub { get; }
        public ICommand NextMaunualSub { get; }
    }
      
}