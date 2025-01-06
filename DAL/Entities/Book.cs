using System.Text.Json.Serialization;

namespace DAL.Entities
{
    public class Book
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int ReleaseYear { get; set; }

        public decimal Price { get; set; }

        public Guid AuthorId { get; set; }

        // для не зацикливания сериализации в json
        [JsonIgnore]
        public Author? Author { get; set; }

        public List<Genre> Genres { get; set; } = [];
    }
}
