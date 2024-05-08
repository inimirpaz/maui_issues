using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Helpers;

namespace ToolbarItemBindingIssue;

public class MainPageVM : INotifyPropertyChanged
{
    private bool _isVisible = false;
    public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

    public ICommand GridCommand { get; private set; }

    public MainPageVM()
    {
        GridCommand = new AsyncCommand(async () =>
        {
            IsVisible = true;
            await Task.Delay(3000);
            IsVisible = false;
        });
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