using System.Globalization;
using System.Linq.Expressions;

namespace Goodtocode.Validation;

public class RuleBuilder<T, TProp>(Expression<Func<T, TProp>> selector, string propertyName, List<Func<T, ValidationFailure?>> rules)
{
    private readonly Expression<Func<T, TProp>> _selector = selector;
    private readonly string _propertyName = propertyName;
    private readonly List<Func<T, ValidationFailure?>> _rules = rules;
    private Func<T, bool>? _condition;

    public RuleBuilder<T, TProp> NotEmpty(string? errorMessage = null)
    {
        _rules.Add(instance =>
        {
            if (_condition != null && !_condition(instance)) return null;

            var value = _selector.Compile()(instance);

            bool isValid = value switch
            {
                null => false,
                Guid guid => guid != Guid.Empty,
                string str => !string.IsNullOrWhiteSpace(str),
                int i => i != 0,
                long l => l != 0L,
                double d => d != 0.0,
                decimal m => m != 0m,
                DateTime dt => dt != DateTime.MinValue,
                Enum e => Convert.ToInt32(e, CultureInfo.InvariantCulture) != 0,
                _ => true
            };

            return isValid ? null : new ValidationFailure(_propertyName, errorMessage ?? $"{_propertyName} must not be empty");
        });
        return this;
    }

    public RuleBuilder<T, TProp> NotEqual(TProp other, string? errorMessage = null)
    {
        _rules.Add(instance =>
        {
            if (_condition != null && !_condition(instance)) return null;

            var value = _selector.Compile()(instance);
            var comparison = Comparer<TProp>.Default.Compare(value, other);
            return comparison != 0 ? null : new ValidationFailure(_propertyName, errorMessage ?? $"{_propertyName} must not be equal to {other}");
        });
        return this;
    }

    public RuleBuilder<T, TProp> Equal(TProp other, string? errorMessage = null)
    {
        _rules.Add(instance =>
        {
            if (_condition != null && !_condition(instance)) return null;

            var value = _selector.Compile()(instance);
            var comparison = Comparer<TProp>.Default.Compare(value, other);
            return comparison == 0 ? null : new ValidationFailure(_propertyName, errorMessage ?? $"{_propertyName} must not be equal to {other}");
        });
        return this;
    }

    public RuleBuilder<T, TProp> LessThanOrEqualTo(Func<T, TProp> otherSelector, string? errorMessage = null)
    {
        _rules.Add(instance =>
        {
            if (_condition != null && !_condition(instance)) return null;

            var value = _selector.Compile()(instance);
            var other = otherSelector(instance);
            var comparison = Comparer<TProp>.Default.Compare(value, other);
            return comparison <= 0 ? null : new ValidationFailure(_propertyName, errorMessage ?? $"{_propertyName} must be ≤ {other}");
        });
        return this;
    }

    public RuleBuilder<T, TProp> GreaterThanOrEqualTo(Func<T, TProp> otherSelector, string? errorMessage = null)
    {
        _rules.Add(instance =>
        {
            if (_condition != null && !_condition(instance)) return null;

            var value = _selector.Compile()(instance);
            var other = otherSelector(instance);
            var comparison = Comparer<TProp>.Default.Compare(value, other);
            return comparison >= 0 ? null : new ValidationFailure(_propertyName, errorMessage ?? $"{_propertyName} must be ≥ {other}");
        });
        return this;
    }

    public RuleBuilder<T, TProp> IsInEnum(string? errorMessage = null)
    {
        _rules.Add(instance =>
        {
            if (_condition != null && !_condition(instance)) return null;

            var value = _selector.Compile()(instance);

            if (value is null) return new ValidationFailure(_propertyName, errorMessage ?? $"{_propertyName} must be a valid enum value");

            var type = typeof(TProp);
            if (!type.IsEnum)
                return new ValidationFailure(_propertyName, errorMessage ?? $"{_propertyName} is not an enum type");

            bool isDefined = Enum.IsDefined(type, value);
            return isDefined ? null : new ValidationFailure(_propertyName, errorMessage ?? $"{_propertyName} must be a valid enum value");
        });
        return this;
    }

    public RuleBuilder<T, TProp> When(Func<T, bool> condition)
    {
        _condition = condition;
        return this;
    }

    public RuleBuilder<T, TProp> When(Func<T, bool> condition, string? errorMessage = null)
    {
        _condition = condition;
        if (errorMessage != null)
        {
            _rules.Add(instance =>
            {
                if (_condition != null && !_condition(instance))
                {
                    return new ValidationFailure(_propertyName, errorMessage);
                }
                return null;
            });
        }
        return this;
    }
}
