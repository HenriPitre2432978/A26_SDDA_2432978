using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace MyLittleRPG
{
    public class Pokemon
    {
        //Objects to deserialize
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Nom { get; set; } = "";


        //Whole objects deserialized, but only taking the info we need and rendering the info needed public.
        //The whole objects stay private to keep out too much data for no reason
        [JsonInclude]
        [JsonPropertyName("stats")]
        private StatItem[] Stats { get; set; } = [];


        [JsonInclude]
        [JsonPropertyName("sprites")]
        private SpriteItem Sprites { get; set; } = new();


        [JsonInclude]
        [JsonPropertyName("types")]
        private TypeItem[] Types { get; set; } = [];

        // HP
        public int PointsVieBase =>
            Stats.Length > 0
                ? Math.Max(Stats[0].BaseStat / 3, 10)
                : 10;

        // Attack
        public int ForceBase =>
            Stats.Length > 1
                ? Math.Max(Stats[1].BaseStat / 8, 5)
                : 5;

        // Defense
        public int DefenseBase =>
            Stats.Length > 2
                ? Math.Max(Stats[2].BaseStat / 8, 3)
                : 3;

        public int ExperienceBase =>
            Stats.Length > 0
                ? (int)Stats.Average(s => s.BaseStat)
                : 0;

        public string SpriteUrl =>
            Sprites.FrontDefault ?? "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT6eoP22fr1gee9ZOkWtUS0frFs_ZYwV0SCwrGuI14ebw&s=10";

        public string Type1 =>
            Types.Length > 0
                ? Types[0].Type.Name
                : "N/A";

        public string Type2 =>
            Types.Length > 1
                ? Types[1].Type.Name
                : "N/A";
    }


    /// <summary>
    /// Get the children of Stat object
    /// </summary>
    public class StatItem
    {
        [JsonPropertyName("base_stat")]
        public int BaseStat { get; set; }
    }


    /// <summary>
    /// Get the children of Sprite object
    /// </summary>
    public class SpriteItem
    {
        [JsonPropertyName("front_default")]
        public string? FrontDefault { get; set; }
    }


    /// <summary>
    /// Get the children of Type object
    /// </summary>
    public class TypeItem
    {
        [JsonPropertyName("type")]
        public TypeInfo Type { get; set; } = new();
    }

    /// <summary>
    /// Get the children of TypeInfo object
    /// </summary>
    public class TypeInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}
