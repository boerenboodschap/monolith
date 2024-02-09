using bb_api.Models;
using bb_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace bb_api.Controllers;

using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class FarmsController : ControllerBase
{
    private readonly FarmsService _FarmsService;
    private readonly ProductsService _ProductsService;

    private readonly ILogger logger;

    public FarmsController(FarmsService FarmsService, ProductsService ProductsService)
    {
        _FarmsService = FarmsService;
        _ProductsService = ProductsService;
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Program");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Farm>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string name = "",
            [FromQuery] string userId = "",
            [FromQuery] double posX = 0,
            [FromQuery] double posY = 0,
            [FromQuery] int radius = 0
        )
    {
        try
        {
            var Farms = await _FarmsService.GetAsync(page, pageSize, name, userId, posX, posY, radius);

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

    [HttpGet("my-farm/{id:length(24)}")]
    // [Authorize]
    public async Task<ActionResult<Farm>> GetMyFarm(string id)
    {
        // string farmerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // if (farmerId is null)
        // {
        //     return BadRequest();
        // }

        // TODO: check if the id is not encoded

        // check if this user already has a Farm
        var Farm = await _FarmsService.GetByFarmerIdAsync(id);

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
    public async Task<IActionResult> Post(Farm newFarm)
    {
        if(newFarm.FarmerId == "") return BadRequest();

        string farmerId = newFarm.FarmerId;

        // check if this user already has a Farm
        var Farm = await _FarmsService.GetByFarmerIdAsync(farmerId);
        if (Farm != null) return BadRequest();

        newFarm.FarmerId = farmerId;

        await _FarmsService.CreateAsync(newFarm);

        return CreatedAtAction(nameof(Get), new { id = newFarm.Id }, newFarm);
    }

    [HttpPut("{id:length(24)}")]
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