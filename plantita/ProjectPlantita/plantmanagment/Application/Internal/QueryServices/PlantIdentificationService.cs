using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;

namespace plantita.ProjectPlantita.plantmanagment.Application.Internal.QueryServices;

public class PlantIdentificationService : IPlantIdentificationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public PlantIdentificationService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

 public async Task<IdentifiedPlantResource?> IdentifyPlantAsync(IFormFile image)
{
    var apiKey = _config["PlantIdApiKey"];
    if (string.IsNullOrWhiteSpace(apiKey)) return null;

    using var ms = new MemoryStream();
    await image.CopyToAsync(ms);
    var base64Image = Convert.ToBase64String(ms.ToArray());

    var payload = new
    {
        images = new[] { $"data:{image.ContentType};base64,{base64Image}" },
        similar_images = true,
        classification_level = "species"
    };

    var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    var request = new HttpRequestMessage(HttpMethod.Post, "https://plant.id/api/v3/identification");
    request.Headers.Add("Api-Key", apiKey);
    request.Content = jsonContent;

    var response = await _httpClient.SendAsync(request);
    if (!response.IsSuccessStatusCode)
    {
        var errorBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Plant.id error: " + errorBody);
        return null;
    }

    var json = await response.Content.ReadAsStringAsync();
    var doc = JsonDocument.Parse(json);

    try
    {
        var first = doc.RootElement
            .GetProperty("result")
            .GetProperty("classification")
            .GetProperty("suggestions")[0];

        var details = first.GetProperty("details");
        var description = details.TryGetProperty("description", out var desc)
            ? desc.GetString()
            : "No description";

        var wikiUrl = details.TryGetProperty("url", out var url)
            ? url.GetString()
            : null;

        var imageUrl = first.GetProperty("similar_images")[0].GetProperty("url").GetString();

        return new IdentifiedPlantResource
        {
            ScientificName = first.GetProperty("name").GetString(),
            CommonName = details.TryGetProperty("common_names", out var names) && names.GetArrayLength() > 0
                ? names[0].GetString()
                : "Unknown",
            Description = description,
            WikiUrl = wikiUrl,
            ImageUrl = imageUrl,
            Watering = "Average",
            Sunlight = "Unknown"
        };
    }
    catch (Exception e)
    {
        Console.WriteLine("Error parsing Plant.id response: " + e.Message);
        return null;
    }
}


}
