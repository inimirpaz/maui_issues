using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinToolbarItemBindingIssue
{
    public partial class MainPage : ContentPage
    {
        public ICommand SelectedCommand { get; private set; }

        public MainPage()
        {
           InitializeComponent();
        }
    }
}

