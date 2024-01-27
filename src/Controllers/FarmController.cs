using bb_api.Models;
using bb_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace bb_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FarmsController : ControllerBase
{
    private readonly FarmsService _FarmsService;
    private readonly ProductsService _ProductsService;

    public FarmsController(FarmsService FarmsService, ProductsService ProductsService)
    {
        _FarmsService = FarmsService;
        _ProductsService = ProductsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Farm>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string name = "",
            [FromQuery] string userId = ""
        )
    {
        try
        {
            var Farms = await _FarmsService.GetAsync(page, pageSize, name, userId);

            if (Farms == null)
            {
                return StatusCode(500, $"Internal server error: Farm database not found");
            }
            else if (Farms.Count == 0) return NoContent();

            return Ok(Farms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Farm>> Get(string id)
    {
        var Farm = await _FarmsService.GetAsync(id);

        if (Farm is null)
        {
            return NotFound();
        }

        return Farm;
    }

    [HttpGet("{id:length(24)}/products")]
    public async Task<ActionResult<Product[]>> GetProducts(string id)
    {
        var Farm = await _FarmsService.GetAsync(id);
        if (Farm == null || Farm.Id == null) return StatusCode(404, "farm not found");

        var products = await _ProductsService.GetByFarmIdAsync(Farm.Id);
        if (products == null)
        {
            return StatusCode(404, "Internal server error: product database not found");
        }

        return Ok(products);
    }

    [HttpPost]
    // [Authorize]
    public async Task<IActionResult> Post(Farm newFarm)
    {
        // string farmerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // newFarm.FarmerId = farmerId;

        await _FarmsService.CreateAsync(newFarm);

        return CreatedAtAction(nameof(Get), new { id = newFarm.Id }, newFarm);
    }

    [HttpPut("{id:length(24)}")]
    // [Authorize]
    public async Task<IActionResult> Update(string id, Farm updatedFarm)
    {

        var Farm = await _FarmsService.GetAsync(id);
        if (Farm is null)
        {
            return NotFound();
        }

        // string farmerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (farmerId != Farm.FarmerId)
        // {
        //     return Unauthorized();
        // }

        updatedFarm.Id = Farm.Id;

        await _FarmsService.UpdateAsync(id, updatedFarm);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    // [Authorize]
    public async Task<IActionResult> Delete(string id)
    {

        var Farm = await _FarmsService.GetAsync(id);
        if (Farm is null)
        {
            return NotFound();
        }

        // string farmerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (farmerId != Farm.FarmerId)
        // {
        //     return Unauthorized();
        // }

        await _FarmsService.RemoveAsync(id);

        return NoContent();
    }
}