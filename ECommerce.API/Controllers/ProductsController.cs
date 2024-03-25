using ECommerce.Application.Features.Commands.Product.CreateProduct;
using ECommerce.Application.Features.Commands.Product.DeleteProduct;
using ECommerce.Application.Features.Commands.Product.UpdateProduct;
using ECommerce.Application.Features.Commands.ProductImageFile.DeleteProductImage;
using ECommerce.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ECommerce.Application.Features.Queries.Product.GetAllProduct;
using ECommerce.Application.Features.Queries.Product.GetByIdProduct;
using ECommerce.Application.Features.Queries.ProductImageFile.GetProductImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using ECommerce.Application.Features.Commands.ProductImageFile.ChangeShowcaseImage;
using ECommerce.Application.Constants;
using ECommerce.Application.CustomAttributes;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProductById([FromRoute] GetByIdProductQueryRequest getByIdProductQuery)
        {
            GetByIdProductQueryResponse response = await _mediator.Send(getByIdProductQuery);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Products, ActionType = Application.Enums.ActionType.Update, Definition = "Update Product")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateSingleProductCommandRequest updateProductCommandRequest)
        {
            UpdateSingleProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);
            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Products, ActionType = Application.Enums.ActionType.Write, Definition = "Create Product")]
        public async Task<IActionResult> CreateProduct(CreateSingleProductCommandRequest createProductCommandRequest)
        {
            CreateSingleProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Products, ActionType = Application.Enums.ActionType.Delete, Definition = "Delete Product")]
        public async Task<IActionResult> Delete([FromRoute] DeleteSingleProductCommandRequest deleteProductCommandRequest)
        {
            DeleteSingleProductCommandResponse results = await _mediator.Send(deleteProductCommandRequest);
            return Ok();
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Products, ActionType = Application.Enums.ActionType.Write, Definition = "Upload Product Image")]
        public async Task<IActionResult> Upload([FromQuery] UploadProductImageCommandRequest uploadProductImageCommandRequest)
        {
            uploadProductImageCommandRequest.FormCollection = Request.Form.Files;
            await _mediator.Send(uploadProductImageCommandRequest);
            return Ok();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProductImages([FromRoute] GetProductImageQueryRequest getProductImageQueryRequest)
        {

            List<GetProductImageQueryResponse> response = await _mediator.Send(getProductImageQueryRequest);
            return Ok(response);
        }

        [HttpDelete("[action]/{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Products, ActionType = Application.Enums.ActionType.Delete, Definition = "Delete Product Image")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] DeleteProductImageCommandRequest deleteProductImageCommandRequest, [FromQuery] string imageId)
        {
            deleteProductImageCommandRequest.ImageId = imageId;
            await _mediator.Send(deleteProductImageCommandRequest);
            return Ok();
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Products, ActionType = Application.Enums.ActionType.Update, Definition = "Update Products ShowCase Image")]
        public async Task<IActionResult> changeShowcase([FromQuery] ChangeShowcaseImageCommandRequest changeShowcaseImageCommandRequest)
        {
            ChangeShowcaseImageCommandResponse response = await _mediator.Send(changeShowcaseImageCommandRequest);
            return Ok(response);
        }
    }
}
