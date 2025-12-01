using BoDi;
using Plantita.AcceptanceTests.Support;
using TechTalk.SpecFlow;

namespace Plantita.AcceptanceTests.Hooks;

/// <summary>
/// SpecFlow hooks for test lifecycle management
/// </summary>
[Binding]
public class TestHooks
{
    private static CustomWebApplicationFactory? _factory;
    private static HttpClient? _httpClient;
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Runs once before all tests
    /// </summary>
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        // Create the test server
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
        _serviceProvider = _factory.Services;

        Console.WriteLine("✓ Test server started");
    }

    /// <summary>
    /// Runs once after all tests
    /// </summary>
    [AfterTestRun]
    public static void AfterTestRun()
    {
        // Cleanup
        _httpClient?.Dispose();
        _factory?.Dispose();

        Console.WriteLine("✓ Test server stopped");
    }

    /// <summary>
    /// Runs before each scenario
    /// </summary>
    [BeforeScenario]
    public void BeforeScenario(IObjectContainer objectContainer, ScenarioContext scenarioContext)
    {
        if (_httpClient == null || _serviceProvider == null)
        {
            throw new InvalidOperationException("Test infrastructure not initialized");
        }

        // Create test context for this scenario
        var testContext = new TestContext();
        objectContainer.RegisterInstanceAs(testContext);

        // Create API client
        var apiClient = new ApiClient(_httpClient, testContext);
        objectContainer.RegisterInstanceAs(apiClient);

        // Create database helper
        var databaseHelper = new DatabaseHelper(_serviceProvider);
        objectContainer.RegisterInstanceAs(databaseHelper);

        // Reset database before each scenario
        databaseHelper.ResetDatabase().Wait();

        Console.WriteLine($"▶ Starting scenario: {scenarioContext.ScenarioInfo.Title}");
    }

    /// <summary>
    /// Runs after each scenario
    /// </summary>
    [AfterScenario]
    public void AfterScenario(ScenarioContext scenarioContext, TestContext testContext)
    {
        var status = scenarioContext.TestError == null ? "✓ PASSED" : "✗ FAILED";
        Console.WriteLine($"{status}: {scenarioContext.ScenarioInfo.Title}");

        if (scenarioContext.TestError != null)
        {
            Console.WriteLine($"Error: {scenarioContext.TestError.Message}");
        }

        // Clean up test context
        testContext.Reset();
    }

    /// <summary>
    /// Runs before each step (useful for debugging)
    /// </summary>
    [BeforeStep]
    public void BeforeStep(ScenarioContext scenarioContext)
    {
        // Uncomment for detailed step logging
        // Console.WriteLine($"  → {scenarioContext.StepContext.StepInfo.Text}");
    }

    /// <summary>
    /// Runs after each step (useful for debugging)
    /// </summary>
    [AfterStep]
    public void AfterStep(ScenarioContext scenarioContext)
    {
        // Log step errors
        if (scenarioContext.TestError != null)
        {
            Console.WriteLine($"  ✗ Step failed: {scenarioContext.StepContext.StepInfo.Text}");
        }
    }
}
