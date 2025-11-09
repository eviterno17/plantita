namespace plantita.User.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<Guid> CreateAuthUser(string email, string password,string name,string lastname,string timeZone,string preferredLanguage,DateTime datecreatedat,string role);
    Task<Guid> FetchAuthUserIdByEmail(string email);
    Task<string> FetchAuthUsernameByUserId(Guid userId);
}