namespace plantita.User.Interfaces.REST.Resources;

public record SignUpResource(string Email, string Password,string Name,string LastName,string timeZone,string preferredLanguage,DateTime dateCreated,string Role);