using Discord;
using Discord.Commands;
using PlexNotifierrDiscord.Services;

namespace PlexNotifierrDiscord.Modules
{
    public class SubscribeManagementCommands : ModuleBase<ShardedCommandContext>
    {
        private readonly IPlexNotifierrApi _plexNotifierr;

        public SubscribeManagementCommands(IPlexNotifierrApi plexNotifierr)
        {
            _plexNotifierr = plexNotifierr;
        }

        [Command("subscribe", RunMode = RunMode.Async)]
        public async Task? SubscribeWithoutPlexName()
        {
            await Context.Message.ReplyAsync($"Il faut mettre son nom d'utilisateur Plex pour pouvoir souscrire aux notifications");
        }

        [Command("subscribe", RunMode = RunMode.Async)]
        public async Task Subscribe(string plexName)
        {
            if (await _plexNotifierr.Subscribe(Context.User.Id, plexName))
            {
                await Context.Message.ReplyAsync($"Tu as souscrit aux notifications");
            }
            else
            {
                await Context.Message.ReplyAsync($"Il y a eu un problème pendant ta souscription aux notifications");
            }
        }

        [Command("unsubscribe", RunMode = RunMode.Async)]
        public async Task Subscribe()
        {
            if (await _plexNotifierr.Unsubscribe(Context.User.Id))
            {
                await Context.Message.ReplyAsync($"Tu es maintenant désinscrit des notifications!");
            }
            else
            {
                await Context.Message.ReplyAsync($"Il y a eu un problème pendant ta désinscription");
            }
        }
    }
}