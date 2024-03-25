using ECommerce.Application.Abstraction.Services.Authentications;
using ECommerce.Application.DTOs.User;
using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    {
        private readonly IExternalAuthentication _authService;

        public GoogleLoginCommandHandler(IExternalAuthentication authService)
        {
            _authService = authService;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {

           TokenDTO token = await _authService.GoogleLoginAsync(request.IdToken, 900);

            return new()
            {
                Token = token,
            };
        }
    }
}
