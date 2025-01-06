namespace BookAPI.ContractsDTOs.Responses
{
    public record GetBookResponse (
        Guid Id,
        string Title,
        int ReleaseYear,
        decimal Price,
        Guid AuthorId
    );
}
