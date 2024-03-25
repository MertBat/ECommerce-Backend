using ECommerce.Application.DTOs.User;

namespace ECommerce.Application.Abstraction.Services.Authentications
{
    public interface IExternalAuthentication
    {
        Task<TokenDTO> FaceBookLoginAsync(string authToken, int accessTokenLifetime);
        Task<TokenDTO> GoogleLoginAsync(string idToken, int accessTokenLifetime);
    }
}
