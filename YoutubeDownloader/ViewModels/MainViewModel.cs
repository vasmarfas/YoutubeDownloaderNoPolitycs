using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using YoutubeDownloader.Core.Downloading;
using YoutubeDownloader.Framework;
using YoutubeDownloader.Services;
using YoutubeDownloader.Utils;
using YoutubeDownloader.Utils.Extensions;
using YoutubeDownloader.ViewModels.Components;

namespace YoutubeDownloader.ViewModels;

public partial class MainViewModel(
    ViewModelManager viewModelManager,
    DialogManager dialogManager,
    SettingsService settingsService
) : ViewModelBase
{
    public string Title { get; } = $"{Program.Name} v{Program.VersionString}";

    public DashboardViewModel Dashboard { get; } = viewModelManager.CreateDashboardViewModel();

    private async Task ShowDevelopmentBuildMessageAsync()
    {
        if (!Program.IsDevelopmentBuild)
            return;

        // If debugging, the user is likely a developer
        if (Debugger.IsAttached)
            return;

        var dialog = viewModelManager.CreateMessageBoxViewModel(
            "Unstable build warning",
            $"""
            You're using a development build of {Program.Name}. These builds are not thoroughly tested and may contain bugs.

            Auto-updates are disabled for development builds. If you want to switch to a stable release, please download it manually.
            """,
            "SEE RELEASES",
            "CLOSE"
        );

        if (await dialogManager.ShowDialogAsync(dialog) == true)
            ProcessEx.StartShellExecute(Program.ProjectReleasesUrl);
    }

    private async Task ShowFFmpegMessageAsync()
    {
        if (FFmpeg.IsAvailable())
            return;

        var dialog = viewModelManager.CreateMessageBoxViewModel(
            "FFmpeg is missing",
            $"""
            FFmpeg is required for {Program.Name} to work. Please download it and make it available in the application directory or on the system PATH.

            Alternatively, you can also download a version of {Program.Name} that has FFmpeg bundled with it.

            Click DOWNLOAD to go to the FFmpeg download page.
            """,
            "DOWNLOAD",
            "CLOSE"
        );

        if (await dialogManager.ShowDialogAsync(dialog) == true)
            ProcessEx.StartShellExecute("https://ffmpeg.org/download.html");

        if (Application.Current?.ApplicationLifetime?.TryShutdown(3) != true)
            Environment.Exit(3);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await ShowDevelopmentBuildMessageAsync();
        await ShowFFmpegMessageAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Save settings
            settingsService.Save();

            // Finalize pending updates
        }

        base.Dispose(disposing);
    }
}
