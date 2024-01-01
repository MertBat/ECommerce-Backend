using ECommerce.Application.Abstraction.Token;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities.Identity;
using Google.Apis.Auth.OAuth2;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly HttpClient _httpClientFactory;

        public FacebookLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _httpClientFactory = httpClientFactory.CreateClient();
        }
        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {

            string accessTokenReponse = await _httpClientFactory.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id=906078141031020&client_secret=c6a330857c2d9fdc29fb5c9534da5410&grant_type=client_credentials");

            FacebookDTO facebookDTO = JsonSerializer.Deserialize<FacebookDTO>(accessTokenReponse);

            string userAccessTokenValidation = await _httpClientFactory.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={request.AuthToken}&access_token={facebookDTO.AccessToken}");

            FacebookTokenValidationDTO validationDTO = JsonSerializer.Deserialize<FacebookTokenValidationDTO>(userAccessTokenValidation);

            if (validationDTO.Data.IsValid)
            {
                string userInfoResponse = await _httpClientFactory.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={request.AuthToken}");

                FacebookPersonalInfoDTO userInfo = JsonSerializer.Deserialize<FacebookPersonalInfoDTO>(userInfoResponse);

                var info = new UserLoginInfo("FACEBOOK", validationDTO.Data.UserId, "FACEBOOK");

                Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                bool result = user != null;
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(userInfo.Email);
                    if (user == null)
                    {
                        user = new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = userInfo.Email,
                            UserName = userInfo.Email,
                            NameSurname = userInfo.Name

                        };
                        var identityResult = await _userManager.CreateAsync(user);
                        result = identityResult.Succeeded;
                    }
                }

                if (result)
                {
                    await _userManager.AddLoginAsync(user, info);
                    TokenDTO token = _tokenHandler.CreateAccessToken(5);
                    return new()
                    {
                        Token = token
                    };
                }
            }

            throw new Exception("Invalid external authentication");
        }
    }
}
