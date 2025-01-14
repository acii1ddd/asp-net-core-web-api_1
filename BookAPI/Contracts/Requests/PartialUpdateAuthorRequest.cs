namespace BookAPI.Contracts.Requests
{
    // poco модели
    // неизменяемый (значения свойств - только при инициализации)
    public record PartialUpdateAuthorRequest(
        string? FirstName,
        string? LastName,
        string? Email,
        DateTime? BirthDate
    );
}