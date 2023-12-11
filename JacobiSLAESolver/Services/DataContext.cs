using JacobiSLAESolver.Models;
using Microsoft.EntityFrameworkCore;

namespace JacobiSLAESolver.Services
{
    public class DataContext : DbContext
    {
        public DbSet<SLAE> Systems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Resources.SQLConnectionString);
        }
    }
}