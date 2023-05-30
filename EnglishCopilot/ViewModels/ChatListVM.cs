using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.Input;
using EnglishCopilot.Services;

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
    private readonly string OpenAIKey;
    private OpenAIClient OpenAIClient { get; init; }

    public ChatListVM(ITextToSpeech textToSpeech, ISpeechToText speechToText)
    {
        this.textToSpeech = textToSpeech;
        this.speechToText = speechToText;

        var locales = textToSpeech.GetLocalesAsync().Result;
        locale = locales.Where(l => l.Language == defaultLanguage).FirstOrDefault();
        OpenAIKey = Preferences.Get("OpenAIKey", string.Empty);
        OpenAIClient = new OpenAIClient(OpenAIKey);
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
            var message = new ChatMessage
            {
                Message = recognitionResult.Text,
                UserName = "you"
            };
            ChatMessages.Add(message);

            var choices = await OpenAIClient.ResponseChatAsync(RecognitionText);
            var response = choices.FirstOrDefault()?.Message.Content;
            if (response != null)
            {
                await TextToSpeech(response, cancellationToken);
                var resMessage = new ChatMessage
                {
                    Message = response,
                    UserName = "Copilot"
                };
                ChatMessages.Add(resMessage);
            }
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