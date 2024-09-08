# .NET Native AOT - ready for my Web APIs?

This repo contains the slides and example code for the talk ".NET Native AOT - ready for my Web APIs?" held at BASTA! Fall 2024 in Mainz, Germany.

All analysis results can be found in the `Analysis` folder.

## Prerequisites

You require the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for the code examples. The code can run on multiple platforms and has been tested on Windows and Ubuntu. If you experience problems, please [create an issue](https://github.com/thinktecture-labs/basta-fall-2024-dotnet-native-aot/issues/new) in this repo.

## How to run the example code

### Example 1 - Memory, App Size, Startup Time

In the subfolder `01-SizeAndStartup`, you can find two minimal ASP.NET Core apps: the ActualWebApp project represents an ASP.NET Core app which prints out the startup time interval and the memory usage. The BenchmarkApp is a console app which will call actual app 100 times, capturing the startup time and memory usage, and then creating an HTML file with the results, including the statistical mean and standard deviation.

To run the actual app in Native AOT mode, use the following commands:

```bash
# Make sure you are in the root folder of the repo
cd 01-SizeAndStartup/ActualWebApp
dotnet publish -o bin/native-aot
cd bin/native-aot
./ActualWebApp # append .exe on Windows
```

Alternatively, you can run the actual app in regular CLR mode:

```bash
# Make sure you are in the root folder of the repo
cd 01-SizeAndStartup/ActualWebApp
dotnet publish -o bin/regular-clr /p:PublishAot=false
cd bin/regular-clr
./ActualWebApp # append .exe on Windows
```

The benchmark app should always be run in Native AOT mode:

```bash
cd 01-SizeAndStartup/BenchmarkApp
dotnet publish -o bin/native-aot
cd bin/native-aot
./BenchmarkApp # append .exe on Windows
```

By default, the benchmark app will call into the native aot actual app. You can adjust this by updating appsettings.json, passing corresponding arguments to the BenchmarkApp, or by setting environment variables. To run the alpine and ubuntu docker containers, use the Dockerfile-benchmark-xxx files:

```bash
cd 01-SizeAndStartup
docker build -f Dockerfile-benchmark-alpine-nativeaot -t benchmark-alpine-nativeaot . # add --build-arg RUNTIME_ID=linux-musl-arm64 when running on an ARM CPU
docker run -it --rm benchmark-alpine-nativeaot
> cd benchmark-app
> ./BenchmarkApp
> cat Execution \Results \<timestamp>.html
```

```bash
cd 01-SizeAndStartup
docker build -f Dockerfile-benchmark-ubuntu-nativeaot -t benchmark-ubuntu-nativeaot . # add --build-arg RUNTIME_ID=linux-arm64 when running on an ARM CPU
docker run -it --rm benchmark-ubuntu-nativeaot
> cd benchmark-app
> ./BenchmarkApp
> cat Execution \Results \<timestamp>.html
```

If you want to check the different sizes of docker files, simply run `docker build` on the different Dockerfiles in `01-SizeAndStartup/ActualWebApp`.

### Example 2 - ASP.NET Core Web API with Native AOT

This web app represents a full example with ADO.NET data access, mediator pattern, Object-To-Object mapping.

To set up the database, run the following command:

```bash
# Make sure you are in the root folder of the repo
docker compose -f docker-compose.postgres-dev.yml up -d
```

Then, you can run the app:

```bash
# Make sure you are in the root folder of the repo
cd WebApp
dotnet publish -o bin/native-aot
cd bin/native-aot
./WebApp # append .exe on Windows
```

You can then use the `requests.http` file to call different Minimal API endpoints. .http files are supported out-of-the-box by Visual Studio, Visual Studio Code, and JetBrains Rider, amongst others. To run the app in a docker container, check the Dockerfiles for the web app and use `docker-compose.nativeaot.yml` or `docker-compose.regular.yml`.

If you want to run the throughput test, then please checkout branch `in-memory-spike-test`, start up the app (either on the host or in a docker container, localhost:5000 will be targeted) and run `k6 run ./spike-test.js`.