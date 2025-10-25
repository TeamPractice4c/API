using API.ExportClasses;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController(PostgresContext context) : ControllerBase
    {
        private readonly PostgresContext Context = context;

        [HttpGet("GetFlights")]
        public ActionResult<List<ExportFlight>> GetFlights()
        {
            List<Flight> flights = [.. Context.Flights];

            if (flights is null || flights.Count == 0)
            {
                return NotFound();
            }

            List<ExportFlight> response = [];

            flights.ForEach(flight => response.Add(flight.ToExport()));
            return Ok(response);
        }

        [HttpGet("GetFlight/{id}")]
        public ActionResult<ExportFlight> GetFlight(int id)
        {
            Flight? flight = Context.Flights.FirstOrDefault(x => x.FId == id);

            if (flight is null)
            {
                return NotFound("Указанный рейс не найден");
            }

            return Ok(flight.ToExport());
        }

        [HttpPost("AddFlight")]
        public ActionResult<ExportFlight> AddFlight([FromBody] ExportFlight flight)
        {
            Airline? airline = Context.Airlines.FirstOrDefault(x => x.AlName == flight.FAirline);
            Airport? departure_airport = Context.Airports.FirstOrDefault(x => x.ApName == flight.FDepartureAirport);
            Airport? arrival_airport = Context.Airports.FirstOrDefault(x => x.ApName == flight.FArrivalAirport);

            if (airline is null)
            {
                return BadRequest("Указанная авиакомпания не найдена");
            }

            if (departure_airport is null)
            {
                return BadRequest("Указанный аэропорт страны отправления не найден");
            }

            if (arrival_airport is null)
            {
                return BadRequest("Указанный аэропорт страны назначения не найден");
            }

            Flight? gotten_flight = Context.Flights.FirstOrDefault(x => x.FAirline == airline.AlId &&
            x.FArrivalAirport == arrival_airport.ApId && x.FDepartureAirport == departure_airport.ApId && 
            x.FDepartureTime == flight.FDepartureTime && x.FArrivalTime == flight.FArrivalTime);

            if (gotten_flight is not null)
            {
                return BadRequest("Рейс с такими параметрами уже существует");
            }

           int id = Context.Flights.Any() ? Context.Flights.Max(x => x.FId) + 1 : 1;

            Flight new_flight = new()
            {
                FId = id,
                FAirline = airline.AlId,
                FDepartureAirport = departure_airport.ApId,
                FArrivalAirport = arrival_airport.ApId,
                FDepartureTime = flight.FDepartureTime,
                FArrivalTime = flight.FArrivalTime,
                FSeatsCount = flight.FSeatsCount,
                FPrice = flight.FPrice,
            };

            Context.Flights.Add(new_flight);

            Context.SaveChanges();

            return Ok(new_flight.ToExport());
        }

        [HttpPost("EditFlight")]
        public ActionResult<ExportFlight> EditFlight([FromBody] ExportFlight flight)
        {
            Flight? gotten_flight = Context.Flights.FirstOrDefault(x => x.FId == flight.FId);

            if (gotten_flight is null)
            {
                return NotFound("Указанный рейс не найден");
            }

            Airline? airline = Context.Airlines.FirstOrDefault(x => x.AlName == flight.FAirline);
            Airport? departure_airport = Context.Airports.FirstOrDefault(x => x.ApName == flight.FDepartureAirport);
            Airport? arrival_airport = Context.Airports.FirstOrDefault(x => x.ApName == flight.FArrivalAirport);

            if (airline is null)
            {
                return BadRequest("Указанная авиакомпания не найдена");
            }

            if (departure_airport is null)
            {
                return BadRequest("Указанный аэропорт страны отправления не найден");
            }

            if (arrival_airport is null)
            {
                return BadRequest("Указанный аэропорт страны назначения не найден");
            }

            gotten_flight.FAirline = airline.AlId;
            gotten_flight.FArrivalAirport = arrival_airport.ApId;
            gotten_flight.FDepartureAirport = departure_airport.ApId;
            gotten_flight.FDepartureTime = flight.FDepartureTime;
            gotten_flight.FArrivalTime = flight.FArrivalTime;
            gotten_flight.FSeatsCount = flight.FSeatsCount;
            gotten_flight.FPrice = flight.FPrice;

            Context.Flights.Update(gotten_flight);
            Context.SaveChanges();

            return Ok(gotten_flight.ToExport());
        }


    }
}
