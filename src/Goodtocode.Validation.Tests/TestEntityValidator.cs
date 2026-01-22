namespace Goodtocode.Validation.Tests;

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
