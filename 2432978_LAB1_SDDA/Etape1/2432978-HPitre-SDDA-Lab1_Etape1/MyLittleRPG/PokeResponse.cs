using System;

namespace MyLittleRPG
{
    public class PokemonResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int PointsVieBase { get; set; }
        public int ForceBase { get; set; }
        public int DefenseBase { get; set; }
        public int ExperienceBase { get; set; }
        public string SpriteUrl { get; set; } = "";
        public string Type1 { get; set; } = "";
        public string Type2 { get; set; } = "";

        public PokemonResponse(Pokemon p)
        {
            Id = p.Id;
            Name = p.Nom;
            PointsVieBase = p.PointsVieBase;
            ForceBase = p.ForceBase;
            DefenseBase = p.DefenseBase;
            ExperienceBase = p.ExperienceBase;
            SpriteUrl = p.SpriteUrl;
            Type1 = p.Type1;
            Type2 = p.Type2;
        }
    }
}
