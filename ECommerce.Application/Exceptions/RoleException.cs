using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Exceptions
{
    public class RoleException :Exception
    {
        public RoleException() : base("Role Error!")
        {
        }

        public RoleException(string? message) : base(message)
        {
        }

        public RoleException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
