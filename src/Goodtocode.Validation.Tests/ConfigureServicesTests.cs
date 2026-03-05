using Microsoft.Extensions.DependencyInjection;

namespace Goodtocode.Validation.Tests;

[TestClass]
public class ConfigureServicesTests
{
    [TestMethod]
    public void AddValidationServicesRegistersValidatorsInAssembly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var validator = serviceProvider.GetService<IValidator<TestEntity>>();
        Assert.IsNotNull(validator);
    }

    [TestMethod]
    public void AddValidationServicesResolvesCorrectValidatorType()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var validator = serviceProvider.GetService<IValidator<TestEntity>>();
        Assert.IsNotNull(validator);
        Assert.IsInstanceOfType<TestEntityValidator>(validator);
    }

    [TestMethod]
    public void AddValidationServicesRegistersValidatorsAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var validator1 = serviceProvider.GetService<IValidator<TestEntity>>();
        var validator2 = serviceProvider.GetService<IValidator<TestEntity>>();

        Assert.IsNotNull(validator1);
        Assert.IsNotNull(validator2);
        Assert.AreNotSame(validator1, validator2, "Validators should be transient and not the same instance");
    }

    [TestMethod]
    public void AddValidationServicesCanResolveAndValidate()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddValidationServices();
        var serviceProvider = services.BuildServiceProvider();
        var validator = serviceProvider.GetRequiredService<IValidator<TestEntity>>();

        var entity = new TestEntity
        {
            Name = "Test",
            Age = 25,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            PageNumber = 1,
            PageSize = 10,
            Status = TestEnumeration.Active
        };

        // Act
        var result = validator.Validate(entity);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.IsEmpty(result.Errors);
    }

    [TestMethod]
    public void AddValidationServicesCanResolveAndDetectValidationErrors()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddValidationServices();
        var serviceProvider = services.BuildServiceProvider();
        var validator = serviceProvider.GetRequiredService<IValidator<TestEntity>>();

        var entity = new TestEntity
        {
            Name = "",  // Invalid - empty
            Age = 0,    // Invalid - equals 0
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(-1), // Invalid - before start date
            PageNumber = 0, // Invalid - equals 0
            PageSize = 0,   // Invalid - equals 0
            Status = (TestEnumeration)999 // Invalid - not defined
        };

        // Act
        var result = validator.Validate(entity);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsNotEmpty(result.Errors);
    }

    [TestMethod]
    public async Task AddValidationServicesCanResolveAndValidateAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddValidationServices();
        var serviceProvider = services.BuildServiceProvider();
        var validator = serviceProvider.GetRequiredService<IValidator<TestEntity>>();

        var entity = new TestEntity
        {
            Name = "Test",
            Age = 25,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            PageNumber = 1,
            PageSize = 10,
            Status = TestEnumeration.Active
        };

        // Act
        var result = await validator.ValidateAsync(entity);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void AddValidationServicesReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddValidationServices();

        // Assert
        Assert.AreSame(services, result, "Extension method should return the same IServiceCollection for chaining");
    }

    [TestMethod]
    public void AddValidationServicesDoesNotThrowWhenCalledMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        services.AddValidationServices();
        services.AddValidationServices();

        var serviceProvider = services.BuildServiceProvider();
        var validator = serviceProvider.GetService<IValidator<TestEntity>>();

        Assert.IsNotNull(validator);
    }

    [TestMethod]
    public void AddValidationServicesRegistersOnlyValidatorsFromExecutingAssembly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var testEntityValidator = serviceProvider.GetService<IValidator<TestEntity>>();
        Assert.IsNotNull(testEntityValidator);

        var validatorType = testEntityValidator.GetType();
        Assert.AreEqual("Goodtocode.Validation.Tests", validatorType.Namespace);
    }
}