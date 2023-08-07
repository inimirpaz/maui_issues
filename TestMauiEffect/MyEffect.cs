using Microsoft.Maui.Controls.Platform;
#if ANDROID
using Android.Views;
#elif IOS || MACCATALYST
using UIKit;
using Foundation;
#endif

namespace TestMauiEffect;

public class MyEffect : RoutingEffect
{
    internal const string ColorPropertyName = "Color";

    public static readonly BindableProperty ColorProperty = BindableProperty.CreateAttached(
        ColorPropertyName,
        typeof(Color),
        typeof(MyEffect),
        Colors.Magenta);

    public static Color GetColor(BindableObject bindable)
        => (Color)bindable.GetValue(ColorProperty);

    public static void SetColor(BindableObject bindable, Color value)
        => bindable.SetValue(ColorProperty, value);

    public MyEffect()
        : base("TestMauiEffect.MyEffect")
    {
    }
}

public class MyPlatformEffect : PlatformEffect
{
#if ANDROID
    Android.Views.View View => Control ?? Container;

    protected override void OnAttached()
    {
    }

    protected override void OnDetached()
    {
    }
#elif IOS || MACCATALYST
    UIView? View
    {
        get
        {
            var view = Control ?? Container;
            return Element is Frame ? view?.Subviews.FirstOrDefault() ?? view : view;
        }
    }

    protected override void OnAttached()
    {
    }

    protected override void OnDetached()
    {
    }
#endif
}