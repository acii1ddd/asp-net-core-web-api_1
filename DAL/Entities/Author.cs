namespace DAL.Entities
{
    public class Author
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public List<Book> Books { get; set; } = [];
    }
}
