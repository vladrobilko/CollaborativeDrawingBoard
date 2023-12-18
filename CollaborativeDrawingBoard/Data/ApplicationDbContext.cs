using CollaborativeDrawingBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeDrawingBoard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Board>? Boards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Data Source=USER-PC;Trusted_Connection=True;Initial Catalog=CollaborativeDrawingBoardDb;Integrated Security=True;TrustServerCertificate=true");

        protected override void OnModelCreating(ModelBuilder builder) => builder.Entity<Board>().ToTable(tb => tb.HasTrigger("TriggerName"));
    }
}