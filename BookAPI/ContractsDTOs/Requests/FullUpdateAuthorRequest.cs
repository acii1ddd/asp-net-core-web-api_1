namespace BookAPI.ContractsDTOs.Requests
{
    // poco модели
    // неизменяемый (значения свойств - только при инициализации)
    public record FullUpdateAuthorRequest(
        string FirstName,
        string LastName,
        string Email,
        DateTime BirthDate // если не указать - будет by default value (DateTime.MinValue) / остальные поля - required
    );
}