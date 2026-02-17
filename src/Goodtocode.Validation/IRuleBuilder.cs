namespace Goodtocode.Validation;

public interface IRuleBuilder<T, TProp>
{
    IRuleBuilder<T, TProp> NotEqual(TProp other, string? errorMessage = null);
    IRuleBuilder<T, TProp> Equal(TProp other, string? errorMessage = null);
    IRuleBuilder<T, TProp> IsInEnum(string? errorMessage = null);
    IRuleBuilder<T, TProp> NotEmpty(string? errorMessage = null);
    IRuleBuilder<T, TProp> LessThanOrEqualTo(Func<T, TProp> other, string? errorMessage = null);
    IRuleBuilder<T, TProp> GreaterThanOrEqualTo(Func<T, TProp> other, string? errorMessage = null);
#pragma warning disable CA1716 // Identifiers should not match keywords
    IRuleBuilder<T, TProp> When(Func<T, bool> predicate);
#pragma warning restore CA1716
    IRuleBuilder<T, TProp> Must(Func<T, bool> predicate, string? errorMessage = null);
}
