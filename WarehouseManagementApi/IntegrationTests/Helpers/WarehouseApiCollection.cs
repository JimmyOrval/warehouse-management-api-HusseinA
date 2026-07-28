using IntegrationTests.Helpers.Dependencies;

namespace IntegrationTests.Helpers;

// since we have stuff like auth, background jobs...
// I use this in every test instead of creating it each time
[CollectionDefinition("Warehouse API")]
public class WarehouseApiCollection : ICollectionFixture<CustomWebApplicationFactory>;