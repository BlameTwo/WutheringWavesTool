﻿using System;
using System.Diagnostics;
using System.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using Waves.Api.Models;
using Waves.Core.Common;
using Waves.Core.Models;
using Waves.Core.Models.Enums;

namespace Waves.Core.GameContext;

partial class GameContextBase
{
    public bool IsProdDownload { get; private set; }

    public void StartPredDownloadGame(
        string folder,
        WavesIndex index,
        List<Resource> resources,
        string version
    )
    {
        var downloadFolder = folder + "\\ProdDownload";
        _prodDowloadCTS = new();
        Directory.CreateDirectory(downloadFolder);
        Task.Run(async () =>
        {
            this.Cdn = index.Default.CdnList.OrderByDescending(p => p.P).LastOrDefault();
            if (Cdn == null)
                return;
            await this.GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.ProdDownloadFolderPath,
                downloadFolder
            );
            await this.GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.ProdDownloadVersion,
                index.Predownload.Version
            );
            this.ProdDownloadBaseUrl = Cdn.Url + index.Predownload.ResourcesBasePath;
            await this.ProdDownloadGameFiles(
                GameDownloadActionSource.ProdDownload,
                downloadFolder,
                resources,
                version
            );
        });
    }

    private void UpdateProdDownloadProgress(
        GameContextActionType type,
        int currentFile,
        int maxFile,
        long currentSize,
        long maxSize,
        double speed = 0.0,
        string speedString = "0MB/s"
    )
    {
        try
        {
            var progress = Math.Round((double)currentSize / maxSize * 100, 2);
            var averageSpeed = CalculateAverageSpeed(speed);
            // 计算剩余时间
            string remainingTimeString = "N/A";
            var speedValue = speed;
            if (averageSpeed > 0)
            {
                var remainingBytes = maxSize - currentSize;
                var remainingTime = TimeSpan.FromSeconds(remainingBytes / speedValue);
                remainingTimeString = $"{remainingTime:hh\\:mm\\:ss}";
            }

            this.gameContextProdOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs()
                {
                    Type = type,
                    CurrentFile = currentFile,
                    MaxFile = maxFile,
                    CurrentSize = currentSize,
                    MaxSize = maxSize,
                    Speed = speed,
                    SpeedString = speedString,
                    Progress = progress,
                    RemainingTime = remainingTimeString,
                }
            );
        }
        catch (Exception ex) { }
    }

    public void StopProdDownload()
    {
        this._prodDowloadCTS.Cancel();
        this.IsProdDownload = false;
    }

    private async Task ProdDownloadGameFiles(
        GameDownloadActionSource actionSource,
        string folder,
        List<Resource> resources,
        string version = ""
    )
    {
        try
        {
            if (_prodDowloadCTS.Token.IsCancellationRequested)
                return;
            if (this.IsProdDownload)
                return;
            this.IsProdDownload = true;
            this.gameContextOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs()
                {
                    Type = Models.Enums.GameContextActionType.ProdDownload,
                    CurrentFile = 0,
                    MaxFile = resources.Count,
                    Progress = 0.0,
                }
            );
            var list = resources.OrderByDescending(x => x.Size).ToArray();
            Array.Reverse(list);
            long maxSize = list.Sum(x => x.Size);
            var totalBytesRead = 0L;
            long nowFileSize = 0L;
            UpdateProdDownloadProgress(
                GameContextActionType.ProdDownload,
                0,
                resources.Count,
                0,
                maxSize,
                0
            );
            this.IsProdDownload = true;
            for (global::System.Int32 i = 0; i < list.Length; i++)
            {
                if (_prodDowloadCTS.Token.IsCancellationRequested)
                    return;
                string cachefile = "";
                cachefile = folder + list[i].Dest.Replace('/', '\\');
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(cachefile)!);
                var url = ProdDownloadBaseUrl + list[i].Dest;
                nowFileSize = 0;
                var request = new HttpRequestMessage()
                {
                    RequestUri = new(url),
                    Method = HttpMethod.Get,
                };
                FileStream fs = null;
                if (File.Exists(cachefile))
                {
                    var size = ReadFileSize(cachefile);
                    var md5 = GetFileMD5(cachefile, _prodDowloadCTS.Token);
                    if (md5 == "" || _prodDowloadCTS.Token.IsCancellationRequested)
                        return;
                    if (md5 == list[i].Md5)
                    {
                        var progresssValue = Math.Round(
                            Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                            2
                        );
                        UpdateProdDownloadProgress(
                            GameContextActionType.ProdDownload,
                            i,
                            list.Length,
                            totalBytesRead,
                            maxSize,
                            0
                        );
                        Debug.WriteLine(progresssValue);
                        totalBytesRead += size;
                        continue;
                    }
                    if (actionSource == GameDownloadActionSource.ProdDownload)
                    {
                        if (size > list[i].Size)
                        {
                            File.Delete(cachefile);
                            fs = new FileStream(cachefile, FileMode.Create, FileAccess.Write);
                        }
                        else
                        {
                            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(
                                size,
                                null
                            );
                            totalBytesRead += size;
                            fs = new FileStream(cachefile, FileMode.Append, FileAccess.Write);
                        }
                    }
                    else if (
                        actionSource == GameDownloadActionSource.Verify
                        || actionSource == GameDownloadActionSource.Update
                    )
                    {
                        File.Delete(cachefile);
                        fs = new FileStream(cachefile, FileMode.Create, FileAccess.Write);
                    }
                }
                else
                {
                    fs = new FileStream(cachefile, FileMode.Create, FileAccess.Write);
                }
                using (
                    var response = await HttpClientService.GameDownloadClient.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        _prodDowloadCTS.Token
                    )
                )
                {
                    if (_prodDowloadCTS.Token.IsCancellationRequested)
                        return;
                    response.EnsureSuccessStatusCode();
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    using (fs)
                    {
                        int bytesRead;
                        var startTime = DateTime.UtcNow;
                        var lastReportTime = DateTime.UtcNow;
                        var bytesSinceLastReport = 0L;
                        while (
                            (
                                bytesRead = await MaybeLimitAndReadAsync(
                                    responseStream,
                                    fs,
                                    IsLimitSpeed,
                                    Lock
                                )
                            ) > 0
                        )
                        {
                            if (_prodDowloadCTS.Token.IsCancellationRequested)
                            {
                                await fs.FlushAsync();
                                fs.Close();
                                await fs.DisposeAsync();
                                break;
                            }
                            totalBytesRead += bytesRead;
                            nowFileSize += bytesRead;
                            bytesSinceLastReport += bytesRead;
                            var currentTime = DateTime.UtcNow;
                            var reportInterval = TimeSpan.FromSeconds(1);
                            if (currentTime - lastReportTime >= reportInterval)
                            {
                                var speed =
                                    bytesSinceLastReport
                                    / (currentTime - lastReportTime).TotalSeconds;
                                var progresssValue = Math.Round(
                                    Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                                    2
                                );
                                lastReportTime = currentTime;
                                bytesSinceLastReport = 0;
                                UpdateDownloadProgress(
                                    GameContextActionType.ProdDownload,
                                    i,
                                    list.Length,
                                    totalBytesRead,
                                    maxSize,
                                    speed,
                                    $"{speed / 1024 / 1024:F2} MB"
                                );
                            }
                        }
                        var currentTime2 = DateTime.UtcNow;
                        var speed2 =
                            bytesSinceLastReport / (currentTime2 - lastReportTime).TotalSeconds;
                        var progresssValue2 = Math.Round(
                            Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                            2
                        );
                        UpdateProdDownloadProgress(
                            GameContextActionType.ProdDownload,
                            i,
                            list.Length,
                            totalBytesRead,
                            maxSize,
                            speed2,
                            $"{speed2 / 1024 / 1024:F2} MB"
                        );
                        await fs.FlushAsync();
                    }
                }
            }
            ;
            this.gameContextProdOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs()
                {
                    Type = Models.Enums.GameContextActionType.ProdDownload,
                    CurrentSize = totalBytesRead,
                    MaxSize = maxSize,
                    Speed = 0.0,
                    SpeedString = "0MB",
                    CurrentFile = list.Length,
                    MaxFile = list.Length,
                    Progress = Math.Round(
                        Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                        2
                    ),
                }
            );
            await this.GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.ProdDownloadFolderDone,
                true.ToString()
            );
            this.gameContextOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs() { Type = Models.Enums.GameContextActionType.None }
            );
            this.IsProdDownload = false;
        }
        catch (System.IO.IOException ioEx)
        {
            this.gameContextOutputDelegate?.Invoke(
                this,
                new() { Type = GameContextActionType.Error, ErrorMessage = "网络断开！" }
            );
            this.IsProdDownload = false;
            this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
        }
        catch (HttpRequestException httpex)
        {
            this.gameContextOutputDelegate?.Invoke(
                this,
                new() { Type = GameContextActionType.Error, ErrorMessage = httpex.Message }
            );
            this.IsProdDownload = false;
            this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
        }
        catch (Exception ex)
        {
            this.gameContextOutputDelegate?.Invoke(
                this,
                new() { Type = GameContextActionType.Error, ErrorMessage = ex.Message }
            );
            this.IsProdDownload = false;
            this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
        }
    }

    public async Task InstallProdGameResourceAsync(
        string folder,
        WavesIndex index,
        List<Resource> resources
    )
    {
        try
        {
            this.InstallProd = true;
            this._installProdCTS = new CancellationTokenSource();
            List<Resource> clearResource = new();
            this.gameContextOutputDelegate?.Invoke(
                this,
                new()
                {
                    Type = Models.Enums.GameContextActionType.Clear,
                    CurrentFile = 0,
                    MaxFile = resources.Count,
                    Progress = 0,
                    Speed = 0.0,
                    CurrentSize = 0.0,
                    MaxSize = 0.0,
                    SpeedString = "",
                }
            );
            for (int i = 0; i < resources.Count; i++)
            {
                if (_installProdCTS.IsCancellationRequested)
                    return;
                var cachefile = folder + "\\ProdDownload" + resources[i].Dest.Replace('/', '\\');
                var file = folder + resources[i].Dest.Replace('/', '\\');
                if (File.Exists(file))
                    File.Delete(file);
                Directory.CreateDirectory(Path.GetDirectoryName(file)!);
                File.Move(cachefile, file);
                var progress = Math.Round(Convert.ToDouble((double)i / resources.Count) * 100, 2);
                this.gameContextOutputDelegate?.Invoke(
                    this,
                    new()
                    {
                        Type = Models.Enums.GameContextActionType.Clear,
                        CurrentFile = i + 1,
                        MaxFile = resources.Count,
                        Progress = progress,
                        Speed = 0.0,
                        CurrentSize = 0.0,
                        MaxSize = 0.0,
                        SpeedString = "",
                    }
                );
            }
            await GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.GameLauncherBassProgram,
                $"{folder}\\Wuthering Waves.exe"
            );
            await GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.LocalGameResourceVersion,
                index.Default.ResourceChunk.LastVersion
            );
            await GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.LocalGameVersion,
                index.Default.Version
            );
            this.gameContextOutputDelegate?.Invoke(
                this,
                new()
                {
                    Type = Models.Enums.GameContextActionType.None,
                    CurrentFile = 0,
                    MaxFile = resources.Count,
                    Progress = 0.0,
                    Speed = 0.0,
                    CurrentSize = 0.0,
                    MaxSize = 0.0,
                    SpeedString = "",
                }
            );
            this.InstallProd = false;
        }
        catch (Exception)
        {
            return;
        }
    }
}