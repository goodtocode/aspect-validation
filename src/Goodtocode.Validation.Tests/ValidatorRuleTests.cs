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
        public TestEnum Status { get; set; }
    }

    public enum TestEnum { None = 0, Active = 1, Inactive = 2 }

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
            Status = TestEnum.Active
        };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void ValidateNameEmptyReturnsInvalid()
    {
        var entity = new TestEntity { Name = "", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 10, Status = TestEnum.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Name"));
    }

    [TestMethod]
    public void ValidateAgeZeroReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 0, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 10, Status = TestEnum.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Age"));
    }

    [TestMethod]
    public void ValidateStartDateGreaterThanEndDateReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today.AddDays(2), EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 10, Status = TestEnum.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "StartDate"));
    }

    [TestMethod]
    public void ValidateEndDateLessThanStartDateReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(-1), PageNumber = 1, PageSize = 10, Status = TestEnum.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "EndDate"));
    }

    [TestMethod]
    public void ValidatePageNumberZeroReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 0, PageSize = 10, Status = TestEnum.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "PageNumber"));
    }

    [TestMethod]
    public void ValidatePageSizeZeroReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 0, Status = TestEnum.Active };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "PageSize"));
    }

    [TestMethod]
    public void ValidateStatusNotInEnumReturnsInvalid()
    {
        var entity = new TestEntity { Name = "John", Age = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), PageNumber = 1, PageSize = 10, Status = (TestEnum)99 };
        var validator = new TestEntityValidator();
        var result = validator.Validate(entity);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Status"));
    }
}
