using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Behaviours;
using TravelInspiration.API.Shared.Metrics;
using TravelInspiration.API.Shared.Networking;
using TravelInspiration.API.Shared.Persistence;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {

        services.AddScoped<IDestinationSearchApiClient, DestinationSearchApiClient>();
        services.RegisterSlices();
        var currentAssembly = Assembly.GetExecutingAssembly();
        services.AddAutoMapper(currentAssembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(currentAssembly)
            .AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>))
            .AddOpenBehavior(typeof(ModelValidationBehavior<,>))
            .AddOpenBehavior(typeof(HandlePerformanceMetricBehaviour<,>));
        });

        services.AddValidatorsFromAssembly(currentAssembly);
        services.AddSingleton<HandlePerformanceMetric>();
        return services;
    }

    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TravelnspirationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("TravelInspirationDbConnection")));

        return services;
    }
}
