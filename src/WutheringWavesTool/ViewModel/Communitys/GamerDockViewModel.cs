﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Communitys.DataCenter;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Wrapper;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.Communitys;

public partial class GamerDockViewModel : ViewModelBase, IDisposable
{
    private bool disposedValue;

    public IWavesClient WavesClient { get; }
    public ITipShow TipShow { get; }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterPhantomItemWrapper> GamerPhantoms { get; set; }

    [ObservableProperty]
    public partial GamerCalabashData GamerCalabash { get; set; }
    public GameRoilDataItem GameRoil { get; private set; }

    public GamerDockViewModel(IWavesClient wavesClient, ITipShow tipShow)
    {
        WavesClient = wavesClient;
        TipShow = tipShow;
    }

    private async Task RefreshDataAsync(GameRoilDataItem item)
    {
        this.GameRoil = item;
        var calabash = await WavesClient.GetGamerCalabashDataAsync(GameRoil);
        if (calabash == null)
        {
            TipShow.ShowMessage("未请求到数据坞信息", Microsoft.UI.Xaml.Controls.Symbol.Clear);
        }
        else
        {
            this.GamerCalabash = calabash;
            this.GamerPhantoms = FormatData(GamerCalabash);
        }
    }

    private ObservableCollection<DataCenterPhantomItemWrapper> FormatData(
        GamerCalabashData gamerCalabash
    )
    {
        ObservableCollection<DataCenterPhantomItemWrapper> items = new();
        foreach (var item in gamerCalabash.PhantomList)
        {
            items.Add(new DataCenterPhantomItemWrapper() { BassData = item });
        }
        return items;
    }

    public async Task SetDataAsync(GameRoilDataItem item)
    {
        await RefreshDataAsync(item);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                GamerPhantoms.RemoveAll();
                this.CTS.Cancel();
            }
            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~GamerDockViewModel()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}