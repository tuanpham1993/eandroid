namespace EApp.Entities
{
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class EntityModel : DbContext
  {
    public EntityModel()
        : base("name=EntityConnection")
    {
    }

    public virtual DbSet<CommonWord> CommonWords { get; set; }
    public virtual DbSet<Manual> Manuals { get; set; }
    public virtual DbSet<Word> Words { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
    }
  }
}
