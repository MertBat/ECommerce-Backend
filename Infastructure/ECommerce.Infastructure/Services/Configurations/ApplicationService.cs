using ECommerce.Application.Abstraction.Services.Configurations;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.DTOs.Configuration;
using ECommerce.Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infastructure.Services.Configurations
{
    internal class ApplicationService : IApplicationService
    {
        public List<MenuDTO> GetAuthorizeDefinitionEndpoints(Type type)
        {
            //GetExecutingAssembly(); yazıldığında bulunduğu assambly verir.
            Assembly assembly = Assembly.GetAssembly(type);
            //controllerbase referansından türeyenleri yani controllerlarımı filitreliyorum.
            var controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase)));

            List<MenuDTO> menus = new();

            if (controllers != null) { }
            {
                foreach (var controller in controllers)
                {
                    var actions = controller.GetMethods().Where(m => m.IsDefined(typeof(AuthorizeDefinitionAttribute)));
                    if (actions != null)
                    {
                        foreach (var action in actions)
                        {
                            var attributes = action.GetCustomAttributes(true);
                            if (attributes != null)
                            {
                                MenuDTO menuDTO = new();

                                var authorizeDefinitionAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;

                                if (!menus.Any(m => m.Name == authorizeDefinitionAttribute.Menu))
                                {
                                    menuDTO = new() { Name = authorizeDefinitionAttribute.Menu, };
                                    menus.Add(menuDTO);
                                }
                                else
                                    menuDTO = menus.FirstOrDefault(m => m.Name == authorizeDefinitionAttribute.Menu);

                                ActionDTO actionDTO = new()
                                {
                                    ActionType = Enum.GetName(typeof(ActionType), authorizeDefinitionAttribute.ActionType),
                                    Definition = authorizeDefinitionAttribute.Definition,
                                };

                                var httpAttribute = attributes.FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpMethodAttribute;

                                if (httpAttribute != null)
                                    actionDTO.HttpType = httpAttribute.HttpMethods.First();
                                else
                                    actionDTO.HttpType = HttpMethods.Get;

                                actionDTO.Code = $"{actionDTO.HttpType}.{actionDTO.ActionType}.{actionDTO.Definition.Replace(" ", "")}";

                                menuDTO.Actions.Add(actionDTO);
                            }
                        }
                    }
                }
            }


            return menus;
        }
    }
}
