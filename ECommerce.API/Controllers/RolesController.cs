using ECommerce.Application.Constants;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.Enums;
using ECommerce.Application.Features.Commands.Role.CreateRole;
using ECommerce.Application.Features.Commands.Role.DeleteRole;
using ECommerce.Application.Features.Commands.Role.UpdateRole;
using ECommerce.Application.Features.Queries.Role.GetAllRoles;
using ECommerce.Application.Features.Queries.Role.GetByIdRole;
using ECommerce.Application.Features.Queries.Role.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("allRoles")]
        [AuthorizeDefinition(ActionType = ActionType.Read, Definition = "Get Roles without pagination", Menu = AuthorizeDefinitionConstant.Roles)]
        public async Task<IActionResult> GetAllRoles([FromQuery] GetAllRolesQueryRequest getAllRolesQueryRequest)
        {
            GetAllRolesQueryResponse response = await _mediator.Send(getAllRolesQueryRequest);
            return Ok(response);
        }

        [HttpGet("roles")]
        [AuthorizeDefinition(ActionType = ActionType.Read, Definition = "Get Roles with pagination", Menu = AuthorizeDefinitionConstant.Roles)]
        public async Task<IActionResult> GetRoles([FromQuery] GetRolesQueryRequest getRolesQueryRequest)
        {
            GetRolesQueryResponse response = await _mediator.Send(getRolesQueryRequest);
            return Ok(response);
        }

        [HttpGet("role/{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Read, Definition = "Get Role By Id", Menu = AuthorizeDefinitionConstant.Roles)]
        public async Task<IActionResult> GetRoleById([FromRoute] GetByIdRoleQueryRequest getByIdRoleQueryRequest)
        {
            GetByIdRoleQueryResponse response = await _mediator.Send(getByIdRoleQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [AuthorizeDefinition(ActionType = ActionType.Write, Definition = "Create Role", Menu = AuthorizeDefinitionConstant.Roles)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommandRequest createRoleCommandRequest)
        {
            CreateRoleCommandResponse response = await _mediator.Send(createRoleCommandRequest);
            return Ok(response);
        }

        [HttpPut]
        [AuthorizeDefinition(ActionType = ActionType.Update, Definition = "Update Role", Menu = AuthorizeDefinitionConstant.Roles)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommandRequest updateRoleCommandRequest)
        {
            UpdateRoleCommandResponse response = await _mediator.Send(updateRoleCommandRequest);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Delete, Definition = "Delete Role", Menu = AuthorizeDefinitionConstant.Roles)]
        public async Task<IActionResult> RemoveRole([FromRoute] DeleteRoleCommandRequest deleteRoleCommandRequest)
        {
            DeleteRoleCommandResponse response = await _mediator.Send(deleteRoleCommandRequest);
            return Ok(response);
        }
    }
}
