using System;
using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared Collection")]
public class AuctionControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private const string GT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";
    public AuctionControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllAuctions_ShouldReturn3Auctions()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("api/auctions");

        // Assert
        Assert.Equal(3, response.Count);
    }

    [Fact]
    public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"api/auctions/{GT_ID}");

        // Assert
        Assert.Equal("GT", response.Model);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidId_ShouldReturn404Response()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ShouldReturn400()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetAsync($"api/auctions/notaguid");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        // Arrange
        var auction = new CreateAuctionDto{Make = "test"};

        // Act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        // Arrange
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelpler.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        Assert.Equal("bob", createdAuction.Seller);
    }

    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // arrange
        var auction = new CreateAuctionDto();
        _httpClient.SetFakeJwtBearerToken(AuthHelpler.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        // arrange
        var updatedAuction = new UpdateAuctionDto {Make = "Updated"};
        _httpClient.SetFakeJwtBearerToken(AuthHelpler.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", updatedAuction);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // arrange 
        var updatedAuction = new UpdateAuctionDto {Make = "Updated"};
        _httpClient.SetFakeJwtBearerToken(AuthHelpler.GetBearerForUser("alice"));

        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", updatedAuction);

        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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
