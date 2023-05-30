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
        OpenAIKey = Preferences.Get("OpenAIKey", string.Empty);
        var localeId = Preferences.Get("LocaleId", string.Empty);

        this.textToSpeech = textToSpeech;
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
    public void SaveSetting()
    {
        Preferences.Set("OpenAIKey", OpenAIKey);
        Preferences.Set("LocaleId", Locale.Id);

    }
}
