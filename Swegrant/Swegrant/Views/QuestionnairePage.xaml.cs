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
    public partial class QuestionnairePage : ContentPage
    {
        QuestionnaireViewModel vm;
        QuestionnaireViewModel VM
        {
            get => vm ?? (vm = (QuestionnaireViewModel)BindingContext);
        }
        public QuestionnairePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, Helpers.Settings.IsUserAdmin);
            if (!DesignMode.IsDesignModeEnabled)
                VM.LoadQuestionsCommand.Execute(null);
        }
    }
}