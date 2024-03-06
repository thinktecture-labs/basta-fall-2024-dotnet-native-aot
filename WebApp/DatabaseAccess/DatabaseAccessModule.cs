using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Light.EmbeddedResources;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;

namespace WebApp.DatabaseAccess;

public static class DatabaseAccessModule
{
    public static IServiceCollection AddDatabaseAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidDataException("Could not find default connection string in app settings");
        }

        return services.AddSingleton<NpgsqlDataSource>(
                sp => new NpgsqlDataSourceBuilder(connectionString)
                   .UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>())
                   .Build()
            )
           .AddScoped(sp => sp.GetRequiredService<NpgsqlDataSource>().CreateConnection());
    }

    public static ValueTask SetupDatabaseAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        var resiliencyPipeline = new ResiliencePipelineBuilder()
           .AddRetry(
                new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(1)
                }
            )
           .Build();

        return resiliencyPipeline.ExecuteAsync(
            async (webApp, cancelToken) =>
            {
                await using var scope = webApp.Services.CreateAsyncScope();
                var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();
                await connection.OpenAsync(cancelToken);
                await using var command = connection.CreateCommand();
                command.CommandText = typeof(DatabaseAccessModule).GetEmbeddedResource("DatabaseSetup.sql");
                await command.ExecuteNonQueryAsync(cancelToken);
            },
            app,
            cancellationToken
        );
    }
}