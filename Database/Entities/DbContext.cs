namespace Database.Entities
{
  using Microsoft.EntityFrameworkCore;
  
  public partial class EAppContext : DbContext
  {
    private string DatabasePath { get; set; }
    public EAppContext(string databasePath)
    {
      DatabasePath = databasePath;
    }

    public virtual DbSet<Word> Words { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite($"Filename={DatabasePath}");
    }
  }
}
