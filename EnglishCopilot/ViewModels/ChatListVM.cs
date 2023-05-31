using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.Input;

namespace EnglishCopilot.ViewModels;

public partial class ChatListVM : ObservableObject
{
    const string defaultLanguage = "en-US";

    [ObservableProperty]
    public ObservableCollection<ChatMessage> chatMessages = new();

    [ObservableProperty]
    string? recognitionText = string.Empty;

    [ObservableProperty]
    public bool isListening = false;

    readonly ITextToSpeech textToSpeech;
    readonly ISpeechToText speechToText;
    private readonly Locale locale;
    private readonly string OpenAIKey;

    private OpenAIClient OpenAIClient { get; init; }

    public ChatListVM(ITextToSpeech textToSpeech, ISpeechToText speechToText)
    {
        this.textToSpeech = textToSpeech;
        this.speechToText = speechToText;

        InitData();

        var localeId = Preferences.Default.Get("LocaleId", string.Empty);
        var locales = textToSpeech.GetLocalesAsync().Result;
        if (string.IsNullOrWhiteSpace(localeId))
        {
            locale = locales?.OrderBy(x => x.Language).ThenBy(x => x.Name).FirstOrDefault();
        }
        else
        {
            locale = locales?.Where(x => x.Id == localeId).FirstOrDefault();
        }

        OpenAIKey = Preferences.Default.Get("OpenAIKey", string.Empty);
        OpenAIClient = new OpenAIClient(OpenAIKey);
    }


    public void InitData()
    {
        ChatMessages.Add(new ChatMessage
        {
            UserName = "You:",
            Message = "Hello, Just test message!"
        });

        ChatMessages.Add(new ChatMessage
        {
            UserName = "AI:",
            Message = "AI messages  test!"
        });
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
    private async Task SpeechToTextAsync(CancellationToken cancellationToken)
    {
        var message = new ChatMessage
        {
            Message = "test",
            UserName = "YOU:"
        };
        ChatMessages.Add(message);
        return;


        //if (!IsListening)
        //{
        //    if (!await CheckLocaleAsync()) return;
        //    IsListening = true;
        //}
        //else
        //{
        //    IsListening = false;
        //}

        //const string beginSpeakingPrompt = "Begin speaking...";
        //RecognitionText = beginSpeakingPrompt;
        //var recognitionResult = await speechToText.ListenAsync(
        //                                    CultureInfo.GetCultureInfo(locale.Language ?? defaultLanguage),
        //                                    new Progress<string>(partialText =>
        //                                    {
        //                                        if (RecognitionText is beginSpeakingPrompt)
        //                                        {
        //                                            RecognitionText = string.Empty;
        //                                        }

        //                                        RecognitionText += partialText + " ";
        //                                    }), cancellationToken);
        //if (recognitionResult.IsSuccessful)
        //{
        //    IsListening = false;
        //    RecognitionText = recognitionResult.Text;
        //    var message = new ChatMessage
        //    {
        //        Message = recognitionResult.Text,
        //        UserName = "YOU:"
        //    };
        //    ChatMessages.Add(message);

        //    var choices = await OpenAIClient.ResponseChatAsync(RecognitionText);
        //    var response = choices.FirstOrDefault()?.Message.Content;
        //    if (response != null)
        //    {
        //        await TextToSpeech(response, cancellationToken);
        //        var resMessage = new ChatMessage
        //        {
        //            Message = response,
        //            UserName = "Copilot:"
        //        };
        //        ChatMessages.Add(resMessage);
        //    }
        //}
        //else
        //{
        //    await Toast.Make(recognitionResult.Exception?.Message ?? "Unable to recognize speech").Show(CancellationToken.None);

        //    IsListening = false;
        //}
    }
    private async Task<bool> CheckLocaleAsync()
    {
        if (locale is null)
        {
            await Toast.Make("当前系统未安装英文语音包，请先安装").Show();
            return false;
        }
        return true;
    }
}


public class ChatMessage
{
    public string UserName { get; set; }
    public string Message { get; set; }
}