﻿namespace WutheringWavesTool.Services.Contracts;

public interface IGamerRoilContext
{
    void SetScope(IServiceScope scope);

    INavigationService NavigationService { get; }
}
