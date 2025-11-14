using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.InternalClasses
{
    internal static class SearchFlight
    {
        public static async Task<List<Flight>?> FindFLightsAsync(PostgresContext context, string from, string to, DateOnly start, DateOnly end)
        {
            Airport? airportFrom = await context.Airports.FirstOrDefaultAsync(x => x.ApCity.ToLower() == from.ToLower());

            if (airportFrom is null)
            {
                return null;
            }

            Airport? airportTo = await context.Airports.FirstOrDefaultAsync(x => x.ApCity.ToLower() == to.ToLower());
            
            if (airportTo is null)
            {
                return null;
            }

            List<Flight> airlines = await context.Flights
                .Where(x => x.FDepartureAirport == airportFrom.ApId)
                .Where(x => x.FArrivalAirport == airportTo.ApId)
                .Where(x => new DateOnly(x.FDepartureTime.Year, x.FDepartureTime.Month, x.FDepartureTime.Day) == start)
                .Where(x => new DateOnly(x.FArrivalTime.Year, x.FArrivalTime.Month, x.FArrivalTime.Day) == end)
                .ToListAsync();

            return airlines;
        }

        public static async Task FindFLightsAsync(PostgresContext context, string from, string to, DateOnly start, DateOnly end, int max = -1)
        {
            if (max == -1)
            {
                await FindFLightsAsync(context, from, to, start, end);
            }


        }

        public static async Task FindFLightsAsync(PostgresContext context, string from, string to, DateOnly start, DateOnly end, string? airline = null)
        {
            if (airline is null)
            {
                await FindFLightsAsync(context, from, to, start, end);
            }


        }
    }
}
