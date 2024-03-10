using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.Abstraction.Token;
using ECommerce.Application.DTOs;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Features.Commands.AppUser.LoginUser;
using ECommerce.Domain.Entities.Identity;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ECommerce.Persistance.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserService _userService;

        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, SignInManager<Domain.Entities.Identity.AppUser> signInManager, IUserService userService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _userService = userService;
        }
        public async Task<TokenDTO> FaceBookLoginAsync(string authToken, int accessTokenLifetime)
        {
            string accessTokenReponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLoginSettings:FaceBook:ClientID"]}&client_secret={_configuration["ExternalLoginSettings:FaceBook:ClientSecret"]}&grant_type=client_credentials");

            FacebookDTO? facebookDTO = JsonSerializer.Deserialize<FacebookDTO>(accessTokenReponse);

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookDTO?.AccessToken}");

            FacebookTokenValidationDTO? validationDTO = JsonSerializer.Deserialize<FacebookTokenValidationDTO>(userAccessTokenValidation);

            if (validationDTO?.Data.IsValid != null)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");
                FacebookPersonalInfoDTO? userInfo = JsonSerializer.Deserialize<FacebookPersonalInfoDTO>(userInfoResponse);
                var info = new UserLoginInfo("FACEBOOK", validationDTO.Data.UserId, "FACEBOOK");
                Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                return await CreateUserExternalAsync(user, userInfo.Email, userInfo.Name, info, accessTokenLifetime);
            }
            throw new Exception("Invalid external authentication");
        }


        public async Task<TokenDTO> GoogleLoginAsync(string idToken, int accessTokenLifetime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:ClientID"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");
            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            return await CreateUserExternalAsync(user, payload.Email, payload.Name, info, accessTokenLifetime);
        }

        private async Task<TokenDTO> CreateUserExternalAsync(AppUser user, string email, string name, UserLoginInfo info, int accessTokenLifetime)
        {
            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurname = name

                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
                await _userManager.AddLoginAsync(user, info);
            else
                throw new Exception("Invalid external authentication");

            TokenDTO token = _tokenHandler.CreateAccessToken(accessTokenLifetime, user);
            await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 20);
            return token;
        }
        public async Task<TokenDTO> LoginAsync(string userNameOrEmail, string password, int accessTokenLifetime)
        {
            Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(userNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userNameOrEmail);
            }

            if (user == null)
            {
                throw new NotFoundUserException("Username or password wrong!");
            }

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (result.Succeeded)
            {
                TokenDTO token = _tokenHandler.CreateAccessToken(accessTokenLifetime, user);
                await _userService.UpdateRefreshToken(token.RefreshToken, user,token.Expiration, 20);
                return token;
            }

            throw new AuthenticationErrorException();
        }

        public async Task<TokenDTO> RefreshTokenLogin(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if(user != null && user?.RefreshTokenEndDate > DateTime.UtcNow)
            {
                TokenDTO token = _tokenHandler.CreateAccessToken(900, user);

                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 1300);
                return token;
            }
            else
            {
                throw new NotFoundUserException();
            }
        }
    }
}

