using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG_Etape4.Models
{
    public class Utilisateur
    {
        [Key]
        public int idUtilisateur { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        //On stocke le HASH du mot de passe, jamais le mot de passe en clair
        [Required, MaxLength(255)]
        public string MotDePasse { get; set; } = string.Empty;

        [Required, MaxLength(45)]
        public string Pseudo { get; set; } = string.Empty;

        public DateTime DateInscription { get; set; } = DateTime.UtcNow;

        //Un utilisateur = un personnage
        public Personnage? Personnage { get; set; }
    }
}
