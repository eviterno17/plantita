using FluentAssertions;

namespace Plantita.CoreTests.Helpers;

/// <summary>
/// Helper methods for common test assertions
/// </summary>
public static class AssertionHelpers
{
    /// <summary>
    /// Assert that a value is within a percentage of an expected value
    /// </summary>
    public static void ShouldBeWithinPercentage(this double actual, double expected, double percentageTolerance)
    {
        var tolerance = Math.Abs(expected * (percentageTolerance / 100.0));
        actual.Should().BeApproximately(expected, tolerance);
    }

    /// <summary>
    /// Assert that a BCrypt password hash is valid
    /// </summary>
    public static void ShouldBeValidBCryptHash(this string hash)
    {
        hash.Should().NotBeNullOrEmpty();
        hash.Should().StartWith("$2a$");
        hash.Length.Should().Be(60); // BCrypt hash length
    }

    /// <summary>
    /// Assert that a collection contains unique items
    /// </summary>
    public static void ShouldHaveUniqueItems<T>(this IEnumerable<T> collection)
    {
        var list = collection.ToList();
        list.Should().OnlyHaveUniqueItems();
    }

    /// <summary>
    /// Assert that a date is recent (within last N minutes)
    /// </summary>
    public static void ShouldBeRecent(this DateTime dateTime, int withinMinutes = 5)
    {
        dateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(withinMinutes));
    }

    /// <summary>
    /// Assert that a sensor reading is within the sensor's range
    /// </summary>
    public static void ShouldBeWithinSensorRange(this double value, double minRange, double maxRange)
    {
        value.Should().BeInRange(minRange, maxRange);
    }

    /// <summary>
    /// Assert that a string is a valid email format
    /// </summary>
    public static void ShouldBeValidEmail(this string email)
    {
        email.Should().NotBeNullOrWhiteSpace();
        email.Should().Contain("@");
        email.Should().Contain(".");
    }

    /// <summary>
    /// Assert that a firmware version follows semantic versioning
    /// </summary>
    public static void ShouldBeValidSemanticVersion(this string version)
    {
        version.Should().NotBeNullOrEmpty();
        version.Should().StartWith("v");
        version.Should().MatchRegex(@"^v\d+\.\d+\.\d+");
    }
}
