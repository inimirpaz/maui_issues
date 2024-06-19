using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ToolbarItemBindingIssue;

public class GroupedObservableCollection<T> : ObservableCollection<T>
{
    public string Title { get; }
    public GroupedObservableCollection(string title, List<T> observableCollection) : base(observableCollection)
    {
        Title = title;
    }
}

public class MyClass : ABaseViewModel
{
    private string _name;
    public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }
}

public abstract class ABaseViewModel : INotifyPropertyChanged
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

public class MainPageVM : ABaseViewModel
{
    public ObservableCollection<GroupedObservableCollection<MyClass>> MyGroupedItems { get; private set; }
        = new ObservableCollection<GroupedObservableCollection<MyClass>>();

    public ICommand SelectCommand { get; private set; }
    private const string BUCKET_NAME = "Bucket #3";

    public MainPageVM()
    {
        SelectCommand = new Command<MyClass>((el) =>
        {
            if (MyGroupedItems.Count(x => x.Title == BUCKET_NAME) < 1)
            {
                MyGroupedItems.Add(new GroupedObservableCollection<MyClass>(BUCKET_NAME, new List<MyClass>()));
            }
            var b = MyGroupedItems.First(x => x.Title == BUCKET_NAME);

            foreach (var a in MyGroupedItems)
            {
                if (a.Contains(el))
                {
                    if (a.Title != BUCKET_NAME)
                    {
                        a.Remove(el);
                        b.Insert(0, el);
                    }
                    else
                    {
                        a.Remove(el);
                        MyGroupedItems[0].Insert(0, el);
                    }
                    break;
                }
            }
        });

        MyGroupedItems.Add(new GroupedObservableCollection<MyClass>("Main Container", new List<MyClass>()
        {
            new MyClass { Name = "Element 1" },
            new MyClass { Name = "Element 2" },
            new MyClass { Name = "Element 3" },
            new MyClass { Name = "Element 4" },
            new MyClass { Name = "Element 5" },
        }));
    }
}