namespace BookAPI.ContractsDTOs.Requests
{
    // poco модели
    // неизменяемый (значения свойств - только при инициализации)
    public record AddAuthorRequest(
        string FirstName, 
        string LastName, 
        string Email, 
        DateTime BirthDate
    );
}
