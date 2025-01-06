namespace BookAPI.ContractsDTOs.Responses
{
    public record GetAuthorWithBooksResponse (
        Guid id, 
        string FirstName, 
        string LastName, 
        string Email, 
        DateTime BirthDate, 
        List<GetBookResponse> Books
    );
}
