using ECommerce.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandResponse
    {
        public TokenDTO Token { get; set; }
    }
}
