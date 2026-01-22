namespace Goodtocode.Validation.Tests;

[TestClass]
public class ValidatorExceptionTests
{
    [TestMethod]
    public void ValidateAndThrowInvalidThrowsCustomValidationException()
    {
        var entity = new ValidatorRuleTests.TestEntity { Name = "", Age = 0, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 0, PageSize = 0, Status = (TestStatus)99 };
        var validator = new ValidatorRuleTests.TestEntityValidator();
        Assert.ThrowsException<CustomValidationException>(() => validator.ValidateAndThrow(entity));
    }

    [TestMethod]
    public void CustomValidationExceptionErrorsAreGroupedByProperty()
    {
        var failures = new[]
        {
            new ValidationFailure("Prop1", "Error1"),
            new ValidationFailure("Prop1", "Error2"),
            new ValidationFailure("Prop2", "Error3")
        };
        var ex = new CustomValidationException(failures);
        Assert.AreEqual(2, ex.Errors.Count);
        Assert.AreEqual(2, ex.Errors["Prop1"].Length);
        Assert.AreEqual(1, ex.Errors["Prop2"].Length);
    }
}
