namespace Goodtocode.Validation;

public class ValidationFailure(string propertyName, string errorMessage)
{
    public string PropertyName { get; private set; } = propertyName;
    public string ErrorMessage { get; private set; } = errorMessage;
}
