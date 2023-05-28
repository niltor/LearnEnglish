﻿using System.Text;

using EnglishCopilot.Pages;

using Microsoft.CognitiveServices.Speech;

namespace EnglishCopilot;

public partial class MainPage : ContentPage
{

    public MainPage(ChatListVM chatListVM)
    {
        InitializeComponent();
        BindingContext = chatListVM;
    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SettingsPage());
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await CheckAndRequestMicrophonePermission();
    }

    private async Task<PermissionStatus> CheckAndRequestMicrophonePermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status == PermissionStatus.Granted)
        {
            return status;
        }
        if (Permissions.ShouldShowRationale<Permissions.Microphone>())
        {
            // Prompt the user with additional information as to why the permission is needed
        }
        status = await Permissions.RequestAsync<Permissions.Microphone>();
        return status;
    }

    private async void OnRecognitionButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");

            using (var recognizer = new SpeechRecognizer(config))
            {
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query.
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                StringBuilder sb = new StringBuilder();
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    sb.AppendLine($"RECOGNIZED: Text={result.Text}");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    sb.AppendLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    sb.AppendLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        sb.AppendLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        sb.AppendLine($"CANCELED: Did you update the subscription info?");
                    }
                }

                var message = new ChatMessage
                {
                    UserName = "You",
                    Message = sb.ToString()
                };
                // update chatlisvm with new message
                var chatListVM = BindingContext as ChatListVM;
                chatListVM.ChatMessages.Add(message);

            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Exception: " + ex.ToString());
        }
    }

}

