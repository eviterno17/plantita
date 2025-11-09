namespace plantita.User.Domain.Model.Commands;

public record SignUpCommand(string Email, string Password, string Name,string LastName, string TimeZone, string PreferredLanguage ,DateTime DateCreatedAt, string Role = "User");
