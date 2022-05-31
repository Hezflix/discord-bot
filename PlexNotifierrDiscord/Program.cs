using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlexNotifierrDiscord.Services;
using Serilog;


Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext().WriteTo.Console().CreateLogger();

var services = new ServiceCollection();
var config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();
var client = new DiscordShardedClient();


var commands = new CommandService(new CommandServiceConfig
{
    // Again, log level:
    LogLevel = LogSeverity.Info,

    // There's a few more properties you can set,
    // for example, case-insensitive commands.
    CaseSensitiveCommands = false
});

// Setup your DI container.
services.AddSingleton(client)
        .AddSingleton(commands)
        .AddSingleton(config)
        .AddSingleton<ICommandHandler, CommandHandler>();

await MainAsync();

async Task MainAsync()
{
    var service = services.BuildServiceProvider();
    await service.GetRequiredService<ICommandHandler>().InitializeAsync();

    client.Log += PlexNotifierrDiscord.Extensions.LoggerExtensions.LogAsync;
    client.ShardReady += async shard => { Log.Information($"Shard Number {shard.ShardId} is connected and ready!"); };

    // Login and connect.
    var token = config.GetRequiredSection("Discord")["DiscordBotToken"];
    if (string.IsNullOrWhiteSpace(token))
    {
        Log.Error("Token is null or empty");
        return;
    }

    await client.LoginAsync(TokenType.Bot, token);
    await client.StartAsync();

    // Wait infinitely so your bot actually stays connected.
    await Task.Delay(Timeout.Infinite);
}