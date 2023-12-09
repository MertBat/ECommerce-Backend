using ECommerce.Infastructure.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infastructure.Services.Storage
{
    public class Storage
    {
        protected string FileRename(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string oldName = Path.GetFileNameWithoutExtension(fileName);
            string guidName = Guid.NewGuid().ToString();
            return $"{guidName}-{NameOperation.CharacterRegulator(oldName)}{extension}";

        }
    }
}
