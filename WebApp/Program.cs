using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Serilog;
using WebApp.CompositionRoot;
using WebApp.LoggingConfiguration;

namespace WebApp;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = Logging.CreateLogger();
        try
        {
            var app = WebApplication
               .CreateSlimBuilder(args)
               .ConfigureServices(Log.Logger)
               .Build()
               .ConfigureMiddleware();
            await app.RunAsync();
            return 0;
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Could not run web app");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}