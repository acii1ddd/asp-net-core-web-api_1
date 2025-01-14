namespace BookAPI.Contracts.Responses
{
    // poco модели
    // неизменяемый (значения свойств - только при инициализации)
    public record GetAuthorResponse(
        Guid Id, 
        string FirstName, 
        string LastName, 
        string Email, 
        DateTime BirthDate
    );
}
