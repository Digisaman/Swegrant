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
    public partial class TheaterPage : ContentPage
    {
        

        TheaterViewModel vm;
        TheaterViewModel VM
        {
            get => vm ?? (vm = (TheaterViewModel)BindingContext);
        }

        public TheaterPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            if (!DesignMode.IsDesignModeEnabled)
                VM.ConnectCommand.Execute(null);
        }

    }
}