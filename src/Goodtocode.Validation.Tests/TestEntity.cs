namespace Goodtocode.Validation.Tests;

public class TestEntity
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public TestEnumeration Status { get; set; }
}

public enum TestEnumeration { None = 0, Active = 1, Inactive = 2 }
