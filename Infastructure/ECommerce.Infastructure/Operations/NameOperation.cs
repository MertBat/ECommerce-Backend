using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infastructure.Operations
{
    public static class NameOperation
    {
        public static string CharacterRegulator(string name)
        {
            return name.Replace("\"", "")
                       .Replace("ı", "i")
                       .Replace("ğ", "g")
                       .Replace("ü", "u")
                       .Replace("ş", "s")
                       .Replace("ö", "o")
                       .Replace("ç", "c")
                       .Replace("ö", "o")
                       .Replace("*", "")
                       .Replace("-", "")
                       .Replace("/", "")
                       .Replace("+", "")
                       .Replace(".", "")
                       .Replace(";", "")
                       .Replace(",", "")
                       .Replace("?", "")
                       .Replace("!", "")
                       .Replace("#", "")
                       .Replace("^", "")
                       .Replace("%", "")
                       .Replace("$", "")
                       .Replace("&", "")
                       .Replace("{", "")
                       .Replace("}", "")
                       .Replace("_", "")
                       .Replace("<", "")
                       .Replace(">", "")
                       .Replace("'", "")
                       .Replace("é", "")
                       .Replace("₺", "")
                       .Replace("~", "")
                       .Replace("`", "")
                       .Replace("¨", "");
        }              
    }                  
}                      
                       