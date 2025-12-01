using System.Net;

namespace Plantita.AcceptanceTests.Support;

/// <summary>
/// Shared context for test scenarios
/// Stores data that needs to be shared between step definitions
/// </summary>
public class TestContext
{
    // HTTP Response
    public HttpResponseMessage? HttpResponse { get; set; }
    public HttpStatusCode StatusCode => HttpResponse?.StatusCode ?? HttpStatusCode.InternalServerError;
    public string? ResponseBody { get; set; }

    // Authentication
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? CurrentUserEmail { get; set; }
    public int? CurrentUserId { get; set; }

    // Request Data
    public Dictionary<string, object> RequestData { get; set; } = new();

    // Response Data
    public Dictionary<string, object> ResponseData { get; set; } = new();

    // Entity IDs for tracking created resources
    public Dictionary<string, int> EntityIds { get; set; } = new();

    // Test Data Storage
    public Dictionary<string, object> TestData { get; set; } = new();

    // Validation Errors
    public List<string> ValidationErrors { get; set; } = new();

    /// <summary>
    /// Reset the context for a new scenario
    /// </summary>
    public void Reset()
    {
        HttpResponse?.Dispose();
        HttpResponse = null;
        ResponseBody = null;
        AccessToken = null;
        RefreshToken = null;
        CurrentUserEmail = null;
        CurrentUserId = null;
        RequestData.Clear();
        ResponseData.Clear();
        EntityIds.Clear();
        TestData.Clear();
        ValidationErrors.Clear();
    }

    /// <summary>
    /// Store an entity ID for later reference
    /// </summary>
    public void StoreEntityId(string key, int id)
    {
        EntityIds[key] = id;
    }

    /// <summary>
    /// Get a stored entity ID
    /// </summary>
    public int? GetEntityId(string key)
    {
        return EntityIds.TryGetValue(key, out var id) ? id : null;
    }

    /// <summary>
    /// Store test data
    /// </summary>
    public void Store(string key, object value)
    {
        TestData[key] = value;
    }

    /// <summary>
    /// Retrieve test data
    /// </summary>
    public T? Get<T>(string key)
    {
        if (TestData.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Check if context has a specific key
    /// </summary>
    public bool Has(string key)
    {
        return TestData.ContainsKey(key);
    }
}
