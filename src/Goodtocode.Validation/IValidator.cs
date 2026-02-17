namespace Goodtocode.Validation;

public interface IValidator<T>
{
    ValidationResult Validate(T instance);
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
    void ValidateAndThrow(T instance);
    Task ValidateAndThrowAsync(T instance, CancellationToken cancellationToken = default);
}