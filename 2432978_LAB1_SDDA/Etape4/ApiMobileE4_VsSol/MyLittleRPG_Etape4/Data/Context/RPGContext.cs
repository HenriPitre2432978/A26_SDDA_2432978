using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape4.Models;

namespace MyLittleRPG_Etape4.Data.Context
{
    public class RPGContext : DbContext
    {
        public DbSet<Monster> Monster { get; set; }
        public DbSet<Tile> Tile { get; set; }
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }
    }
}
