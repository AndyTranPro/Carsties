using System;
using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Utils;
using Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared Collection")]
public class AuctionBusTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private ITestHarness _testHarness;
    public AuctionBusTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
        _testHarness = factory.Services.GetTestHarness();
    }

    [Fact]
    public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
    {
        // Arrange
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelpler.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.True(await _testHarness.Published.Any<AuctionCreated>());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReInitDbForTests(db);
        return Task.CompletedTask;
    }

    private CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            Make = "test",
            Model = "testModel",
            Color = "test",
            Mileage = 10,
            Year = 10,
            ImageUrl = "test",
            AuctionEnd = DateTime.UtcNow.AddDays(10),
            ReservePrice = 10
        };
    }
}
