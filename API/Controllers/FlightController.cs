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
            int airline = Context.Airlines.First(x => x.AlName == flight.FAirline).AlId;
            int departure_airport = Context.Airports.First(x => x.ApName == flight.FDepartureAirport).ApId;
            int arrival_airport = Context.Airports.First(x => x.ApName == flight.FArrivalAirport).ApId;

            Flight? gotten_flight = Context.Flights.FirstOrDefault(x => x.FAirline == airline &&
            x.FArrivalAirport == arrival_airport && x.FDepartureAirport == departure_airport && 
            x.FDepartureTime == flight.FDepartureTime && x.FArrivalTime == flight.FArrivalTime);

            if (gotten_flight is not null)
            {
                return BadRequest("Рейс с такими параметрами уже существует");
            }

           int id = Context.Flights.Any() ? Context.Flights.Max(x => x.FId) + 1 : 1;
            

            Context.Flights.Add(new()
            {
                FId = id,
                FAirline = airline,
                FDepartureAirport = departure_airport,
                FArrivalAirport = arrival_airport,
                FDepartureTime = flight.FDepartureTime,
                FArrivalTime = flight.FArrivalTime,
                FSeatsCount = flight.FSeatsCount,
            });

            Context.SaveChanges();

            return Ok(flight);
        }

        [HttpPost("EditFlight")]
        public ActionResult<ExportFlight> EditFlight([FromBody] ExportFlight flight)
        {
            Flight? gotten_flight = Context.Flights.FirstOrDefault(x => x.FId == flight.FId);

            if (gotten_flight is null)
            {
                return NotFound("Указанный рейс не найден");
            }
            
            int airline = Context.Airlines.First(x => x.AlName == flight.FAirline).AlId;
            int departure_airport = Context.Airports.First(x => x.ApName == flight.FDepartureAirport).ApId;
            int arrival_airport = Context.Airports.First(x => x.ApName == flight.FArrivalAirport).ApId;

            gotten_flight.FAirline = airline;
            gotten_flight.FArrivalAirport = arrival_airport;
            gotten_flight.FDepartureAirport = departure_airport;
            gotten_flight.FDepartureTime = flight.FDepartureTime;
            gotten_flight.FArrivalTime = flight.FArrivalTime;
            gotten_flight.FSeatsCount = flight.FSeatsCount;

            Context.Flights.Update(gotten_flight);
            Context.SaveChanges();

            return Ok(gotten_flight.ToExport());
        }


    }
}
