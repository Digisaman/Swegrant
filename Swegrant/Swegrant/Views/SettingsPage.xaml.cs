using Swegrant.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swegrant.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel vm;
        SettingsViewModel VM
        {
            get => vm ?? (vm = (SettingsViewModel)BindingContext);
        }
        public SettingsPage()
        {
            InitializeComponent();
        }
    }
}