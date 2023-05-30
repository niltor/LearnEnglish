namespace EnglishCopilot.Pages;

public partial class SettingsPage : ContentPage
{

    SettingsVM SettingsVM;
    public SettingsPage(SettingsVM settingsVM)
    {
        InitializeComponent();
        BindingContext = SettingsVM = settingsVM;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        SettingsVM.InitData();
    }
}
