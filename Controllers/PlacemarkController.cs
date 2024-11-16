using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/placemarks")]
public class PlacemarkController : ControllerBase
{
    private readonly IKmlService _kmlService;

    public PlacemarkController(IKmlService kmlService)
    {
        _kmlService = kmlService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlacemarks([FromQuery] string cliente, [FromQuery] string situacao, [FromQuery] string bairro, [FromQuery] string referencia, [FromQuery] string ruaCruzamento)
    {
        try
        {
            var placemarks = await _kmlService.GetPlacemarksAsync(cliente, situacao, bairro, referencia, ruaCruzamento);
            return Ok(placemarks);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("export")]
    public async Task<IActionResult> ExportKml([FromBody] Placemark request)
    {
        try
        {
            var kmlFile = await _kmlService.ExportKmlAsync(request.Cliente, request.Situacao, request.Bairro, request.Referencia, request.RuaCruzamento);

            var kmlStream = new MemoryStream();
            kmlFile.Save(kmlStream);
            kmlStream.Position = 0;

            return File(kmlStream, "application/vnd.google-earth.kml+xml", "exported.kml");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetFilterOptions()
    {
        var filterOptions = await _kmlService.GetFilterOptionsAsync();
        return Ok(filterOptions);
    }
}
