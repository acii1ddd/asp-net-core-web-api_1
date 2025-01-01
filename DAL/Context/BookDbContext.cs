using DAL.Entities;
using DAL.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {  
        }

        public DbSet<Book> Books { get; set; }
        
        public DbSet<Author> Authors { get; set; }

        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // регистрируем конфигурации сущностей
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
