using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using Testcontainers.PostgreSql;
using WebApp.Contacts.GetContacts;
using Xunit;
using Xunit.Abstractions;

namespace WebApp.Tests;

public sealed class NativeAotIntegrationTests : IAsyncLifetime
{
    private readonly INetwork _network;
    private readonly ITestOutputHelper _output;
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly IContainer _webAppContainer;
    private readonly IFutureDockerImage _webAppImage;

    public NativeAotIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        
        _network = new NetworkBuilder().Build();

        _postgresContainer = new PostgreSqlBuilder()
           .WithNetwork(_network)
           .WithNetworkAliases("postgres")
           .WithDatabase("ContactsDb")
           .WithUsername("webapp")
           .WithPassword("password")
           .Build();

        _webAppImage = new ImageFromDockerfileBuilder()
           .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "WebApp")
           .WithDockerfile("Dockerfile-alpine-nativeaot")
           .WithName("nativeaot-webapp")
           .Build();

        _webAppContainer = new ContainerBuilder()
           .WithNetwork(_network)
           .WithImage("nativeaot-webapp")
           .WithEnvironment(
                "ConnectionStrings__Default",
                "Server=postgres; Port=5432; Database=ContactsDb; User Id=webapp; Password=password"
            )
           .WithEnvironment("ASPNETCORE_URLS", "http://*:5000")
           .WithEnvironment("LogSettings__FormattingType", "CompactJson")
           .WithEnvironment("LogSettings__DefaultLevel", "Information")
           .WithEnvironment("LogSettings__Overrides__0__Namespace", "Microsoft.AspNetCore")
           .WithEnvironment("LogSettings__Overrides__0__Level", "Warning")
           .WithPortBinding(5000, true)
           .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(5000)))
           .Build();
    }

    public async Task InitializeAsync()
    {
        
        _output.WriteLine("Creating network...");
        await _network.CreateAsync();
        _output.WriteLine("Starting Postgres container...");
        await _postgresContainer.StartAsync();
        _output.WriteLine("Creating web app image...");
        await _webAppImage.CreateAsync();
        _output.WriteLine("Starting web app...");
        await _webAppContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _webAppContainer.StopAsync();
        await _webAppContainer.DisposeAsync();
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
        await _network.DeleteAsync();
        await _webAppImage.DeleteAsync();
    }

    [Fact]
    public async Task GetContactsTest()
    {
        using var client = new HttpClient();
        var uri = new UriBuilder(
            Uri.UriSchemeHttp,
            _webAppContainer.Hostname,
            _webAppContainer.GetMappedPublicPort(5000),
            "api/contacts"
        ).Uri;

        var contacts = await client.GetFromJsonAsync<List<ContactListDto>>(uri);
        contacts.Should().HaveCount(3);
        foreach (var contact in contacts!)
        {
            _output.WriteLine(contact.ToString());
        }
    }
}