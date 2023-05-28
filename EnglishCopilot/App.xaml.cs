namespace EnglishCopilot;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        // weather current OS is android
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            window.MaximumWidth = 1080;
            window.Height = 1080;
            window.Width = 540;
        }

        return window;
    }
}
