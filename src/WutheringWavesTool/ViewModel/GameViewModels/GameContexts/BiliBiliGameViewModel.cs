﻿using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public partial class BiliBiliGameViewModel : GameContextViewModelBase
{
    public BiliBiliGameViewModel(
        [FromKeyedServices(nameof(BiliBiliGameContext))] IGameContext gameContext,
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager,
        IAppContext<App> appContext
    )
        : base(gameContext, dialogManager, appContext) { }

    [ObservableProperty]
    public partial ObservableCollection<Slideshow> SlideShows { get; set; }

    #region Datas
    public Notice Notice { get; private set; }
    public News News { get; private set; }
    public Waves.Api.Models.Activity Activity { get; private set; }
    #endregion

    [ObservableProperty]
    public partial bool IsOpen { get; set; } = true;

    [ObservableProperty]
    public partial bool TabLoad { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Content> Contents { get; set; } = new();

    public override Task LoadAfter()
    {
        return Task.CompletedTask;
    }

    public override void DisposeAfter() { }
}
