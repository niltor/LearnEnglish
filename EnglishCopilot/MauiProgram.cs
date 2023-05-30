using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Media;
using EnglishCopilot.Pages;
using Microsoft.Extensions.Logging;

namespace EnglishCopilot;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<ISpeechToText>(SpeechToText.Default);
        builder.Services.AddSingleton<ITextToSpeech>(TextToSpeech.Default);

        builder.Services.AddSingleton<ChatListVM>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<SettingsPage>();
        return builder.Build();
    }
}
