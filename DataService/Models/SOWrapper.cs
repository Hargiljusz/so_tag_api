using System.Collections;
using System.Text.Json.Serialization;

namespace DataService.Models
{
    public class SOWrapper<T> where T : class
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = Enumerable.Empty<T>().ToList();

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("quota_max")]
        public int QuotaMax { get; set; }

        [JsonPropertyName("quota_remaining")]
        public int QuotaRemaining { get; set; }

        public SOWrapper(List<T> items, bool hasMore, int quotaMax, int quotaRemaining)
        {
            Items = items;
            HasMore = hasMore;
            QuotaMax = quotaMax;
            QuotaRemaining = quotaRemaining;
        }
    }
}
