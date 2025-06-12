using Moq;
using ProductionERP_MinAPI.Model;
using System.Net;

namespace ProductionERP_MinAPI.Tests
{
    public class MaterialApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public MaterialApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task PostMaterial_ReturnsCreated_WhenPublishSucceeds()
        {
            // Arrange
            var material = new Material { MaterialID = 1, MaterialName = "TestMaterial", CountryCode = "UK" };

            _factory.BusServiceMock
                .Setup(svc => svc.PublishAsync(It.IsAny<Material>()))
                .ReturnsAsync($"Processed item of type Material: {material}");

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(material),
                System.Text.Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/material", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            _factory.BusServiceMock.Verify(svc => svc.PublishAsync(It.IsAny<Material>()), Times.Once);
        }
    }
}