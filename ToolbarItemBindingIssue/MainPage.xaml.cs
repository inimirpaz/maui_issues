namespace ToolbarItemBindingIssue;

public partial class MainPage : ContentPage
{
    private bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        MainPageVM viewModel = (MainPageVM)BindingContext;
        viewModel.IsVisible = !viewModel.IsVisible;
        IsVisible = !IsVisible;
    }
}