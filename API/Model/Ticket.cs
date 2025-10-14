using API.Enums;
using System;
using System.Collections.Generic;

namespace API.Model;

public partial class Ticket
{
    public int TId { get; set; }

    public int TFlight { get; set; }

    public int TUser { get; set; }

    public DateTime TBoughtDate { get; set; }

    public ClassOfService TClass { get; set; }

    public int TTotalPrice { get; set; }

    public TicketStatus TStatus { get; set; }

    public virtual Flight TFlightNavigation { get; set; } = null!;

    public virtual User TUserNavigation { get; set; } = null!;
}
