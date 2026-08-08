using ERP_Finance.DTOs.Product;
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

        if (products == null)
            return NotFound();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult GetProduct(int id)
    {
        var product = _productService.GetProductService(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public ActionResult AddProduct([FromBody] CreateProductDTO productDTO)
    {
        var created = _productService.AddProductService(productDTO);

        if(!created)
            return BadRequest();

        return Created();
    }

    [HttpPatch("{id}")]
    public ActionResult UpdateProduct(int id, [FromBody] UpdateProductDTO productDTO)
    {
        var updated = _productService.UpdateProductService(id, productDTO);

        if(!updated)
            return BadRequest();

        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id) 
    { 
        var deleted = _productService.DeleteProductService(id);

        if(!deleted)
            return BadRequest();

        return Ok();
    }
}
