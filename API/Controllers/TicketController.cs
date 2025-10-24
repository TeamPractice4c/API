using Microsoft.AspNetCore.Mvc;
using API.InternalClasses;
using API.ExportClasses;
using API.Model;
using API.Enums;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController(PostgresContext context) : ControllerBase
    {
        private readonly PostgresContext Context = context;

        [HttpGet("GetTickets")]
        public ActionResult<List<ExportTicket>> GetTickets()
        {
            List<Ticket> tickets = [.. Context.Tickets];

            if (tickets is null || tickets.Count == 0)
            {
                return NotFound();
            }

            List<ExportTicket> response = [];

            tickets.ForEach(ticket => response.Add(ticket.ToExport()));

            return Ok(response);
        }

        [HttpGet("GetTicket/{id}")]
        public ActionResult<ExportTicket> GetTicket(int id)
        {
            Ticket? ticket = Context.Tickets.FirstOrDefault(x => x.TId == id);

            if (ticket is null)
            {
                return NotFound();
            }

            return Ok(ticket.ToExport());
        }

        [HttpPost("AddTicket")]
        public ActionResult<ExportTicket> AddTicket([FromBody] ExportTicket ticket)
        {
            Flight? flight = Context.Flights.FirstOrDefault(x => x.FId == ticket.TFlight);

            if (flight is null)
            {
                return BadRequest();
            }

            User? user = Context.Users.AsEnumerable().FirstOrDefault(x => x.UId == x.GetUserId(ticket.TUser));

            if (user is null)
            {
                return BadRequest();
            }

            int id = Context.Tickets.Any() ? Context.Tickets.Max(x => x.TId) + 1 : 1;

            Ticket new_ticket = new()
            {
                TId = id,
                TFlight = flight.FId,
                TUser = user.UId,
                TClass = (ClassOfService)Convertation.ConvertStringToEnum<ClassOfService>(ticket.TStatus)!,
                TBoughtDate = DateTime.Now,
                TTotalPrice = ticket.TTotalPrice,
                TStatus = (TicketStatus)Convertation.ConvertStringToEnum<TicketStatus>("Куплен")!
            };

            SendEmail.SendTicket(Context, id);

            Context.Tickets.Add(new_ticket);

            Context.SaveChanges();

            return Ok(new_ticket.ToExport());
        }

        [HttpPost("ChangeTicketStatus")]
        public ActionResult<ExportTicket> ChangeTicketStatus([FromBody] ExportTicket ticket)
        {
            Ticket? gotten_ticket = Context.Tickets.FirstOrDefault(x => x.TId ==  ticket.TId);

            if (gotten_ticket is null) {
                return NotFound(); 
            }

            gotten_ticket.TStatus = (TicketStatus)Convertation.ConvertStringToEnum<TicketStatus>(ticket.TStatus)!;

            Context.Tickets.Update(gotten_ticket);

            return Ok(gotten_ticket.ToExport());
        }
    }
}
