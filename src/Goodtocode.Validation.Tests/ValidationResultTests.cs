namespace Goodtocode.Validation.Tests;

[TestClass]
public class ValidationResultTests
{
    [TestMethod]
    public void ValidationFailurePropertiesAreSetCorrectly()
    {
        var failure = new ValidationFailure("TestProp", "Test error");
        Assert.AreEqual("TestProp", failure.PropertyName);
        Assert.AreEqual("Test error", failure.ErrorMessage);
    }

    [TestMethod]
    public void ValidationResultErrorsAndIsValidWorkCorrectly()
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Prop1", "Error1"),
            new ValidationFailure("Prop2", "Error2")
        };
        var result = new ValidationResult(failures);
        Assert.AreEqual(2, result.Errors.Count);
        Assert.IsFalse(result.IsValid);
    }
}
