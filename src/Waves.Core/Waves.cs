﻿using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Waves.Core.Contracts;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using Waves.Core.Models.Handlers;
using Waves.Core.Services;

namespace Waves.Core;

public static class Waves
{
    /// <summary>
    /// 注入游戏上下文，注意已包含HttpClientFactory
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddGameContext(this IServiceCollection services)
    {
        StaticConfig.EnableAot = true;
        services
            .AddTransient<IHttpClientService, HttpClientService>()
            .AddKeyedSingleton<IGameContext, MainGameContext>(
                nameof(MainGameContext),
                (provider, c) =>
                {
                    var context = GameContextFactory.GetMainGameContext();
                    context.HttpClientService = provider.GetRequiredService<IHttpClientService>();
                    context.InitAsync().GetAwaiter().GetResult();
                    return context;
                }
            )
            .AddKeyedSingleton<IGameContext, BilibiliGameContext>(
                nameof(BilibiliGameContext),
                (provider, c) =>
                {
                    var context = GameContextFactory.GetBilibiliGameContext();
                    context.HttpClientService = provider.GetRequiredService<IHttpClientService>();
                    context.InitAsync().GetAwaiter().GetResult();
                    return context;
                }
            )
            .AddKeyedSingleton<IGameContext, GlobalGameContext>(
                nameof(GlobalGameContext),
                (provider, c) =>
                {
                    var context = GameContextFactory.GetGlobalGameContext();
                    context.HttpClientService = provider.GetRequiredService<IHttpClientService>();
                    context.InitAsync().GetAwaiter().GetResult();
                    return context;
                }
            )
            .AddTransient<IHttpClientService, HttpClientService>();
        services.AddHttpClient();
        return services;
    }
}