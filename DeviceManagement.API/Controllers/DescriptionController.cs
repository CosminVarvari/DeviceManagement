using DeviceManagement.Core.DTOs.Device;
using DeviceManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DescriptionController : ControllerBase
{
    private readonly IDescriptionGeneratorService _generatorService;
    private readonly ILogger<DescriptionController> _logger;

    public DescriptionController(
        IDescriptionGeneratorService generatorService,
        ILogger<DescriptionController> logger)
    {
        _generatorService = generatorService;
        _logger = logger;
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromBody] GenerateDescriptionDto dto)
    {
        _logger.LogInformation(
            "Generating description for {Name} by {Manufacturer}", dto.Name, dto.Manufacturer);

        var description = await _generatorService.GenerateDescriptionAsync(dto);

        return Ok(new { description });
    }
}