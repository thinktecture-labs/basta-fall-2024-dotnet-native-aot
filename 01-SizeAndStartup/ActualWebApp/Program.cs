using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

var startingTimestamp = Stopwatch.GetTimestamp();

#if IS_NATIVE_AOT
var builder = WebApplication.CreateSlimBuilder(args);
const bool isSlimBuilder = true;
#else
var builder = WebApplication.CreateBuilder(args);
const bool isSlimBuilder = false;
#endif

await using var app = builder.Build();
app.MapGet("/", () => "Hello World!");

await app.StartAsync();

var endingTimestamp = Stopwatch.GetTimestamp();
var internalElapsed = Stopwatch.GetElapsedTime(startingTimestamp, endingTimestamp);
Console.WriteLine($"IsSlimBuilder: {isSlimBuilder}");
Console.WriteLine($"Ending timestamp: {endingTimestamp}");
Console.WriteLine($"Internal Startup Time in ms: {internalElapsed.TotalMilliseconds:N3}");
double workingSet = Process.GetCurrentProcess().WorkingSet64;
Console.WriteLine($"Working Set in MB: {workingSet / (1024 * 1024):N3}");
await app.StopAsync();
