using API.ExportClasses;
using API.InternalClasses;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController(PostgresContext context) : ControllerBase
    {
        private readonly PostgresContext _context = context;

        [HttpGet("GetFlights")]
        public async Task<IActionResult> GetFlights()
        {
            List<Flight> flights = await _context.Flights.AsNoTracking().ToListAsync();

            if (flights is null || flights.Count == 0)
            {
                return NotFound();
            }

            List<ExportFlight> response = [];

            flights.ForEach(flight => response.Add(flight.ToExport()));
            return Ok(response);
        }

        [HttpGet("GetCurrentFlights")]
        public async Task<IActionResult> GetCurrentFlights()
        {
            List<Ticket> tickets = await _context.Tickets.AsNoTracking().ToListAsync() ?? [];
            List<Flight> flights = await _context.Flights.AsNoTracking().ToListAsync();
            flights = flights
                .Where(x => x.FDepartureTime >= DateTime.Now.ToUniversalTime()).AsEnumerable()
                .Where(x => x.FSeatsCount > tickets.AsEnumerable().Where(t => t.TFlight == x.FId).ToList().Count)
                .ToList() ?? [];

            if (flights is null || flights.Count == 0)
            {
                return NotFound();
            }

            List<ExportFlight> response = [];

            flights.ForEach(flight => response.Add(flight.ToExport()));
            return Ok(response);
        }

        [HttpGet("GetFlight/{id}")]
        public async Task<IActionResult> GetFlight(int id)
        {
            Flight? flight = await _context.Flights.AsNoTracking().FirstOrDefaultAsync(x => x.FId == id);

            if (flight is null)
            {
                return NotFound("Указанный рейс не найден");
            }

            return Ok(flight.ToExport());
        }

        [HttpPost("AddFlight")]
        public async Task<IActionResult> AddFlight([FromBody] ExportFlight flight)
        {
            Airline? airline = await _context.Airlines.AsNoTracking().FirstOrDefaultAsync(x => x.AlName == flight.FAirline);
            Airport? departureAirport = await _context.Airports.AsNoTracking().FirstOrDefaultAsync(x => x.ApName == flight.FDepartureAirport);
            Airport? arrivalAirport = await _context.Airports.AsNoTracking().FirstOrDefaultAsync(x => x.ApName == flight.FArrivalAirport);

            if (airline is null)
            {
                return BadRequest("Указанная авиакомпания не найдена");
            }

            if (departureAirport is null)
            {
                return BadRequest("Указанный аэропорт страны отправления не найден");
            }

            if (arrivalAirport is null)
            {
                return BadRequest("Указанный аэропорт страны назначения не найден");
            }

            Flight? gottenFlight = await _context.Flights.AsNoTracking().FirstOrDefaultAsync(x => x.FAirline == airline.AlId &&
            x.FArrivalAirport == arrivalAirport.ApId && x.FDepartureAirport == departureAirport.ApId &&
            x.FDepartureTime == flight.FDepartureTime && x.FArrivalTime == flight.FArrivalTime);

            if (gottenFlight is not null)
            {
                return BadRequest("Рейс с такими параметрами уже существует");
            }

            int id = await _context.Flights.AsNoTracking().AnyAsync() ? await _context.Flights.AsNoTracking().MaxAsync(x => x.FId) + 1 : 1;

            Flight newFlight = new()
            {
                FId = id,
                FAirline = airline.AlId,
                FDepartureAirport = departureAirport.ApId,
                FArrivalAirport = arrivalAirport.ApId,
                FDepartureTime = flight.FDepartureTime,
                FArrivalTime = flight.FArrivalTime,
                FSeatsCount = flight.FSeatsCount,
                FPrice = flight.FPrice,
            };

            _context.Flights.Add(newFlight);

            await _context.SaveChangesAsync();

            return Ok(newFlight.ToExport());
        }

        [HttpPost("EditFlight")]
        public async Task<IActionResult> EditFlight([FromBody] ExportFlight flight)
        {
            Flight? gottenFlight = await _context.Flights.AsNoTracking().FirstOrDefaultAsync(x => x.FId == flight.FId);

            if (gottenFlight is null)
            {
                return NotFound("Указанный рейс не найден");
            }

            Airline? airline = await _context.Airlines.AsNoTracking().FirstOrDefaultAsync(x => x.AlName == flight.FAirline);
            Airport? departutrAirport = await _context.Airports.AsNoTracking().FirstOrDefaultAsync(x => x.ApName == flight.FDepartureAirport);
            Airport? arrivalAirport = await _context.Airports.AsNoTracking().FirstOrDefaultAsync(x => x.ApName == flight.FArrivalAirport);

            if (airline is null)
            {
                return BadRequest("Указанная авиакомпания не найдена");
            }

            if (departutrAirport is null)
            {
                return BadRequest("Указанный аэропорт страны отправления не найден");
            }

            if (arrivalAirport is null)
            {
                return BadRequest("Указанный аэропорт страны назначения не найден");
            }

            gottenFlight.FAirline = airline.AlId;
            gottenFlight.FArrivalAirport = arrivalAirport.ApId;
            gottenFlight.FDepartureAirport = departutrAirport.ApId;
            gottenFlight.FDepartureTime = flight.FDepartureTime;
            gottenFlight.FArrivalTime = flight.FArrivalTime;
            gottenFlight.FSeatsCount = flight.FSeatsCount;
            gottenFlight.FPrice = flight.FPrice;

            _context.Flights.Update(gottenFlight);
            await _context.SaveChangesAsync();

            return Ok(gottenFlight.ToExport());
        }

        [HttpPost("SearchFlights")]
        public async Task<IActionResult> SearchFlights([FromBody] SearchFlightParams parameters)
        {
            List<Flight>? gottenFlights = await SearchFlight.FindFLightsAsync(_context, parameters.CityFrom, parameters.CityTo, parameters.StartDate, parameters.EndDate);

            if (gottenFlights is null)
            {
                return BadRequest("ERROR");
            }

            if (gottenFlights.Count == 0)
            {
                return NotFound("Рейсы не найдены");
            }

            List<ExportFlight> response = [];

            gottenFlights.ForEach(flight => response.Add(flight.ToExport()));

            return Ok(response);
        }
    }
}
