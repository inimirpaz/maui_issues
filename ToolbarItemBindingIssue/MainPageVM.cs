using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Helpers;

namespace ToolbarItemBindingIssue;

public class MainPageVM : INotifyPropertyChanged
{
    private bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

    public ICommand MyCommand { get; private set; }

    public MainPageVM()
    {
        MyCommand = new Command(async (obj) =>
        {
            IsVisible = false;
            System.Diagnostics.Debug.WriteLine("CLICKED", "TEST");
            await Task.Delay(2000);
            IsVisible = true;
        }, (obj) => IsVisible);

        //MyCommand = new AsyncCommand(async () =>
        //{
        //    IsVisible = false;
        //    await Task.Delay(2000);
        //    IsVisible = true;
        //}, (obj) => IsVisible);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChangedEventHandler? changed = PropertyChanged;
        if (changed == null)
            return;
        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}