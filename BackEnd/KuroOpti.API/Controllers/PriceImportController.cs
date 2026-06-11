using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/prices")]
public class PriceImportController : ControllerBase
{
    private readonly IFuelPriceImporter _importer;

    public PriceImportController(IFuelPriceImporter importer)
    {
        _importer = importer;
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdatePrices()
    {
        await _importer.ImportAsync();
        return Ok(new { success = true, message = "Kainos atnaujintos" });
    }
}
