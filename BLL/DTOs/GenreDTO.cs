namespace BLL.DTOs
{
    public class GenreDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<BookDTO> Books { get; set; } = [];
    }
}