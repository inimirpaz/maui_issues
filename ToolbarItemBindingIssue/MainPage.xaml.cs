using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plugin.Maui.Calendar.Models;

namespace ToolbarItemBindingIssue;

public partial class MainPage : ContentPage
{
    public class EventModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler? changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }
        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(nameof(Description)); } }
    }

    public EventCollection MyEvents { get; set; } = new EventCollection();

    public MainPage()
    {
        MyEvents = new EventCollection
        {
            [DateTime.Now] = new List<EventModel>
            {
                new EventModel { Name = "Cool event1", Description = "This is Cool event1's description!" },
                new EventModel { Name = "Cool event2", Description = "This is Cool event2's description!" }
            },
            // 5 days from today
            [DateTime.Now.AddDays(5)] = new List<EventModel>
            {
                new EventModel { Name = "Cool event3", Description = "This is Cool event3's description!" },
                new EventModel { Name = "Cool event4", Description = "This is Cool event4's description!" }
            },
            // 3 days ago
            [DateTime.Now.AddDays(-3)] = new List<EventModel>
            {
                new EventModel { Name = "Cool event5", Description = "This is Cool event5's description!" }
            },
            // custom date
            [new DateTime(2020, 3, 16)] = new List<EventModel>
            {
                new EventModel { Name = "Cool event6", Description = "This is Cool event6's description!" }
            }
        };
        InitializeComponent();
    }
}