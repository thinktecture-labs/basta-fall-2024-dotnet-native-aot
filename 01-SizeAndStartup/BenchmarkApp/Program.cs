using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Light.GuardClauses;
using Microsoft.Extensions.Configuration;

namespace WrapperApp;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", true)
           .AddEnvironmentVariables()
           .AddCommandLine(args)
           .Build();

        var targetExecutable = GetTargetExecutableFromConfiguration(configuration);
        if (targetExecutable is null)
        {
            Console.WriteLine("Please provide a valid target executable in the configuration.");
            return 1;
        }

        var intervalBetweenIterations = GetIntervalBetweenIterations(configuration);
        var numberOfIterations = GetNumberOfIterations(configuration);
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = targetExecutable,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        List<ExecuteResult> executionResults = new (numberOfIterations);
        for (var i = 0; i < numberOfIterations; i++)
        {
            
            Console.WriteLine($"Performing iteration {i + 1}...");
            var externalTimestamp = Stopwatch.GetTimestamp();
            ExecuteResult? executionResult;
            using (var process = Process.Start(processStartInfo)!)
            {
                await process.WaitForExitAsync();
                executionResult = await CalculateExecutionResultAsync(process, externalTimestamp);
            }

            if (executionResult is null)
            {
                Console.WriteLine("Could not calculate the execution result.");
                return 1;
            }

            executionResults.Add(executionResult.Value);
            if (intervalBetweenIterations > TimeSpan.Zero)
                await Task.Delay(intervalBetweenIterations);
        }

        var statisticsResults = new StatisticsResult(executionResults);
        var resultsFileName = WriteStatisticsResults(statisticsResults);
        Console.WriteLine($"Results written to \"{resultsFileName}\"");

        return 0;
    }

    private static string WriteStatisticsResults(StatisticsResult statisticsResult)
    {
        var executionResults = statisticsResult.ExecuteResults;
        var now = DateTime.UtcNow;
        var nowText = now.ToString("yyyy-MM-dd HH-mm-ss");
        var fileName = $"Execution Results {nowText}.html";
        using var writer = new StreamWriter(new FileStream(fileName, FileMode.Create));
        var content =
            $$"""
              <!DOCTYPE html>
              <html lang="en">
              <head>
                  <script src="https://cdn.plot.ly/plotly-latest.min.js"></script>
                  <title>Execution Results {{nowText}}</title>
              </head>
              <body>
              <h1>Execution Results {{nowText}}</h1>
              <h2>Startup Time</h2>
              <div><strong>Mean External Startup Time: {{statisticsResult.MeanExternalElapsed.TotalMilliseconds:N2}}ms</strong></div>
              <div><strong>External Startup Time SD: {{statisticsResult.ExternalElapsedStandardDeviation.TotalMilliseconds:N2}}ms</strong></div>
              <div><strong>Mean Internal Startup Time: {{statisticsResult.MeanInternalElapsed.TotalMilliseconds:N2}}ms</strong></div>
              <div><strong>Internal Startup Time SD: {{statisticsResult.InternalElapsedStandardDeviation.TotalMilliseconds:N2}}ms</strong></div>
              <div id="elapsed-time" style="width: 800px; height: 700px"></div>
              <h2>Working Set</h2>
              <div><strong>Mean Working Set: {{statisticsResult.MeanWorkingSet:N2}}MB</strong></div>
              <div><strong>Working Set SD: {{statisticsResult.WorkingSetStandardDeviation:N2}}MB</strong></div>
              <div id="working-set" style="width: 800px; height: 700px"></div>
              <script>
              const x = Array.from({ length: {{executionResults.Count}} }, (_, i) => i + 1);
              const externalElapsed = [{{string.Join(", ", executionResults.Select(r => r.ExternalElapsed.TotalMilliseconds.ToString("N2")))}}];
              const internalElapsed = [{{string.Join(", ", executionResults.Select(r => r.InternalElapsed.TotalMilliseconds.ToString("N2")))}}];
              const externalElapsedTrace = {
                  x,
                  y: externalElapsed,
                  type: 'scatter',
                  name: 'External Startup Time',
                  line: {
                      color: 'rgb(42,223,255)'
                  }
              };
              const internalElapsedTrace = {
                  x,
                  y: internalElapsed,
                  type: 'scatter',
                  name: 'Internal Startup Time',
                  line: {
                      color: 'rgb(175,14,255)'
                  }
              };
              const elapsedLayout = {
                  title: 'Startup Time in ms',
                  xaxis: {
                      title: 'Iteration'
                  },
                  yaxis: {
                      title: 'Elapsed Time (ms)',
                      range: [0, Math.max(...externalElapsed, ...internalElapsed) + 10],
                      autorange: false
                  }
              };
              Plotly.newPlot('elapsed-time', [externalElapsedTrace, internalElapsedTrace], elapsedLayout);
              const workingSetData = [{{string.Join(", ", executionResults.Select(r => r.WorkingSet.ToString("N2")))}}];
              const workingSetTrace = {
                  x,
                  y: workingSetData,
                  type: 'scatter',
                  name: 'Working Set Size',
                  line: {
                      color: 'rgb(255,116,15)'
                  }
              };
              const workingSetLayout = {
                  title: 'Working Set Size',
                  xaxis: {
                      title: 'Iteration'
                  },
                  yaxis: {
                      title: 'Working Set Size (MB)',
                      range: [0, Math.max(...workingSetData) + 10],
                      autorange: false
                  }
              };
              Plotly.newPlot('working-set', [workingSetTrace], workingSetLayout);
              </script>
              </body>
              """;

        writer.WriteLine(content);
        return fileName;
    }

    private static async Task<ExecuteResult?> CalculateExecutionResultAsync(Process process, long externalTimestamp)
    {
        bool? isSlimBuilder = null;
        TimeSpan? internalElapsed = null;
        TimeSpan? externalElapsed = null;
        double? workingSet = null;

        while (!process.StandardOutput.EndOfStream)
        {
            var line = await process.StandardOutput.ReadLineAsync();
            if (line is null)
            {
                continue;
            }

            const string isSlimBuilderPrefix = "IsSlimBuilder: ";
            const string endingTimestampPrefix = "Ending timestamp: ";
            const string internalStartupTimePrefix = "Internal Startup Time in ms: ";
            const string workingSetPrefix = "Working Set in MB: ";
            if (line.StartsWith(isSlimBuilderPrefix))
            {
                isSlimBuilder = bool.Parse(line.AsSpan(isSlimBuilderPrefix.Length));
            }
            else if (line.StartsWith(endingTimestampPrefix))
            {
                var endingTimestamp = long.Parse(line.AsSpan(endingTimestampPrefix.Length));
                externalElapsed = Stopwatch.GetElapsedTime(externalTimestamp, endingTimestamp);
            }
            else if (line.StartsWith(internalStartupTimePrefix))
            {
                internalElapsed =
                    TimeSpan.FromMilliseconds(double.Parse(line.AsSpan(internalStartupTimePrefix.Length)));
            }
            else if (line.StartsWith(workingSetPrefix))
            {
                workingSet = double.Parse(line.AsSpan(workingSetPrefix.Length));
            }
        }

        if (isSlimBuilder is null || internalElapsed is null || externalElapsed is null || workingSet is null)
        {
            return null;
        }

        return new ExecuteResult(isSlimBuilder.Value, externalElapsed.Value, internalElapsed.Value, workingSet.Value);
    }

    private static string? GetTargetExecutableFromConfiguration(IConfiguration configuration)
    {
        var targetExecutable = configuration["TargetExecutable"];
        if (string.IsNullOrWhiteSpace(targetExecutable))
        {
            return null;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !targetExecutable.EndsWith(".exe"))
        {
            targetExecutable += ".exe";
        }

        return targetExecutable;
    }

    private static TimeSpan GetIntervalBetweenIterations(IConfiguration configuration) =>
        TimeSpan.TryParse(configuration["IntervalBetweenIterations"], out var parsedInterval) ?
            parsedInterval :
            TimeSpan.Zero;
    
    private static int GetNumberOfIterations(IConfiguration configuration) =>
        int.TryParse(configuration["NumberOfIterations"], out var parsedIterations) ?
            parsedIterations :
            100;
}

