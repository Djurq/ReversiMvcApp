using Microsoft.EntityFrameworkCore;
using ReversiMvcApp.Models;

namespace ReversiMvcApp.DAL
{
    public class ReversiDbContext : DbContext
    {
        public ReversiDbContext() { }
        public ReversiDbContext(DbContextOptions<ReversiDbContext> options) : base(options) {}

        public DbSet<Speler> Spelers { get; set; }

    }
}
