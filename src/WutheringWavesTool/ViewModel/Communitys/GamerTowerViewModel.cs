﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waves.Api.Models.Communitys;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Wrapper;

namespace WutheringWavesTool.ViewModel.Communitys;

public sealed partial class GamerTowerViewModel : ViewModelBase, IDisposable
{
    private bool disposedValue;

    public GamerTowerViewModel(IWavesClient wavesClient)
    {
        WavesClient = wavesClient;
    }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterTowerDifficultyWrapper> Difficulties { get; set; }

    public IWavesClient WavesClient { get; }
    public GameRoilDataItem RoilData { get; private set; }

    internal void SetData(GameRoilDataItem item)
    {
        this.RoilData = item;
    }

    [RelayCommand]
    async Task LoadedAsync()
    {
        var index = await WavesClient.GetGamerTowerIndexDataAsync(this.RoilData, this.CTS.Token);
        if (index == null)
            return;

        if (Difficulties != null)
        {
            Difficulties.Clear();
        }
        else
        {
            Difficulties = new();
        }
        foreach (var item in index.DifficultyList)
        {
            Difficulties.Add(new(item));
        }
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var item in this.Difficulties)
                {
                    foreach (var item2 in item.Areas)
                    {
                        foreach (var item3 in item2.Floors)
                        {
                            item3.Roles.RemoveAll();
                        }
                        item2.Floors.RemoveAll();
                    }
                    item.Areas.RemoveAll();
                }
                this.Difficulties.RemoveAll();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}