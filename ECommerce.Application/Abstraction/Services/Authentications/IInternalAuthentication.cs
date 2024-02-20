using ECommerce.Application.DTOs;

namespace ECommerce.Application.Abstraction.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<TokenDTO> LoginAsync(string userNameOrEmail, string password, int accessTokenLifetime);
        Task<TokenDTO> RefreshTokenLogin(string refreshToken);
    }
}
