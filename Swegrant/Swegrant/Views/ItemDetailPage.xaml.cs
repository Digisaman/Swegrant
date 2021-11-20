using Swegrant.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Swegrant.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}