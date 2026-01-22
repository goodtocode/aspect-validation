
# Goodtocode.Validation

[![NuGet CI/CD](https://github.com/goodtocode/aspect-validator/actions/workflows/gtc-validator-nuget.yml/badge.svg)](https://github.com/goodtocode/aspect-validator/actions/workflows/gtc-validator-nuget.yml)

Goodtocode.Validation is a lightweight, dependency-free validation library for .NET, designed for use in CQRS, Clean Architecture, and modern .NET applications. It provides a fluent API for defining validation rules for commands, queries, DTOs, and more.

## Features
- Fluent, expressive rule definitions
- Supports conditional and cross-property validation
- Throws custom exceptions for invalid data
- No external dependencies

## Core Concepts
- **Validator<T>**: Base class for defining validation rules for a type.
- **RuleBuilder**: Fluent builder for chaining rules on properties.
- **ValidationResult**: Contains validation errors and validity state.
- **CustomValidationException**: Thrown when validation fails (optional).

## Example: Paginated Query Validator (Clean Architecture Use Case)
```csharp
public class GetEntitiesQueryValidator : Validator<GetEntitiesQuery>
{
    public GetEntitiesQueryValidator()
    {
        RuleFor(v => v.StartDate).NotEmpty()
            .When(v => v.EndDate != null)
            .LessThanOrEqualTo(v => v.EndDate);

        RuleFor(v => v.EndDate)
            .NotEmpty()
            .When(v => v.StartDate != null)
            .GreaterThanOrEqualTo(v => v.StartDate);

        RuleFor(x => x.PageNumber).NotEqual(0);
        RuleFor(x => x.PageSize).NotEqual(0);
    }
}
```

## How to Use
1. Create a validator by inheriting from `Validator<T>`.
2. Use `RuleFor(x => x.Property)` to define rules with fluent methods like `.NotEmpty()`, `.NotEqual()`, `.LessThanOrEqualTo()`, `.GreaterThanOrEqualTo()`, `.When()`, etc.
3. Call `ValidateAndThrow(instance)` or `Validate(instance)` to check validity.


## RuleBuilder, Validation, and Exception Handling

### RuleBuilder
The `RuleBuilder` class provides a fluent API for defining validation rules on your object's properties. You can chain methods such as `.NotEmpty()`, `.NotEqual()`, `.LessThanOrEqualTo()`, `.GreaterThanOrEqualTo()`, and `.When()` to express complex validation logic in a readable way. Each rule is registered with the validator and will be checked when validation is performed.

### Validate
The `Validate` method (and its async variant) executes all defined rules for a given object instance. It returns a `ValidationResult` containing a list of `ValidationFailure` errors and an `IsValid` flag. If you use `ValidateAndThrow`, a `CustomValidationException` is thrown if any rule fails.

### Handling Validation in WebApi
When using `ValidateAndThrow`, any validation failures will result in a `CustomValidationException` being thrown. In ASP.NET Core WebApi projects, it is recommended to catch this exception in your global exception handling middleware. You can then translate the exception into a `400 Bad Request` response, returning the validation errors to the client in a structured format. This ensures that clients receive clear feedback on why their request was invalid, and keeps your controller actions clean and focused on business logic.

**Example: Handling Validation Exceptions in Middleware**
```csharp
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (CustomValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new { errors = ex.Errors });
        await context.Response.WriteAsync(result);
    }
});
```

## Installation
Install via NuGet:

```
dotnet add package Goodtocode.Validation
```

## License
MIT

## Contact
- [GitHub Repo](https://github.com/goodtocode/aspect-validation)
- [@goodtocode](https://twitter.com/goodtocode)
