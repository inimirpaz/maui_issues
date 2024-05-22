using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XamarinToolbarItemBindingIssue
{
    public class MainPageVM : INotifyPropertyChanged
    {
        private bool _isVisible = false;
        public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

        public ObservableCollection<MyModel> Items { get; private set; } = new()
        {
            new(){ Title ="Title #1", Subtitle= "Subtitle" },
            new(){ Title ="Title #2", Subtitle= "Subtitle" },
            new(){ Title ="Title #3", Subtitle= "Subtitle" }
        };

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

        public class MyModel
        {
            public string Title { get; set; }
            public string Subtitle { get; set; }
        }
    }
}