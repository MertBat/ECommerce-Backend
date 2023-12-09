using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Storage
{
    public interface IStorage
    {
        Task<List<(string fileName, string pathOrContrainer)>> uploadAsync(string pathOrContrainer, IFormFileCollection files);
        Task DeleteAsync(string pathOrContrainer, string fileName);
        List<string> GetFiles(string pathOrContrainer);
        bool HasFile(string pathOrContrainer, string fileName);
    }
}
