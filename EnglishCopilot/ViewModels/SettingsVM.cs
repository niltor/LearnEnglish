using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace EnglishCopilot.ViewModels;
public partial class SettingsVM : ObservableObject
{
    readonly ITextToSpeech textToSpeech;

    [ObservableProperty]
    Locale? locale;

    [ObservableProperty]
    string? openAIKey = string.Empty;

    public ObservableCollection<Locale> Locales { get; } = new();

    public SettingsVM(ITextToSpeech textToSpeech)
    {
        this.textToSpeech = textToSpeech;

    }

    public void InitData()
    {
        OpenAIKey = Preferences.Default.Get("OpenAIKey", string.Empty);
        var localeId = Preferences.Default.Get("LocaleId", string.Empty);

        var locales = textToSpeech.GetLocalesAsync().Result;
        foreach (var locale in locales.OrderBy(x => x.Language).ThenBy(x => x.Name))
        {
            Locales.Add(locale);
        }

        if (!string.IsNullOrWhiteSpace(localeId))
        {
            Locale = Locales.Where(x => x.Id == localeId).FirstOrDefault();
        }
        else
        {
            Locale = Locales.FirstOrDefault();
        }
    }

    [RelayCommand]
    public async Task SaveSettingAsync()
    {
        Preferences.Default.Set("OpenAIKey", OpenAIKey);
        Preferences.Default.Set("LocaleId", Locale.Id);

        await Shell.Current.DisplayAlert("Alert", "保存成功", "ok");

    }
}
