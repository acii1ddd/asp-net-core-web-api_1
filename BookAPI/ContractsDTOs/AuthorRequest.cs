namespace BookAPI.ContractsDTOs
{
    // неизменяемый (значения свойств - только при инициализации)
    public record AuthorRequest(string FirstName, string LastName, string Email, DateTime BirthDate);
}
