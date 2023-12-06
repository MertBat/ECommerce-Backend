using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        //Gelen istekler buraya gelir modelstate valid ise bir sonraki filter ı tetikler. Eğer hata varsa hatayı alıp kullanıcıya geri döndürme işlemi yapıyoruz.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Where(x=> x.Value.Errors.Any())
                    .ToDictionary(e=>e.Key, e=>e.Value.Errors.Select(e => e.ErrorMessage));

                context.Result = new BadRequestObjectResult(errors);
                return;
            }

            await next();
        }
    }
}
