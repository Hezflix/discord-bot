using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PlexNotifierrDiscord.Services;

namespace PlexNotifierrDiscord.Modules
{
    public class SubscribeManagementCommands : ModuleBase<ShardedCommandContext>
    {
        private readonly ILocalHandler _localHandler;
        private readonly IPlexNotifierrApi _plexNotifierr;
        private ILogger<SubscribeManagementCommands> _logger;

        public SubscribeManagementCommands(ILocalHandler localHandler, IPlexNotifierrApi plexNotifierr, ILogger<SubscribeManagementCommands> logger)
        {
            _localHandler = localHandler;
            _plexNotifierr = plexNotifierr;
            _logger = logger;
        }

        [Command("subscribe", RunMode = RunMode.Async)]
        public async Task? SubscribeWithoutPlexName()
        {
            await Context.Message.ReplyAsync(_localHandler.GetLocales().SubscribeNoPlex);
        }

        [Command("subscribe", RunMode = RunMode.Async)]
        public async Task Subscribe(string plexName)
        {
            _logger.LogInformation($"User {Context.User.Username} subscribe on plex user {plexName}");
            if (await _plexNotifierr.Subscribe(Context.User.Id, plexName))
            {
                await Context.Message.ReplyAsync(_localHandler.GetLocales().SubscribeSuccess);
            }
            else
            {
                await Context.Message.ReplyAsync(_localHandler.GetLocales().SubscribeError);
            }
        }

        [Command("unsubscribe", RunMode = RunMode.Async)]
        public async Task Unsubscribe()
        {
            _logger.LogInformation($"User {Context.User.Username} unsubscribe");
            if (await _plexNotifierr.Unsubscribe(Context.User.Id))
            {
                await Context.Message.ReplyAsync(_localHandler.GetLocales().UnsubscribeSuccess);
            }
            else
            {
                await Context.Message.ReplyAsync(_localHandler.GetLocales().UnsubscribeError);
            }
        }
    }
}