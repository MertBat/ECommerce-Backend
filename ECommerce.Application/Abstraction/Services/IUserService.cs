using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Services
{
    public interface IUserService
    {
        Task<CreateUserResponseDTO> CreateAsync(CreateUserDTO createUser);
        Task UpdateRefreshToken(string refreshToken, AppUser user, DateTime accessTokenDate, int AddTimeOnAccessToken);
    }
}
