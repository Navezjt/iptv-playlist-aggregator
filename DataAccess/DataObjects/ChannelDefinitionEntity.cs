using System.Collections.Generic;

namespace IptvPlaylistFetcher.DataAccess.DataObjects
{
    public sealed class ChannelDefinitionEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public List<string> Aliases { get; set; }
    }
}
