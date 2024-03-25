using ECommerce.Application.Features.Commands.AuthorizationEndpoints.AsignRole;
using ECommerce.Application.Features.Commands.AuthorizationEndpoints.AsignRoleEndpoint;
using ECommerce.Application.Features.Queries.AuthorizationEndpoint.GetRolesEndpoint;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class AuthorizationAndpointsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorizationAndpointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{Code}")]
        public async Task<IActionResult> GetRolesEndpoint([FromRoute] GetRolesEndpointQueryRequest getRolesEndpointQueryRequest)
        {
            GetRolesEndpointQueryResponse response = await _mediator.Send(getRolesEndpointQueryRequest);
            return Ok(response.Roles);
        }

        [HttpPost]
        public async Task<IActionResult> AsignRoleEndpoint([FromBody] AssignRoleEndpointCommandRequest assignRoleEndpointCommandRequest)
        {
            assignRoleEndpointCommandRequest.Type = typeof(Program);
            AssignRoleEndpointCommandResponse response = await _mediator.Send(assignRoleEndpointCommandRequest);
            return Ok(response);
        }
    }
}
