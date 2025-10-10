using IMS_Mobile.Service;
using Microsoft.Maui.Controls;

namespace IMS_Mobile.Controls;

public partial class AdBanner : ContentView
{
    public AdBanner()
    {
        InitializeComponent();
#if !ANDROID
        IsVisible = false; // extra safety on non-Android
#endif
        Loaded += (_, __) => ApplyAdUnitId();
        BindingContext = this;
    }


    public bool IsVisibleOnLoad { get; set; } = NetworkHelper.IsConnected();
    // Optional: set this when you switch from test to real ads.
    // If left null/empty AND AdConfig.UseTestAdUnitIds=true in MauiProgram.cs,
    // the plugin uses Google's test units automatically.
    public static readonly BindableProperty AdUnitIdProperty =
        BindableProperty.Create(
            nameof(AdUnitId),
            typeof(string),
            typeof(AdBanner),
            default(string),
            propertyChanged: OnAdUnitIdChanged);

    public string AdUnitId
    {
        get => (string)GetValue(AdUnitIdProperty);
        set => SetValue(AdUnitIdProperty, value);
    }

    static void OnAdUnitIdChanged(BindableObject bindable, object oldValue, object newValue)
        => ((AdBanner)bindable).ApplyAdUnitId();

    void ApplyAdUnitId()
    {
#if ANDROID
        Banner.AdUnitId = string.IsNullOrWhiteSpace(AdUnitId) ? null : AdUnitId;
#endif
    }
}
