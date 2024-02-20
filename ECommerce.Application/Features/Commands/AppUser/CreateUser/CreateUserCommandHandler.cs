using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IUserService userService )
        {
            _userService = userService;
        }
        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            CreateUserResponseDTO response = await _userService.CreateAsync(new()
            {
                EMail = request.EMail,
                Name = request.Name,
                Password = request.Password,
                PasswordAgain = request.PasswordAgain,
                UserName = request.UserName,
            });

            return new() { 
                Message = response.Message,
                Succeeded = response.Succeeded,
            };
        }
    }
}
