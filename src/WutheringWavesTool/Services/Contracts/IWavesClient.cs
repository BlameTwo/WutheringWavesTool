﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Waves.Api.Models;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Communitys.DataCenter;
using Waves.Core.Contracts;
using WavesLauncher.Core.Models;

namespace WavesLauncher.Core.Contracts;

public interface IWavesClient
{
    public string Token { get; }

    public long Id { get; }

    public Task<bool> IsLoginAsync();

    public IHttpClientService HttpClientService { get; }

    public Task<GamerDataModel?> GetGamerDataAsync(
        GameRoilDataItem gamerRoil,
        CancellationToken token = default
    );
    public Task<GamerRoil?> GetWavesGamerAsync(CancellationToken token = default);

    public Task<SMSResultModel?> SendSMSAsync(
        string mobile,
        string geeTestData,
        CancellationToken token = default
    );

    public Task<SignIn?> GetSignInDataAsync(long userId, long roleId);

    public Task<AccountModel?> LoginAsync(
        string mobile,
        string code,
        CancellationToken token = default
    );

    public Task<SignRecord?> GetSignRecordAsync(string userId, string roldId);
    public Task<SignInResult?> SignInAsync(string userId, string roleId);
    public Task<AccountMine?> GetWavesMineAsync(long id, CancellationToken token = default);

    public Task<PlayerReponse?> GetPlayerReponseAsync(PlayerCard card);

    #region 数据终端
    Task<GamerBassData?> GetGamerBassDataAsync(
        GameRoilDataItem roil,
        CancellationToken token = default
    );
    Task<GamerRoleData?> GetGamerRoleDataAsync(
        GameRoilDataItem roil,
        CancellationToken token = default
    );

    Task<GamerCalabashData?> GetGamerCalabashDataAsync(
        GameRoilDataItem roil,
        CancellationToken token = default
    );

    Task<GamerTowerIndexData?> GetGamerTowerIndexDataAsync(
        GameRoilDataItem roil,
        CancellationToken token = default
    );

    Task<GamerExploreIndexData?> GetGamerExploreIndexDataAsync(
        GameRoilDataItem roil,
        CancellationToken token = default
    );

    Task<GamerChallengeIndexData?> GetGamerChallengeIndexDataAsync(
        GameRoilDataItem roil,
        CancellationToken token = default
    );

    Task<GamerDataBool?> RefreshGamerDataAsync(
        GameRoilDataItem roil,
        CancellationToken token = default
    );

    Task<GamerRoilDetily?> GetGamerRoilDetily(
        GameRoilDataItem roil,
        long roleId,
        CancellationToken token = default
    );
    #endregion
}