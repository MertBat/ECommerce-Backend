using ECommerce.Application.DTOs.User;
using ECommerce.Domain.Entities.Identity;

namespace ECommerce.Application.Abstraction.Token
{
    public interface ITokenHandler
    {
        TokenDTO CreateAccessToken(int second, AppUser appUser);
        string CreateRefreshToken();
    }
}
