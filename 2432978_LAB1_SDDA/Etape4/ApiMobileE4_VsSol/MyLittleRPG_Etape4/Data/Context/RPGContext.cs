using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape4.Models;

namespace MyLittleRPG_Etape4.Data.Context
{
    public class RPGContext : DbContext
    {
        public DbSet<Monster> Monster { get; set; }
        public DbSet<Tile> Tile { get; set; }
        public DbSet<Utilisateur> Utilisateur { get; set; }
        public DbSet<Personnage> Personnage { get; set; }
        public DbSet<TuileExploree> TuileExploree { get; set; }
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Deux comptes ne peuvent pas avoir le même email
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            //1 utilisateur <-> 1 personnage (la clé étrangère est dans Personnage)
            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Personnage)
                .WithOne(p => p.Utilisateur)
                .HasForeignKey<Personnage>(p => p.utilisateurID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
