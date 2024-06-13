using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToolbarItemBindingIssue;

public class MainPageVM : INotifyPropertyChanged
{
    private bool _isVisible = false;
    public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

    public MainPageVM()
    {
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