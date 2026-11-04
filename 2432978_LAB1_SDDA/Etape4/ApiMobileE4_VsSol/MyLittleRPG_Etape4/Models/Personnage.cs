using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG_Etape4.Models
{
    public class Personnage
    {
        [Key]
        public int idPersonnage { get; set; }

        [Required, MaxLength(45)]
        public string Nom { get; set; } = string.Empty;

        public int Niveau { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int PointsVie { get; set; } = 100;
        public int PointsVieMax { get; set; } = 100;
        public int Force { get; set; } = 10;
        public int Defense { get; set; } = 5;

        //Position de départ (à valider: la tuile doit être traversable)
        public int PositionX { get; set; } = 10;
        public int PositionY { get; set; } = 10;

        public int utilisateurID { get; set; }
        public Utilisateur? Utilisateur { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
