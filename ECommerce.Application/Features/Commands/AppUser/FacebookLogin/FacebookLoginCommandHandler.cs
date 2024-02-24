using ECommerce.Application.Abstraction.Services.Authentications;
using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        private readonly IExternalAuthentication _authService;

        public FacebookLoginCommandHandler(IExternalAuthentication authService)
        {
            _authService = authService;
        }
        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            TokenDTO token = await _authService.FaceBookLoginAsync(request.AuthToken, 500);
            return new()
            {
                Token = token
            };
        }
    }
}

