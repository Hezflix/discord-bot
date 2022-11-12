using Newtonsoft.Json;
using PlexNotifierrDiscord.Models;

namespace PlexNotifierrDiscord.Services
{
    public class LocalHandler : ILocalHandler
    {
        private Locales? Locales { get; set; }

        public LocalHandler() { }

        private void InitializeAsync()
        {
            var fileName = "locales.json";
            using var fileStream = File.OpenRead(fileName);
            using var sr = new StreamReader(fileStream);
            using JsonReader reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();
            Locales = serializer.Deserialize<Locales>(reader)!;
        }

        public Locales GetLocales()
        {
            if (Locales is null)
            {
                InitializeAsync();
            }
            return Locales!;
        }
    }
}