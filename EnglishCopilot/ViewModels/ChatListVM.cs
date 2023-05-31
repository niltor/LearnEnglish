using System.Collections.Concurrent;
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
    public ObservableCollection<ChatMessage> chatMessages = new();
    [ObservableProperty]
    string? recognitionText = string.Empty;
    [ObservableProperty]
    public bool isListening = false;

    private bool StartConvert = false;
    private readonly ITextToSpeech textToSpeech;
    private readonly ISpeechToText speechToText;
    private readonly Locale? locale;
    private readonly string OpenAIKey;

    private ConcurrentQueue<string> SpeechTexts { get; set; } = new();

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

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task SpeechToTextAsync(CancellationToken cancellationToken)
    {
        // 开启转换监听线程
        if (!StartConvert)
        {
            _ = Task.Run(GetResponseAsync).ConfigureAwait(false);
            StartConvert = true;
        }
        if (!IsListening)
        {
            if (!await CheckLocaleAsync()) return;
            IsListening = true;
        }
        else
        {
            IsListening = false;
        }
        var sentence = string.Empty;
        var sentenceSigns = new[] { '.', '?', '!' };
        var recognitionResult = await speechToText.ListenAsync(
                                            CultureInfo.GetCultureInfo(locale?.Language ?? defaultLanguage),
                                            new Progress<string>(partialText =>
                                            {
                                                sentence += partialText;
                                                if (sentenceSigns.Any(s =>
                                                    partialText.Contains(s)) || sentence.Length > 20)
                                                {
                                                    SpeechTexts.Enqueue(sentence);
                                                    sentence = string.Empty;
                                                }
                                            }), cancellationToken);
        if (recognitionResult.IsSuccessful)
        {
            IsListening = false;
        }
        else
        {
            await Toast.Make(recognitionResult.Exception?.Message ?? "Unable to recognize speech").Show(CancellationToken.None);

            IsListening = false;
        }
    }

    /// <summary>
    /// 处理队列，并获取对话
    /// </summary>
    /// <returns></returns>
    private async Task GetResponseAsync()
    {
        while (SpeechTexts.TryDequeue(out var content))
        {
            var message = new ChatMessage
            {
                Message = content,
                UserName = "YOU:"
            };
            ChatMessages.Add(message);

            var choices = await OpenAIClient.ResponseChatAsync(content);
            var response = choices.FirstOrDefault()?.Message.Content;
            if (response != null)
            {
                _ = TextToSpeech(response, CancellationToken.None);
                var resMessage = new ChatMessage
                {
                    Message = response,
                    UserName = "Copilot:"
                };
                ChatMessages.Add(resMessage);
            }
            Thread.Sleep(100);
        }
    }

    /// <summary>
    /// 文本转语音
    /// </summary>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task TextToSpeech(string content, CancellationToken cancellationToken)
    {
        await textToSpeech.SpeakAsync(content, new()
        {
            Locale = locale,
            Pitch = 2,
            Volume = 1
        }, cancellationToken);
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