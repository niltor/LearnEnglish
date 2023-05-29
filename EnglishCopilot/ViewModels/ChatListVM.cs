using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.Input;

namespace EnglishCopilot.ViewModels;

public partial class ChatListVM : ObservableObject
{
    const string defaultLanguage = "en-US";

    [ObservableProperty]
    public ObservableCollection<ChatMessage> chatMessages;

    [ObservableProperty]
    string? recognitionText = string.Empty;

    readonly ITextToSpeech textToSpeech;
    readonly ISpeechToText speechToText;
    private readonly Locale locale;

    public ChatListVM(ITextToSpeech textToSpeech, ISpeechToText speechToText)
    {
        this.textToSpeech = textToSpeech;
        this.speechToText = speechToText;

        var locales = textToSpeech.GetLocalesAsync().Result;
        locale = locales.Where(l => l.Language == defaultLanguage).FirstOrDefault();
    }


    async Task TextToSpeech(string content, CancellationToken cancellationToken)
    {
        await textToSpeech.SpeakAsync(content, new()
        {
            Locale = locale,
            Pitch = 2,
            Volume = 1
        }, cancellationToken);
    }

    [RelayCommand(IncludeCancelCommand = true)]
    async Task Listen(CancellationToken cancellationToken)
    {
        const string beginSpeakingPrompt = "Begin speaking...";
        RecognitionText = beginSpeakingPrompt;
        var recognitionResult = await speechToText.ListenAsync(
                                            CultureInfo.GetCultureInfo(locale.Language ?? defaultLanguage),
                                            new Progress<string>(partialText =>
                                            {
                                                if (RecognitionText is beginSpeakingPrompt)
                                                {
                                                    RecognitionText = string.Empty;
                                                }

                                                RecognitionText += partialText + " ";
                                            }), cancellationToken);

        if (recognitionResult.IsSuccessful)
        {
            RecognitionText = recognitionResult.Text;
        }
        else
        {
            await Toast.Make(recognitionResult.Exception?.Message ?? "Unable to recognize speech").Show(CancellationToken.None);
        }

        if (RecognitionText is beginSpeakingPrompt)
        {
            RecognitionText = string.Empty;
        }
    }

}


public class ChatMessage
{
    public string UserName { get; set; }
    public string Message { get; set; }
}