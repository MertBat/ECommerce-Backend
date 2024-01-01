using ECommerce.Application.Abstraction.Token;
using ECommerce.Application.DTOs;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _appUser;
        private readonly ITokenHandler _tokenHandler;

        public GoogleLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> appUser, ITokenHandler tokenHandler)
        {
            _appUser = appUser;
            _tokenHandler = tokenHandler;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { "131707258997-426m0mk6k86gugjff1chjh4n5255r57m.apps.googleusercontent.com" }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);

            Domain.Entities.Identity.AppUser user = await _appUser.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            bool result = user != null;
            if(user == null)
            {
                user = await _appUser.FindByEmailAsync(payload.Email);
                if(user == null )
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        NameSurname = payload.Name

                    };
                    var identityResult = await _appUser.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
                await _appUser.AddLoginAsync(user, info);
            else
                throw new Exception("Invalid external authentication");

            TokenDTO token = _tokenHandler.CreateAccessToken(5);

            return new()
            {
                Token = token,
            };
        }
    }
}
