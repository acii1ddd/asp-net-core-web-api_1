using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    public class BookDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int ReleaseYear { get; set; }

        public decimal Price { get; set; }

        public Guid AuthorId { get; set; }

        // для не зацикливания cериализации в json (cache)
        [JsonIgnore]
        public AuthorDTO? Author { get; set; }

        public List<GenreDTO> Genres { get; set; } = [];
    }
}
