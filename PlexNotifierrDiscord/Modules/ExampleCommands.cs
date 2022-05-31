using Discord;
using Discord.Commands;

namespace PlexNotifierrDiscord.Modules
{
    public class ExampleCommands : ModuleBase<ShardedCommandContext>
    {
        [Command("hello", RunMode = RunMode.Async)]
        public async Task Hello()
        {
            await Context.User.SendMessageAsync($"Hello {Context.User.Username}.");
            await Context.Message.ReplyAsync($"Hello {Context.User.Username}. Nice to meet you!");
        }
    }
}