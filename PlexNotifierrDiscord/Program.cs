using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlexNotifierrDiscord.Models;
using PlexNotifierrDiscord.Services;
using Serilog;
using System.Reflection;
using LoggerExtensions = PlexNotifierrDiscord.Extensions.LoggerExtensions;

Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext().WriteTo.Console().CreateLogger();

var client = new DiscordShardedClient(new DiscordSocketConfig
{
    LogLevel = LogSeverity.Info,
    AlwaysDownloadUsers = true,
    MessageCacheSize = 1000
});

var commands = new CommandService(new CommandServiceConfig
{
    // Again, log level:
    LogLevel = LogSeverity.Info,

    // There's a few more properties you can set,
    // for example, case-insensitive commands.
    CaseSensitiveCommands = false
});

var host = Host.CreateDefaultBuilder()
               .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
               .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;
                    // Setup your DI container.
                    services.AddHostedService<MessageReceiver>();
                    services.AddSingleton(client);
                    services.AddSingleton(commands);
                    services.AddSingleton(config);
                    services.AddSingleton<ICommandHandler, CommandHandler>();
                    services.AddOptions<RabbitMqConfig>().Bind(hostContext.Configuration.GetSection("RabbitMQ"));
                })
               .Build();

await host.Services.GetRequiredService<ICommandHandler>().InitializeAsync();

client.Log += LoggerExtensions.LogAsync;
client.ShardReady += async shard => { Log.Information($"Shard Number {shard.ShardId} is connected and ready!"); };
var config = host.Services.GetRequiredService<IConfiguration>();
// Login and connect.
var token = config.GetRequiredSection("Discord")["DiscordBotToken"];
if (string.IsNullOrWhiteSpace(token))
{
    Log.Error("Token is null or empty");
    return;
}

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();
await host.RunAsync();