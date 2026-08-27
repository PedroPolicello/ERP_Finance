using ERP_Finance.DTOs.Product;
using ERP_Finance.Entities;
using ERP_Finance.Service;
using Microsoft.AspNetCore.Mvc;

namespace ERP_Finance.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public ActionResult GetAllProducts()
    {
        var products = _productService.GetAllProductsService();

        return Ok(products);
    }

    [HttpGet("search")]
    public ActionResult<IReadOnlyList<Product>> GetProductsByName(
    [FromQuery] string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Ok(Array.Empty<Product>());

        var products = _productService.GetProductsByNameService(name);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<Product> GetProduct(Guid id)
    {
        var product = _productService.GetProductService(id);

        return Ok(product);
    }

    [HttpPost]
    public ActionResult CreateProduct([FromBody] CreateProductDTO productDTO)
    {
        var result = _productService.CreateProductService(productDTO);

        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}")]
    public ActionResult UpdateProduct(Guid id, [FromBody] UpdateProductDTO productDTO)
    {
        var updated = _productService.UpdateProductService(id, productDTO);

        if (!updated)
            return BadRequest();

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteProduct(Guid id)
    {
        _productService.DeleteProductService(id);

        return NoContent();
    }
}
