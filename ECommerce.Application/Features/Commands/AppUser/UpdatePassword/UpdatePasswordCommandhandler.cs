using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordCommandhandler : IRequestHandler<UpdatePasswordCommandRequest, UpdatePasswordCommandResponse>
    {
        private readonly IUserService _userService;

        public UpdatePasswordCommandhandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<UpdatePasswordCommandResponse> Handle(UpdatePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.PasswordConfirm)
                throw new PasswordChangeFailedException("Passwords are not matching");

            await _userService.UpdatePasswordAsync(request.UserId, request.ResetToken, request.NewPassword);
            return new();
        }
    }
}
