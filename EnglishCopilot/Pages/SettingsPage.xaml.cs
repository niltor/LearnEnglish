namespace EnglishCopilot.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsVM settingsVM)
    {
        InitializeComponent();
        BindingContext = settingsVM;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
    }
}
