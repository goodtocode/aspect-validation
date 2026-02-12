namespace Goodtocode.Validation.Tests;

[TestClass]
public class ValidatorAsyncTests
{
    public class TestEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public TestStatus Status { get; set; }
    }

    public class TestEntityValidator : Validator<TestEntity>
    {
        public TestEntityValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Age).NotEqual(0);
            RuleFor(x => x.StartDate).LessThanOrEqualTo(x => x.EndDate);
            RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
            RuleFor(x => x.PageNumber).NotEqual(0);
            RuleFor(x => x.PageSize).NotEqual(0);
            RuleFor(x => x.Status).IsInEnum();
        }
    }

    [TestMethod]
    public async Task ValidateAsyncAllRulesPassReturnsValid()
    {
        var entity = new TestEntity
        {
            Name = "John",
            Age = 25,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            PageNumber = 1,
            PageSize = 10,
            Status = TestStatus.Active
        };
        var validator = new TestEntityValidator();
        var result = await validator.ValidateAsync(entity);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task ValidateAndThrowAsyncInvalidThrowsCustomValidationException()
    {
        var entity = new TestEntity { Name = "", Age = 0, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 0, PageSize = 0, Status = (TestStatus)99 };
        var validator = new TestEntityValidator();
        await Assert.ThrowsAsync<CustomValidationException>(async () => await validator.ValidateAndThrowAsync(entity));
    }
}
