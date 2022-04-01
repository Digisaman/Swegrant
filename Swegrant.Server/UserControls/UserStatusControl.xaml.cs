using Swegrant.Server.Controllers;
using Swegrant.Server.Helpers;
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
            //this.dgInfo.ScrollIntoView(this.dgInfo.Items.GetItemAt(this.dgInfo.Items.Count - 1));
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            bool result = FileHelpers.ExportExcelUserStatus(UserStatuses);
            MessageBox.Show((result ? "Export Successfull" : "Export Failed"));
        }
       
        public Character AutoAssignCharachter()
        {
            Character character = Character.None;

            try
            {

                int countSina = UserStatuses.Count(c => c.Event == UserEvent.CharacterSelected && c.Value == Character.Sina.ToString());
                int countTara = UserStatuses.Count(c => c.Event == UserEvent.CharacterSelected && c.Value == Character.Tara.ToString());
                int countLeyla = UserStatuses.Count(c => c.Event == UserEvent.CharacterSelected && c.Value == Character.Lyla.ToString());



                int min = Math.Min(countSina, Math.Min(countTara, countLeyla));

                if (min == countSina)
                    return Character.Sina;
                else if (min == countTara)
                    return Character.Tara;
                else if (min == countLeyla)
                    return Character.Lyla;
                else
                {
                    Random random = new Random();
                    int num = random.Next(1, 4);
                    character = (Character)Enum.Parse(typeof(Character), num.ToString());
                }
            }
            catch(Exception ex)
            {
                Random random = new Random();
                int num = random.Next(1, 4);
                character = (Character)Enum.Parse(typeof(Character), num.ToString());
            }

            return character;
        }
    }
}
