namespace PlexNotifierrDiscord.Models
{
    public class PlexNotification
    {
        public string DiscordId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string ThumbUrl { get; set; } = "";
        public int Season { get; set; }
        public string EpisodeTitle { get; set; } = "";
        public int Episode { get; set; }
        public string GrandParentRatingKey { get; set; }
    }
}