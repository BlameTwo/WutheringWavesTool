﻿using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using WutheringWavesTool.Helpers;

namespace WutheringWavesTool.Services;

public class WallpaperService : IWallpaperService
{
    public WallpaperService(ITipShow tipShow)
    {
        TipShow = tipShow;
    }

    public string BaseFolder { get; private set; }
    public ImageEx ImageHost { get; private set; }
    public ITipShow TipShow { get; }
    public string NowHexValue { get; private set; }

    public void RegisterHostPath(string folder)
    {
        this.BaseFolder = folder;
    }

    public async Task RegisterImageHostAsync(ImageEx image)
    {
        this.ImageHost = image;
        if (!string.IsNullOrWhiteSpace(AppSettings.WallpaperPath))
        {
            await this.SetWrallpaper(AppSettings.WallpaperPath);
        }
        else
        {
            await this.SetWrallpaper(
                AppDomain.CurrentDomain.BaseDirectory + "Assets/Images/changli.png"
            );
        }
    }

    public async Task<bool> SetWrallpaper(string path)
    {
        var result = await ImageIOHelper.HexImageAsync(this.BaseFolder, path);
        if (result.Item1 != null)
        {
            this.ImageHost.Source = result.Item1;
            this.NowHexValue = result.Item3!;
            AppSettings.WallpaperPath = result.Item2;
            return true;
        }
        else
        {
            TipShow.ShowMessage(result.Item2, Microsoft.UI.Xaml.Controls.Symbol.Pictures);
            return false;
        }
    }

    public async IAsyncEnumerable<WallpaperModel> GetFilesAsync(
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        List<WallpaperModel> models = new();
        var folder = new DirectoryInfo(this.BaseFolder);
        using (MD5 md5 = MD5.Create())
        {
            var files = Directory
                .GetFiles(this.BaseFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s =>
                    s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                    || s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                );
            foreach (var item in files)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                using (
                    var stream = new FileStream(
                        item,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 4096,
                        useAsync: true
                    )
                )
                {
                    byte[] hashBytes = await md5.ComputeHashAsync(stream);
                    var md5Value = BitConverter
                        .ToString(hashBytes)
                        .Replace("-", "")
                        .ToLowerInvariant();
                    yield return new()
                    {
                        FilePath = item,
                        Image = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new(item)),
                        Md5String = md5Value,
                    };
                }
            }
        }
    }
}
