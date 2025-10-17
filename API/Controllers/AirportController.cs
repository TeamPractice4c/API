using API.ExportClasses;
using API.Model;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirportController(PostgresContext context) : ControllerBase
    {
        public PostgresContext Context = context;

        [HttpGet("GetAirports")]
        public ActionResult<List<ExportAirport>> GetAirports()
        {
            List<Airport> airports = [.. Context.Airports];

            if (airports is null || airports.Count == 0)
            {
                return NotFound();
            }

            List<ExportAirport> response = [];

            airports.ForEach(airport => response.Add(airport.ToExport()));

            return Ok(response);
        }

        [HttpGet("GetAirport/{id}")]
        public ActionResult<ExportAirport> GetAiport(int id)
        {
            Airport? airport = Context.Airports.FirstOrDefault(x => x.ApId == id);

            if (airport is null)
            {
                return NotFound("Указанный аэропорт не найден");
            }

            return Ok(airport.ToExport());
        }

        [HttpPost("AddAirport")]
        public ActionResult<ExportAirport> AddAirport([FromBody] ExportAirport airport)
        {
            Airport? gotten_airport = Context.Airports.FirstOrDefault(x => x.ApName == airport.ApName && 
            x.ApCountry == airport.ApCountry && x.ApCity == airport.ApCity && x.ApStreet == airport.ApStreet && 
            x.ApBuilding == airport.ApBuilding);

            if (gotten_airport is not null)
            {
                return BadRequest("Аэропорт с такими параметрами уже существует");
            }

            int id = Context.Airports.Any() ? Context.Airports.Max(x => x.ApId) + 1 : 1;

            Context.Airports.Add(new()
            {
                ApId = id,
                ApName = airport.ApName,
                ApCountry = airport.ApCountry,
                ApCity = airport.ApCity,
                ApBuilding = airport.ApBuilding,
                ApStreet = airport.ApStreet,
            });

            Context.SaveChanges();

            return Ok(airport);
        }

        [HttpPost("EditAirport")]
        public ActionResult<ExportAirport> EditAirport([FromBody] ExportAirport airport)
        {
            Airport? gotten_airport = Context.Airports.FirstOrDefault(x => x.ApId == airport.ApId);

            if (gotten_airport is null)
            {
                return BadRequest("Указанный аэропорт не найден");
            }

            gotten_airport.ApName = airport.ApName;
            gotten_airport.ApCountry = gotten_airport.ApCountry;
            gotten_airport.ApCity = gotten_airport.ApCity;
            gotten_airport.ApStreet = gotten_airport.ApStreet;
            gotten_airport.ApBuilding = gotten_airport.ApBuilding;

            Context.Airports.Update(gotten_airport);

            Context.SaveChanges();

            return Ok(gotten_airport.ToExport());
        }

        [HttpDelete("DeleteAirport/{id}")]
        public ActionResult DeleteAirport(int id)
        {
            Airport? airport = Context.Airports.FirstOrDefault(x => x.ApId == id);

            if (airport is null)
            {
                return NotFound("Указанный аэропорт не найден");
            }

            Context.Airports.Remove(airport);

            Context.SaveChanges();

            return Ok();
        }
    }
}
