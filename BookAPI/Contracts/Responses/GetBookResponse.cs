namespace BookAPI.Contracts.Responses
{
    public record GetBookResponse(
        Guid Id,
        string Title,
        int ReleaseYear,
        decimal Price,
        Guid AuthorId
    );
}
