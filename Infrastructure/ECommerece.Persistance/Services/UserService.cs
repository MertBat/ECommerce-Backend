using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.DTOs;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Features.Commands.AppUser.CreateUser;
using ECommerce.Application.Helpers;
using ECommerce.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<Domain.Entities.Identity.AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<CreateUserResponseDTO> CreateAsync(CreateUserDTO createUser)
        {

            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = createUser.UserName,
                Email = createUser.EMail,
                NameSurname = createUser.Name,

            }, createUser.Password);

            CreateUserResponseDTO response = new CreateUserResponseDTO() { Succeeded = result.Succeeded };

            if (result.Succeeded)
            {
                response.Succeeded = true;
                response.Message = "User successfuly created";
            }
            else
            {
                response.Succeeded = false;
                foreach (var error in result.Errors)
                {
                    response.Message += $"{error.Code} - {error.Description}<br>";
                }
                response.Message = "User successfuly created";
            }

            return response;
        }

        public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = resetToken.UrlDecode();
                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                if (result.Succeeded)
                {
                   await _userManager.UpdateSecurityStampAsync(user);
                }
                else
                {
                    throw new PasswordChangeFailedException();
                }
            }
        }

        public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int AddTimeOnAccessToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenEndDate = accessTokenDate.AddSeconds(AddTimeOnAccessToken);
            await _userManager.UpdateAsync(user);
        }
    }
}
