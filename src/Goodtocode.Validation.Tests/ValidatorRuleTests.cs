namespace Goodtocode.Validation.Tests;

[TestClass]
public class ValidatorRuleTests
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
    public void ValidateAllRulesPassReturnsValid()
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
        var result = validator.Validate(entity);
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void ValidateNameEmptyReturnsInvalid()
    {
        var entity = new TestEntity { Name = "", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 10, Status = TestStatus.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Name"));
    }

    [TestMethod]
    public void ValidateAgeZeroReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 0, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 10, Status = TestStatus.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Age"));
    }

    [TestMethod]
    public void ValidateStartDateGreaterThanEndDateReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today.AddDays(2), EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 10, Status = TestStatus.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "StartDate"));
    }

    [TestMethod]
    public void ValidateEndDateLessThanStartDateReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(-1), PageNumber = 1, PageSize = 10, Status = TestStatus.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "EndDate"));
    }

    [TestMethod]
    public void ValidatePageNumberZeroReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 0, PageSize = 10, Status = TestStatus.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "PageNumber"));
    }

    [TestMethod]
    public void ValidatePageSizeZeroReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 0, Status = TestStatus.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "PageSize"));
    }

    [TestMethod]
    public void ValidateStatusNotInEnumReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 10, Status = (TestStatus)99 };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Status"));
    }

    [TestMethod]
    public void ValidateWhenConditionTrueOnlyAppliesRule()
    {
        // Rule: Name must not be empty, but only when Age > 18
        var rules = new List<Func<TestEntity, ValidationFailure?>>();
        var builder = new RuleBuilder<TestEntity, string>(x => x.Name, nameof(TestEntity.Name), rules)
            .NotEmpty()
            .When(x => x.Age > 18);

        var entity = new TestEntity { Name = "", Age = 20 };
        // Should fail because Age > 18 and Name is empty
        var failure = rules.Select(r => r(entity)).FirstOrDefault(f => f != null);
        Assert.IsNotNull(failure);
        Assert.AreEqual("Name", failure.PropertyName);

        entity = new TestEntity { Name = "", Age = 15 };
        // Should pass (no failure) because Age <= 18, so rule is not applied
        failure = rules.Select(r => r(entity)).FirstOrDefault(f => f != null);
        Assert.IsNull(failure);
    }

    [TestMethod]
    public void ValidateWhenWithErrorMessageReturnsCustomError()
    {
        // Rule: Name must not be empty, but if Age <= 18, return custom error
        var rules = new List<Func<TestEntity, ValidationFailure?>>();
        var builder = new RuleBuilder<TestEntity, string>(x => x.Name, nameof(TestEntity.Name), rules)
            .NotEmpty()
            .When(x => x.Age > 18, "Name validation skipped for minors");

        var entity = new TestEntity { Name = "", Age = 15 };
        // Should return the custom error message because Age <= 18
        var failure = rules.Select(r => r(entity)).FirstOrDefault(f => f != null);
        Assert.IsNotNull(failure);
        Assert.AreEqual("Name", failure.PropertyName);
        Assert.AreEqual("Name validation skipped for minors", failure.ErrorMessage);

        entity = new TestEntity { Name = "", Age = 20 };
        // Should fail with the NotEmpty error because Age > 18 and Name is empty
        failure = rules.Select(r => r(entity)).FirstOrDefault(f => f != null);
        Assert.IsNotNull(failure);
        Assert.AreEqual("Name", failure.PropertyName);
        Assert.AreNotEqual("Name validation skipped for minors", failure.ErrorMessage);
    }
}
