using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiIssuesXam
{
    public class MainPageVM : ABaseVM
    {
        public ObservableCollection<string> Strings { get; private set; } = new() {
        "",
        "",
        "",
        "",
        "",
        "",
        "",
    };

        public MainPageVM()
        {
        }
    }

    public abstract class ABaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler? changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

