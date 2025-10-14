using API.ExportClasses;
using System;
using System.Collections.Generic;

namespace API.Model;

public partial class Flight
{
    public int FId { get; set; }

    public int FAirline { get; set; }

    public int FDepartureAirport { get; set; }

    public int FArrivalAirport { get; set; }

    public DateTime FDepartureTime { get; set; }

    public DateTime FArrivalTime { get; set; }

    public int FSeatsCount { get; set; }

    public virtual Airline FAirlineNavigation { get; set; } = null!;

    public virtual Airport FArrivalAirportNavigation { get; set; } = null!;

    public virtual Airport FDepartureAirportNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public ExportFlight ToExport()
    {
        return new()
        {
            FId = FId,
            FAirline = FAirlineNavigation.AlName,
            FDepartureAirport = FDepartureAirportNavigation.ApName,
            FArrivalAirport = FArrivalAirportNavigation.ApName,
            FDepartureTime = FDepartureTime,
            FArrivalTime = FArrivalTime,
            FSeatsCount = FSeatsCount,
        };
    }
}
