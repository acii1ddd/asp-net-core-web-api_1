using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(author => author.Id); // перв ключ

        // описать связь автора с книгой (навигационные свойства и внешние ключи)
        builder
            .HasMany(author => author.Books)
            .WithOne(book => book.Author) // книга имеет одного автора
            .HasForeignKey(book => book.AuthorId); // внешний ключ на автора книги Author
    }
}
