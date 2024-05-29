using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinToolbarItemBindingIssue
{
    public partial class MainPage : ContentPage
    {
        public ICommand SelectedCommand { get; private set; }

        public MainPage()
        {
            SelectedCommand = new Command((c) =>
            {
                if (collView.SelectedItem != null)
                {
                    if (collView.SelectedItem is MainPageVM.MyModel item)
                    {
                        System.Diagnostics.Debug.WriteLine(item.Title, "INFO");
                        collView.SelectedItem = null;
                    }
                }
            }, s => !IsBusy);

            InitializeComponent();
        }
    }
}

