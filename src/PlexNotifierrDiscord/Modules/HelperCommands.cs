using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PlexNotifierrDiscord.Services;

namespace PlexNotifierrDiscord.Modules
{
    public class HelperCommands : ModuleBase<ShardedCommandContext>
    {
        private readonly ILocalHandler _localHandler;
        private readonly ILogger<HelperCommands> _logger;

        public HelperCommands(ILocalHandler localHandler, ILogger<HelperCommands> logger)
        {
            _localHandler = localHandler;
            _logger = logger;
        }
        
        [Command("help", RunMode = RunMode.Async)]
        public async Task? Help()
        {
            _logger.LogInformation($"Help requested by {Context.User.Username}");
            var locales = _localHandler.GetLocales();
            var embedBuilder = new EmbedBuilder()
                              .WithAuthor(Context.Client.CurrentUser.Username, Context.Client.CurrentUser.GetAvatarUrl())
                              .WithTitle(locales.HelpTitle)
                              .AddField("\u200b", "\u200b")
                              .AddField("!subscribe {username Plex}", locales.SubscribeHelp)
                              .AddField("!unsubscribe", locales.UnsubscribeHelp)
                              .WithColor(Color.DarkPurple)
                              .WithCurrentTimestamp();

            await Context.Message.ReplyAsync(embed: embedBuilder.Build());
        }
    }
}