namespace MiniProject1
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CrawlerContext1 : DbContext
    {
        public CrawlerContext1()
            : base("name=CrawlerContext1")
        {
        }

        public virtual DbSet<crawlerStorage> crawlerStorage { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<crawlerStorage>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<crawlerStorage>()
                .Property(e => e.content)
                .IsUnicode(false);
        }
    }
}