public readonly record struct ExecuteResult(
    bool IsSlimBuilder,
    TimeSpan ExternalElapsed,
    TimeSpan InternalElapsed,
    double WorkingSet
);

public readonly record struct StatisticsResult
{
    public StatisticsResult(List<ExecuteResult> executeResults)
    {
        executeResults.MustNotBeNullOrEmpty();
        ExecuteResults = executeResults;
        var externalElapsedSum = TimeSpan.Zero;
        var internalElapsedSum = TimeSpan.Zero;
        var workingSetSum = 0.0;
        foreach (var result in executeResults)
        {
            externalElapsedSum += result.ExternalElapsed;
            internalElapsedSum += result.InternalElapsed;
            workingSetSum += result.WorkingSet;
        }

        MeanExternalElapsed = externalElapsedSum / executeResults.Count;
        MeanInternalElapsed = internalElapsedSum / executeResults.Count;
        MeanWorkingSet = workingSetSum / executeResults.Count;

        var externalSquareDifferenceSum = 0.0;
        var internalSquareDifferenceSum = 0.0;
        var workingSetSquareDifferenceSum = 0.0;
        foreach (var result in executeResults)
        {
            externalSquareDifferenceSum += Math.Pow(
                (result.ExternalElapsed - MeanExternalElapsed).TotalMilliseconds,
                2
            );
            internalSquareDifferenceSum += Math.Pow(
                (result.InternalElapsed - MeanInternalElapsed).TotalMilliseconds,
                2
            );
            workingSetSquareDifferenceSum += Math.Pow(result.WorkingSet - MeanWorkingSet, 2);
        }

        ExternalElapsedStandardDeviation =
            TimeSpan.FromMilliseconds(Math.Sqrt(externalSquareDifferenceSum / executeResults.Count));
        InternalElapsedStandardDeviation =
            TimeSpan.FromMilliseconds(Math.Sqrt(internalSquareDifferenceSum / executeResults.Count));
        WorkingSetStandardDeviation = Math.Sqrt(workingSetSquareDifferenceSum / executeResults.Count);
    }

    public List<ExecuteResult> ExecuteResults { get; }

    public TimeSpan MeanExternalElapsed { get; }
    public TimeSpan ExternalElapsedStandardDeviation { get; }


    public TimeSpan MeanInternalElapsed { get; }
    public TimeSpan InternalElapsedStandardDeviation { get; }
    public double MeanWorkingSet { get; }
    public double WorkingSetStandardDeviation { get; }
}