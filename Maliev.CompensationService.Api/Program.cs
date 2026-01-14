using Maliev.Aspire.ServiceDefaults;
using Maliev.CompensationService.Api.Filters;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Queries.Handlers;
using Maliev.CompensationService.Domain.Authorization;
using Maliev.CompensationService.Infrastructure.Data;
using Maliev.CompensationService.Infrastructure.Repositories;
using Maliev.CompensationService.Infrastructure.Services;
using Maliev.CompensationService.Infrastructure.Consumers;
using Microsoft.EntityFrameworkCore;
using Maliev.CompensationService.Application.Common.Mediator;
using Microsoft.Extensions.Logging;

// Initialize bootstrap logging
using var loggerFactory = LoggerFactory.Create(logBuilder => logBuilder.AddConsole());
var bootstrapLogger = loggerFactory.CreateLogger("Program");

try
{
    Program.Log.StartingHost(bootstrapLogger, "Compensation Service");

    var builder = WebApplication.CreateBuilder(args);

    // --- Secrets & Configuration ---
    builder.AddGoogleSecretManagerVolume();

    // --- Infrastructure & Observability ---
    builder.AddServiceDefaults();
    builder.AddStandardMiddleware(options =>
    {
        options.EnableRequestLogging = true;
    });
    builder.AddServiceMeters("compensation-meter");

    // Database
    builder.AddPostgresDbContext<CompensationDbContext>(connectionName: "CompensationDbContext");

    // Redis
    builder.AddRedisDistributedCache(instanceName: "compensation:");

    // MassTransit
    builder.AddMassTransitWithRabbitMq(x =>
    {
        x.AddConsumer<EmployeeCreatedEventConsumer>();
        x.AddConsumer<EmployeeTerminatedEventConsumer>();
        x.AddConsumer<UndoArchiveCompensationConsumer>();
    });

    // Authentication & Authorization
    builder.AddJwtAuthentication();

    // --- API Configuration ---
    builder.AddDefaultCors();
    builder.AddDefaultApiVersioning();

    if (!builder.Environment.IsProduction())
    {
        builder.AddStandardOpenApi(
            title: "MALIEV Compensation Service API",
            description: "Manages employee compensation, benefits, and salary history.");
    }

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<SalaryLoggingFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    });

    // IAM Registration
    builder.AddIAMServiceClient("compensation");
    builder.Services.AddIAMRegistration<CompensationIAMRegistrationService>("compensation");

    builder.Services.AddScoped<ICompensationRepository, CompensationRepository>();
    builder.Services.AddScoped<IBenefitsRepository, BenefitsRepository>();
    builder.Services.AddScoped<ISalaryHistoryRepository, SalaryHistoryRepository>();
    builder.Services.AddScoped<IBulkJobRepository, BulkJobRepository>();
    builder.Services.AddScoped<Maliev.CompensationService.Application.Commands.Handlers.UndoArchiveCompensationCommandHandler>();

    // Register Mediator
    builder.Services.AddScoped<Maliev.CompensationService.Application.Common.Mediator.IMediator, Maliev.CompensationService.Application.Common.Mediator.Mediator>();

    // Register Handlers
    var assembly = typeof(GetCompensationDetailsQueryHandler).Assembly;
    var handlerTypes = assembly.GetTypes()
        .Where(t => !t.IsAbstract && !t.IsInterface)
        .Where(t => t.GetInterfaces().Any(i => i.IsGenericType &&
            (i.GetGenericTypeDefinition() == typeof(Maliev.CompensationService.Application.Common.Mediator.IRequestHandler<,>) ||
             i.GetGenericTypeDefinition() == typeof(Maliev.CompensationService.Application.Common.Mediator.IRequestHandler<>))));

    foreach (var handlerType in handlerTypes)
    {
        foreach (var interfaceType in handlerType.GetInterfaces().Where(i => i.IsGenericType &&
            (i.GetGenericTypeDefinition() == typeof(Maliev.CompensationService.Application.Common.Mediator.IRequestHandler<,>) ||
             i.GetGenericTypeDefinition() == typeof(Maliev.CompensationService.Application.Common.Mediator.IRequestHandler<>))))
        {
            builder.Services.AddScoped(interfaceType, handlerType);
        }
    }

    var app = builder.Build();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    // --- Database Migrations ---
    await app.MigrateDatabaseAsync<CompensationDbContext>();

    // --- Middleware Pipeline ---
    app.UseStandardMiddleware();
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }
    app.UseRouting();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    // --- Endpoints ---
    app.MapControllers();
    app.MapDefaultEndpoints(servicePrefix: "compensation");
    app.MapApiDocumentation(servicePrefix: "compensation");

    Program.Log.ServiceStarted(logger, "Compensation Service");
    await app.RunAsync();
}
catch (Exception ex)
{
    Program.Log.HostTerminated(bootstrapLogger, ex, "Compensation Service");
    throw;
}
finally
{
    loggerFactory.Dispose();
}

/// <summary>
/// Main program class for the Compensation Service.
/// </summary>
public partial class Program
{
    internal static partial class Log
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Starting {ServiceName} host")]
        public static partial void StartingHost(ILogger logger, string serviceName);

        [LoggerMessage(Level = LogLevel.Critical, Message = "{ServiceName} host terminated unexpectedly during startup")]
        public static partial void HostTerminated(ILogger logger, Exception ex, string serviceName);

        [LoggerMessage(Level = LogLevel.Information, Message = "{ServiceName} started successfully")]
        public static partial void ServiceStarted(ILogger logger, string serviceName);
    }
}
