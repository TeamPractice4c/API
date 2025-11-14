namespace API.InternalClasses
{
    public class SearchFlightParams
    {
        public string CountryFrom { get; set; } = null!;

        public string CountryTo { get; set; } = null!;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int MaxCost { get; set; } = -1;

        public string? Airline { get; set; }

        public string? ClassOfService { get; set; }
    }
}
