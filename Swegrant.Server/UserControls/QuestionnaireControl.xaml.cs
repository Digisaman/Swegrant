using Swegrant.Server.Controllers;
using System;
using System.Collections.Generic;
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
        public QuestionnaireControl()
        {
            InitializeComponent();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SetQuestionnaireDataSource();
        }

        private void SetQuestionnaireDataSource()
        {
            this.dgInfo.ItemsSource = null;
            this.dgInfo.ItemsSource = MediaController.Questions;
        }
    }
}
