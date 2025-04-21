namespace SchoolCoreAPI.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(string userId);
        Task<string> GenerateRefreshToken();
        Task<bool> ValidateToken(string token);
       


    }
}
