using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Swegrant.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Title = Resources.MenuTitles.Main;
            SVFlag = ImageSource.FromResource("Swegrant.Resources.SV_Flag.png");
            FAFlag = ImageSource.FromResource("Swegrant.Resources.FA_Flag.png");
        }

        public ImageSource SVFlag { get; set; }

        public ImageSource FAFlag { get; set; }
    }
}
