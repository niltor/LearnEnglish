namespace EnglishCopilot.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        AzureKey.Text = Preferences.Get("AzureKey", string.Empty);
        OpenAIKey.Text = Preferences.Get("OpenAIKey", string.Empty);
    }

    private void SaveSetting(object sender, EventArgs e)
    {
        Preferences.Set("AzureKey", AzureKey.Text);
        Preferences.Set("OpenAIKey", OpenAIKey.Text);
    }
}