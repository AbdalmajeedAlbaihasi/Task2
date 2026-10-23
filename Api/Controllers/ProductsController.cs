using Api.DTOs;
using Asp.Versioning;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductBusiness _productBusiness;

        public ProductsController(ProductBusiness productBusiness)
        {
            _productBusiness = productBusiness;
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productBusiness.GetProducts(pageNumber, pageSize);

            var response = products.Select(product => new ProductResponseDto(
                product.Id,
                product.Name,
                product.Price,
                product.Category?.Name ?? string.Empty
            ));

            return Ok(response);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequestDto request)
        {
            try
            {
                var success = await _productBusiness.UpdateProduct(id, request.Name, request.Price, request.RowVersion);

                if (!success)
                    return NotFound(new { message = "Product not found." });

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {

                return Conflict(new { message = "The product has been modified by another user. Please reload and try again." });
            }
        }
    }
}