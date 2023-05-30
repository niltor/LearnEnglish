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

        builder.Services.AddSingleton(SpeechToText.Default);
        builder.Services.AddSingleton(TextToSpeech.Default);

        builder.Services.AddTransientWithShellRoute<SettingsPage, SettingsVM>(nameof(SettingsPage));
        builder.Services.AddTransient<MainPage, ChatListVM>();
        return builder.Build();
    }
}
