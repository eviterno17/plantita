using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using plantita.ProjectPlantita.plantmanagment.Application.Internal.QueryServices;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;
using Xunit;

namespace plantita.Tests.Unit.ProjectPlantita.PlantManagement;

public class PlantIdentificationServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly PlantIdentificationService _service;

    public PlantIdentificationServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        _mockConfiguration.Setup(c => c["PlantIdApiKey"]).Returns("test-api-key");
        _service = new PlantIdentificationService(_httpClient, _mockConfiguration.Object);
    }

    [Fact]
    public async Task IdentifyPlantAsync_WithValidImage_ReturnsIdentifiedPlant()
    {
        // Arrange
        var mockImage = CreateMockFormFile("test-image.jpg", "image/jpeg");
        var apiResponse = @"{
            ""result"": {
                ""classification"": {
                    ""suggestions"": [
                        {
                            ""name"": ""Rosa chinensis"",
                            ""details"": {
                                ""common_names"": [""China Rose""],
                                ""description"": ""A beautiful flowering plant"",
                                ""url"": ""https://en.wikipedia.org/wiki/Rosa_chinensis""
                            },
                            ""similar_images"": [
                                {
                                    ""url"": ""https://example.com/rose.jpg""
                                }
                            ]
                        }
                    ]
                }
            }
        }";

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(apiResponse, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _service.IdentifyPlantAsync(mockImage);

        // Assert
        result.Should().NotBeNull();
        result!.ScientificName.Should().Be("Rosa chinensis");
        result.CommonName.Should().Be("China Rose");
        result.Description.Should().Be("A beautiful flowering plant");
        result.WikiUrl.Should().Be("https://en.wikipedia.org/wiki/Rosa_chinensis");
        result.ImageUrl.Should().Be("https://example.com/rose.jpg");
    }

    [Fact]
    public async Task IdentifyPlantAsync_WithMissingApiKey_ReturnsNull()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["PlantIdApiKey"]).Returns((string?)null);
        var service = new PlantIdentificationService(_httpClient, mockConfig.Object);
        var mockImage = CreateMockFormFile("test-image.jpg", "image/jpeg");

        // Act
        var result = await service.IdentifyPlantAsync(mockImage);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task IdentifyPlantAsync_WithApiError_ReturnsNull()
    {
        // Arrange
        var mockImage = CreateMockFormFile("test-image.jpg", "image/jpeg");

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("API Error", Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _service.IdentifyPlantAsync(mockImage);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task IdentifyPlantAsync_WithMalformedResponse_ReturnsNull()
    {
        // Arrange
        var mockImage = CreateMockFormFile("test-image.jpg", "image/jpeg");
        var apiResponse = @"{""invalid"": ""json""}";

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(apiResponse, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _service.IdentifyPlantAsync(mockImage);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task IdentifyPlantAsync_WithNoCommonName_ReturnsUnknown()
    {
        // Arrange
        var mockImage = CreateMockFormFile("test-image.jpg", "image/jpeg");
        var apiResponse = @"{
            ""result"": {
                ""classification"": {
                    ""suggestions"": [
                        {
                            ""name"": ""Plantus unknownus"",
                            ""details"": {
                                ""description"": ""Unknown plant""
                            },
                            ""similar_images"": [
                                {
                                    ""url"": ""https://example.com/plant.jpg""
                                }
                            ]
                        }
                    ]
                }
            }
        }";

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(apiResponse, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _service.IdentifyPlantAsync(mockImage);

        // Assert
        result.Should().NotBeNull();
        result!.CommonName.Should().Be("Unknown");
    }

    [Fact]
    public async Task IdentifyPlantAsync_VerifiesApiKeyInHeaders()
    {
        // Arrange
        var mockImage = CreateMockFormFile("test-image.jpg", "image/jpeg");
        HttpRequestMessage? capturedRequest = null;

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{
                    ""result"": {
                        ""classification"": {
                            ""suggestions"": [{
                                ""name"": ""Test"",
                                ""details"": {},
                                ""similar_images"": [{""url"": ""test.jpg""}]
                            }]
                        }
                    }
                }", Encoding.UTF8, "application/json")
            });

        // Act
        await _service.IdentifyPlantAsync(mockImage);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.Headers.GetValues("Api-Key").Should().Contain("test-api-key");
        capturedRequest.RequestUri!.ToString().Should().Be("https://plant.id/api/v3/identification");
    }

    private static IFormFile CreateMockFormFile(string fileName, string contentType)
    {
        var content = "fake-image-data"u8.ToArray();
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "image", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
