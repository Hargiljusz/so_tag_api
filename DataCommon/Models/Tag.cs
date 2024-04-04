using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataCommon.Models
{
    public class Tag
    {
        [Key]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [JsonPropertyName("last_activity_date")]
        public int LastActivityDate { get; set; }

        [JsonPropertyName("has_synonyms")]
        public bool HasSynonyms { get; set; }

        [JsonPropertyName("is_moderator_only")]
        public bool IsModeratorOnly { get; set; }

        [JsonPropertyName("is_required")]
        public bool IsRequired { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("collectives")]
        public List<Collective>? Collectives { get; set; }

        [JsonPropertyName("percentage")]
        public float Percentage { get; set; } = -1f;

        public Tag(string iD, int lastActivityDate, bool hasSynonyms, bool isModeratorOnly, bool isRequired, int count, string name, List<Collective>? collectives)
        {
            ID = iD;
            LastActivityDate = lastActivityDate;
            HasSynonyms = hasSynonyms;
            IsModeratorOnly = isModeratorOnly;
            IsRequired = isRequired;
            Count = count;
            Name = name;
            Collectives = collectives;
        }

        public Tag()
        {
        }

        public override string? ToString()
        {
            return $"Name: {Name}, Count: {Count}, Percentage: {Percentage}";
        }
    }





}
