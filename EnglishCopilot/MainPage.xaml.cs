﻿using EnglishCopilot.Pages;

namespace EnglishCopilot;

public partial class MainPage : ContentPage
{

    private readonly ChatListVM viewModel;
    public MainPage(ChatListVM chatListVM)
    {
        InitializeComponent();
        BindingContext = viewModel = chatListVM;
    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {
        // navigation to settingsPage
        Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await CheckAndRequestMicrophonePermission();
        _ = viewModel.StartTransformAsync();
    }

    /// <summary>
    /// mic permission
    /// </summary>
    /// <returns></returns>
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

}

