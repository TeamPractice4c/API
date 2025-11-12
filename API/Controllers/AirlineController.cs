using API.ExportClasses;
using API.InternalClasses;
using API.Model;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirlineController(PostgresContext context) : ControllerBase
    {
        public PostgresContext Context = context;

        [HttpGet("GetAirlines")]
        public ActionResult<List<ExportAirline>> GetAirlines()
        {
            List<Airline> airlines = [.. Context.Airlines];

            if (airlines is null || airlines.Count == 0)
            {
                return NotFound();
            }

            List<ExportAirline> response = [];

            airlines.ForEach(airline => response.Add(airline.ToExport()));

            return Ok(response);
        }

        [HttpGet("GetAirline/{id}")]
        public ActionResult<ExportAirline> GetAirline(int id)
        {
            Airline? airline = Context.Airlines.FirstOrDefault(x => x.AlId == id);

            if (airline is null)
            {
                return NotFound("Указанная авиакомпания не найдена");
            }

            return Ok(airline.ToExport());
        }

        [HttpPost("AddAirline")]
        public ActionResult<ExportAirline> AddAirline([FromBody] ExportAirline airline)
        {
            Airline? gotten_airline = Context.Airlines.FirstOrDefault(x => x.AlName == airline.AlName);

            if (gotten_airline is not null)
            {
                return BadRequest("Авиакомпания с такими параметрами уже существует");
            }

            int id = Context.Airlines.Any() ? Context.Airlines.Max(x => x.AlId) + 1 : 1;

            Airline new_airline = new()
            {
                AlId = id,
                AlName = airline.AlName,
                AlEmail = airline.AlEmail,
            };

            Context.Airlines.Add(new_airline);

            Context.SaveChanges();

            return Ok(new_airline.ToExport());
        }

        [HttpPost("EditAirline")]
        public ActionResult<ExportAirline> EditAirline([FromBody] ExportAirline airline)
        {
            Airline? gotten_airline = Context.Airlines.FirstOrDefault(x => x.AlId == airline.AlId);

            if (gotten_airline is null)
            {
                return BadRequest("Указанная авиакомпания не найдена");
            }

            gotten_airline.AlName = airline.AlName;
            gotten_airline.AlEmail = airline.AlEmail;

            Context.Airlines.Update(gotten_airline);

            Context.SaveChanges();

            return Ok(gotten_airline.ToExport());
        }

        [HttpDelete("DeleteAirline/{id}")]
        public ActionResult DeleteAirline(int id)
        {
            Airline? airline = Context.Airlines.FirstOrDefault(x => x.AlId == id);

            if (airline is null)
            {
                return NotFound("Указанная авиакомпания не найдена");
            }

            Context.Airlines.Remove(airline);

            Context.SaveChanges();

            return Ok();
        }

        [HttpPost("UploadAirlineImage")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Upload([FromForm] UploadFile file, [FromForm] int airlineId)
        {
            if (file.File is null || file.File.Length == 0)
            {
                return BadRequest("Файл изображения не передан или пустой.");
            }

            if (Context.Airlines.Any(x => x.AlId == airlineId))
            {
                return NotFound("Указанная авиакомпания не найдена");
            }

            string airlinePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/airline/");

            if (!Directory.Exists(airlinePath))
            {
                Directory.CreateDirectory(airlinePath);
            }

            var path = Path.Combine(airlinePath, $"{airlineId}{Path.GetExtension(file.File.FileName)}");
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.File.CopyToAsync(stream);
            }

            return Ok();
        }
    }
}
