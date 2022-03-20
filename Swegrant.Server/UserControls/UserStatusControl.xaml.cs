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
    /// Interaction logic for UserStatusControl.xaml
    /// </summary>
    public partial class UserStatusControl : UserControl
    {
        
        public ObservableCollection<SubmitUserStatus> UserStatuses
        {
            get; set;

        }

        public UserStatusControl()
        {
            InitializeComponent();
            this.UserStatuses = new ObservableCollection<SubmitUserStatus>();
            this.dgInfo.ItemsSource = this.UserStatuses;
            
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SetDataSource();
        }

        private void SetDataSource()
        {
            this.dgInfo.ItemsSource = null;
            this.dgInfo.ItemsSource = this.UserStatuses;
        }

        public void AddUserStatus(SubmitUserStatus submitUserStatus)
        {
            submitUserStatus.Id = UserStatuses.Count + 1;
            UserStatuses.Add(submitUserStatus);
        }
    }
}
