namespace Goodtocode.Validation;

public class ValidationRule<T>
{
    public Func<T, bool> Condition { get; set; } = _ => true;
    public Func<T, string>? ErrorMessage { get; set; }
    public Func<T, bool>? IsValid { get; set; }
}
