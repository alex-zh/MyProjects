using System.Data.Entity;

namespace OpinionAnalyzer.DataLoader
{
    public class ArticlesDbContext : DbContext
    {
        public DbSet<LoadedArticle> Articles { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<LoadedArticle>()
        //    .Property(a => a.Content)
        //    .HasColumnType("ntext");
        //}
    }
}
