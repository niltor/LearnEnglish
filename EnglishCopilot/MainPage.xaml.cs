using EnglishCopilot.Pages;

namespace EnglishCopilot;

public partial class MainPage : ContentPage
{
    private readonly string AzureKey;
    private readonly string OpenAIKey;

    public MainPage(ChatListVM chatListVM)
    {
        InitializeComponent();
        BindingContext = chatListVM;
        AzureKey = Preferences.Get("AzureKey", string.Empty);
        OpenAIKey = Preferences.Get("OpenAIKey", string.Empty);
    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SettingsPage());
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await CheckAndRequestMicrophonePermission();
    }

    /// <summary>
    /// mic permission
    /// </summary>
    /// <returns></returns>
    private async Task<PermissionStatus> CheckAndRequestMicrophonePermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status == PermissionStatus.Granted)
        {
            return status;
        }
        if (Permissions.ShouldShowRationale<Permissions.Microphone>())
        {
            // Prompt the user with additional information as to why the permission is needed
        }
        status = await Permissions.RequestAsync<Permissions.Microphone>();
        return status;
    }

}

