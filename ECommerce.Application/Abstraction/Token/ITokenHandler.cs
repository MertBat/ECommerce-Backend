using ECommerce.Application.DTOs;

namespace ECommerce.Application.Abstraction.Token
{
    public interface ITokenHandler
    {
        TokenDTO CreateAccessToken(int minute);
    }
}
