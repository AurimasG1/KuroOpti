using KuroOpti.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly KuroOptiDbContext _db;

    public AnalyticsController(KuroOptiDbContext db)
    {
        _db = db;
    }

    [HttpGet("region-prices")]
    public async Task<IActionResult> GetRegionPrices()
    {
        var data = await _db
            .FuelStations.Where(s => s.Municipality != null && s.Municipality != "")
            .GroupBy(s => s.Municipality)
            .Select(g => new
            {
                Region = g.Key,
                AvgPetrol = Math.Round(g.Average(x => x.PetrolPrice), 3),
                AvgDiesel = Math.Round(g.Average(x => x.DieselPrice), 3),
                AvgLpg = Math.Round(g.Average(x => x.LpgPrice), 3),
                Count = g.Count(),
            })
            .OrderBy(x => x.Region)
            .ToListAsync();

        return Ok(data);
    }
}
