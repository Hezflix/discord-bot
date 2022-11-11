using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace PlexNotifierrDiscord.Modules
{
    public class HelperCommands : ModuleBase<ShardedCommandContext>
    {
        private readonly ILogger<HelperCommands> _logger;

        public HelperCommands(ILogger<HelperCommands> logger)
        {
            _logger = logger;
        }
        
        [Command("help", RunMode = RunMode.Async)]
        public async Task? Help()
        {
            _logger.LogInformation($"Help requested by {Context.User.Username}");
            var embedBuilder = new EmbedBuilder()
                              .WithAuthor(Context.Client.CurrentUser.Username, Context.Client.CurrentUser.GetAvatarUrl())
                              .WithTitle("Hezping envoie une notification quand un nouvel épisode d'une série que tu regardes sort sur Hezflix")
                              .AddField("\u200b", "\u200b")
                              .AddField("!subscribe {username Plex}", "Permet de s'inscrire pour recevoir les notifications dès qu'un épisode sort")
                              .AddField("!unsubscribe", "Permet de te désinscrire et de ne plus recevoir les notifications")
                              .WithColor(Color.DarkPurple)
                              .WithCurrentTimestamp();

            await Context.Message.ReplyAsync(embed: embedBuilder.Build());
        }
    }
}