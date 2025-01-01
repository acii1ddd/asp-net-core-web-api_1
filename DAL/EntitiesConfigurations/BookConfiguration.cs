using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntitiesConfigurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(book => book.Id); // перв ключ

            // с автором
            builder
                .HasOne(book => book.Author)
                .WithMany(author => author.Books)
                .HasForeignKey(book => book.AuthorId);
            // внешние ключи указывать в 2 концигурациях

            // с жанром
            builder
                .HasMany(book => book.Genres)
                .WithMany(genre => genre.Books);
        }
    }
}