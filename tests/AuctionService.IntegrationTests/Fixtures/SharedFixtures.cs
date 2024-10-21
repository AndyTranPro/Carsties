using System;

namespace AuctionService.IntegrationTests.Fixtures;

[CollectionDefinition("Shared Collection")]
public class SharedFixture : ICollectionFixture<CustomWebAppFactory>
{

}
