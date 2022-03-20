using Swegrant.Server.Controllers;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    /// Interaction logic for QuestionnaireControl.xaml
    /// </summary>
    public partial class QuestionnaireControl : UserControl
    {
        public ObservableCollection<SubmitQuestion> Questions
        {
            get; set;

        }
        public QuestionnaireControl()
        {
            InitializeComponent();
            Questions = new ObservableCollection<SubmitQuestion>();
            this.dgInfo.ItemsSource = Questions;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SetDataSource();
        }

        private void SetDataSource()
        {
            this.dgInfo.ItemsSource = null;
            this.dgInfo.ItemsSource = Questions;
        }

        private async void btnNavigate_Click(object sender, RoutedEventArgs e)
        {
            await MainWindow.Singleton.SendGroupMessage(new ServiceMessage
            {
                Command = Command.NavigateQuestionnaire,
                Mode = Mode.None
            });
        }

        public void AddUserStatus(SubmitQuestion submitQuestion)
        {
            submitQuestion.Id = Questions.Count + 1;
            this.Questions.Add(submitQuestion);
        }
    }
}
