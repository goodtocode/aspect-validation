using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Goodtocode.Validation;

public static class ConfigureServices
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var callingAssembly = Assembly.GetCallingAssembly();
        
        return services.AddValidationServices(executingAssembly, callingAssembly);
    }

    public static IServiceCollection AddValidationServices(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            return services;

        var assembliesToScan = new HashSet<Assembly>(assemblies);
        
        foreach (var assembly in assembliesToScan)
        {
            var validatorTypes = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IValidator<>)));

            foreach (var validatorType in validatorTypes)
            {
                var validatorInterface = validatorType.GetInterfaces()
                    .First(i => i.IsGenericType && 
                        i.GetGenericTypeDefinition() == typeof(IValidator<>));

                services.AddTransient(validatorInterface, validatorType);
            }
        }

        return services;
    }
}