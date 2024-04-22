using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToolbarItemBindingIssue;

public class MainPageVM : INotifyPropertyChanged
{
    private bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

    public MainPageVM()
    {
    }

#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChangedEventHandler changed = PropertyChanged;
        if (changed == null)
            return;
        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
#pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
}