using System.IO;
using YoutubeExplodeNoPolytics.Common;

namespace YoutubeDownloader.Core.Utils.Extensions;

public static class YoutubeExtensions
{
    public static string? TryGetImageFormat(this Thumbnail thumbnail) =>
        Url.TryExtractFileName(thumbnail.Url)?.Pipe(Path.GetExtension)?.Trim('.');
}
