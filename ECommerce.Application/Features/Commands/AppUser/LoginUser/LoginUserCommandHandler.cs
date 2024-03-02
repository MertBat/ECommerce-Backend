using ECommerce.Application.Abstraction.Services.Authentications;
using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        private readonly IInternalAuthentication _authService;

        public LoginUserCommandHandler(IInternalAuthentication authService)
        {
            _authService = authService;
        }
        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {

            TokenDTO token = await _authService.LoginAsync(request.UserNameOrEmail, request.Password, 900);
                return new LoginUserSuccessCommandResponse()
                {
                    Token = token
                };

        }
    }
}
