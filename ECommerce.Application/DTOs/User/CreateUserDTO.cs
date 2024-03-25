using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.User
{
    public class CreateUserDTO
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
        public string PasswordAgain { get; set; }
    }
}
