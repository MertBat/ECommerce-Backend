using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

        public CreateUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id= Guid.NewGuid().ToString(),
                UserName = request.UserName,
                Email = request.EMail,
                NameSurname = request.Name,

            }, request.Password);

            CreateUserCommandResponse response = new CreateUserCommandResponse() { Succeeded = result.Succeeded};

            if(result.Succeeded)
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
    }
}
