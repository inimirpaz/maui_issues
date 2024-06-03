using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ToolbarItemBindingIssue;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    public MainActivity()
    {
        int res_id = Resource.Mipmap.ic_pin_ok;
    }
}

