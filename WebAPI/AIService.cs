using Application.Services;
using Microsoft.CognitiveServices.Speech;

namespace WebAPI;

/// <summary>
/// AI服务
/// </summary>
public class AIService
{
    private readonly IConfiguration _configuration;

    private readonly ILogger<AIService> _logger;
    private readonly string? AzureSpeech;
    private static readonly string Region = "eastasia";
    private readonly OpenAIClient openAIClient;

    public AIService(IConfiguration configuration, ILogger<AIService> logger, OpenAIClient openAIClient)
    {
        _configuration = configuration;
        _logger = logger;
        AzureSpeech = _configuration.GetValue<string>("Key:AzureSpeech");
        this.openAIClient = openAIClient;
    }


    /// <summary>
    /// 语音流转文本
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async Task<string?> SpeechToTextAsync(Stream stream)
    {
        if (!CheckKey()) return null;
        var text = "";

        var config = SpeechConfig.FromSubscription(AzureSpeech, Region);
        var stopRecognition = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        BinaryReader reader = new BinaryReader(stream);

        //var audioConfig = new AudioConfig() { AudioProcessingOptions = AudioProcessingOptions.};

        // create audioconfig 

        using (var audioInput = Helper.OpenWavFile(reader))
        {
            // Creates a speech recognizer using audio stream input.
            using (var recognizer = new SpeechRecognizer(config, audioInput))
            {
                // Subscribes to events.
                recognizer.Recognizing += (s, e) =>
                {
                    _logger.LogInformation($"RECOGNIZING: Text={e.Result.Text}");
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        text = e.Result.Text;
                        _logger.LogInformation($"RECOGNIZED: Text={e.Result.Text}");
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        _logger.LogInformation($"NOMATCH: Speech could not be recognized.");
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    _logger.LogInformation($"CANCELED: Reason={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        _logger.LogInformation($"CANCELED: ErrorCode={e.ErrorCode}");
                        _logger.LogInformation($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    }
                    stopRecognition.TrySetResult(0);
                };

                recognizer.SessionStarted += (s, e) =>
                {
                    _logger.LogInformation("\nSession started event.");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    _logger.LogInformation("\nSession stopped event.");
                    _logger.LogInformation("\nStop recognition.");
                    stopRecognition.TrySetResult(0);
                };
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                // Waits for completion.
                // Use Task.WaitAny to keep the task rooted.
                Task.WaitAny(new[] { stopRecognition.Task });
                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }
        }
        return text;
    }

    /// <summary>
    /// 对话内容
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public async Task<string?> ChatResponseAsync(string content)
    {
        var choices = await openAIClient.ResponseChatAsync(content);
        return choices?.First().Message.Content;
    }

    public void TextToSpeech()
    {

    }

    private bool CheckKey()
    {
        if (string.IsNullOrWhiteSpace(AzureSpeech))
        {
            Console.WriteLine("openaikey or azurespeech key is null");
            return false;

        }
        return true;
    }

}
