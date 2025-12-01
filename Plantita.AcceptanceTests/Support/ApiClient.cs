using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Plantita.AcceptanceTests.Support;

/// <summary>
/// API Client for making HTTP requests during tests
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TestContext _testContext;

    public ApiClient(HttpClient httpClient, TestContext testContext)
    {
        _httpClient = httpClient;
        _testContext = testContext;
    }

    /// <summary>
    /// Send GET request
    /// </summary>
    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.GetAsync(endpoint);
        await StoreResponse(response);
        return response;
    }

    /// <summary>
    /// Send POST request with JSON body
    /// </summary>
    public async Task<HttpResponseMessage> PostAsync(string endpoint, object? data = null)
    {
        AddAuthorizationHeader();

        HttpContent? content = null;
        if (data != null)
        {
            var json = JsonConvert.SerializeObject(data);
            content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        var response = await _httpClient.PostAsync(endpoint, content);
        await StoreResponse(response);
        return response;
    }

    /// <summary>
    /// Send PUT request with JSON body
    /// </summary>
    public async Task<HttpResponseMessage> PutAsync(string endpoint, object data)
    {
        AddAuthorizationHeader();

        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(endpoint, content);
        await StoreResponse(response);
        return response;
    }

    /// <summary>
    /// Send DELETE request
    /// </summary>
    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.DeleteAsync(endpoint);
        await StoreResponse(response);
        return response;
    }

    /// <summary>
    /// Send POST request with form data
    /// </summary>
    public async Task<HttpResponseMessage> PostFormAsync(string endpoint, Dictionary<string, string> formData)
    {
        AddAuthorizationHeader();

        var content = new FormUrlEncodedContent(formData);
        var response = await _httpClient.PostAsync(endpoint, content);
        await StoreResponse(response);
        return response;
    }

    /// <summary>
    /// Add Authorization header if token is available
    /// </summary>
    private void AddAuthorizationHeader()
    {
        _httpClient.DefaultRequestHeaders.Remove("Authorization");

        if (!string.IsNullOrEmpty(_testContext.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testContext.AccessToken);
        }
    }

    /// <summary>
    /// Store response in test context
    /// </summary>
    private async Task StoreResponse(HttpResponseMessage response)
    {
        _testContext.HttpResponse = response;
        _testContext.ResponseBody = await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Get response content as typed object
    /// </summary>
    public async Task<T?> GetResponseContent<T>()
    {
        if (_testContext.HttpResponse == null)
            return default;

        var content = await _testContext.HttpResponse.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content))
            return default;

        return JsonConvert.DeserializeObject<T>(content);
    }

    /// <summary>
    /// Set authentication token
    /// </summary>
    public void SetAuthToken(string token)
    {
        _testContext.AccessToken = token;
    }

    /// <summary>
    /// Clear authentication token
    /// </summary>
    public void ClearAuthToken()
    {
        _testContext.AccessToken = null;
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
    }
}
