namespace plantita.User.Interfaces.REST.Resources;

public record AuthUserResource(Guid Id, string Email,string Name,string LastName,string timeZone,string preferredLanguage);