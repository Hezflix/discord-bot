using Discord;
using Discord.Commands;

namespace PlexNotifierrDiscord.Modules
{
    public class HelperCommands : ModuleBase<ShardedCommandContext>
    {
        [Command("help", RunMode = RunMode.Async)]
        public async Task? Help()
        {
            var foo = Context.User.GetAvatarUrl();
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