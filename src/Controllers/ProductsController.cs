using bb_api.Models;
using bb_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace bb_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductsService _ProductsService;
    private readonly FarmsService _FarmsService;

    public ProductsController(ProductsService ProductsService, FarmsService FarmsService){
        _ProductsService = ProductsService;
        _FarmsService = FarmsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string name = "",
            [FromQuery] string userId = ""
        )
    {
        try
        {
            var products = await _ProductsService.GetAsync(page, pageSize, name, userId);

            if (products == null)
            {
                return StatusCode(500, $"Internal server error: product database not found");
            }
            else if (products.Count == 0) return NoContent();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var Product = await _ProductsService.GetAsync(id);

        if (Product is null)
        {
            return NotFound();
        }

        return Product;
    }

    [HttpGet("{id:length(24)}/stock/{amount}")]
    public async Task<IActionResult> Get(string id, int amount)
    {
        var Product = await _ProductsService.GetAsync(id);

        if (Product is null)
        {
            return NotFound();
        }

        // remove the specified stock amount
        if (amount > Product.Stock)
        {
            return BadRequest();
        }
        Product.Stock -= amount;

        await _ProductsService.UpdateAsync(id, Product);

        return NoContent();
    }

    [HttpPost]
    // [Authorize]
    public async Task<IActionResult> Post(Product newProduct)
    {
        string farmerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (farmerId == null) return BadRequest();

        Farm farm = await _FarmsService.GetByFarmerIdAsync(farmerId);
        if (farm.Id == null) return BadRequest();

        newProduct.FarmId = farm.Id;

        await _ProductsService.CreateAsync(newProduct);

        return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
    }

    [HttpPut("{id:length(24)}")]
    // [Authorize]
    public async Task<IActionResult> Update(string id, Product updatedProduct)
    {

        var Product = await _ProductsService.GetAsync(id);
        if (Product is null)
        {
            return NotFound();
        }

        string farmerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (farmerId == null) return BadRequest();

        Farm farm = await _FarmsService.GetByFarmerIdAsync(farmerId);
        if (farmerId != farm.FarmerId)
        {
            return Unauthorized();
        }

        updatedProduct.Id = Product.Id;

        await _ProductsService.UpdateAsync(id, updatedProduct);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    // [Authorize]
    public async Task<IActionResult> Delete(string id)
    {

        var Product = await _ProductsService.GetAsync(id);
        if (Product is null)
        {
            return NotFound();
        }

        string farmerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (farmerId == null) return BadRequest();

        Farm farm = await _FarmsService.GetByFarmerIdAsync(farmerId);
        if (farmerId != farm.FarmerId)
        {
            return Unauthorized();
        }

        await _ProductsService.RemoveAsync(id);

        return NoContent();
    }
}