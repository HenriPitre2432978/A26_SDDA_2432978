using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape2.Models;

namespace MyLittleRPG_Etape2.Data.Context
{
    public class MonsterContext : DbContext
    {
        public DbSet<Monster> Monster { get; set; }
        public MonsterContext(DbContextOptions<MonsterContext> options) : base(options) {}
    }
}
