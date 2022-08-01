namespace PlexNotifierrDiscord.Services
{
    public interface IPlexNotifierrApi
    {
        public Task<bool> Subscribe(ulong discordId, string plexUsername);

        public Task<bool> Unsubscribe(ulong discordId);
    }
}