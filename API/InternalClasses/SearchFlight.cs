using API.Model;

namespace API.InternalClasses
{
    internal static class SearchFlight
    {
        public static async Task FindFLightsAsync(string from, string to, DateOnly start, DateOnly end)
        {

        }

        public static async Task FindFLightsAsync(string from, string to, DateOnly start, DateOnly end, int max = -1)
        {
            if (max == -1)
            {
                await FindFLightsAsync(from, to, start, end);
            }


        }

        public static async Task FindFLightsAsync(string from, string to, DateOnly start, DateOnly end, string? airline = null)
        {
            if (airline is null)
            {
                await FindFLightsAsync(from, to, start, end);
            }


        }

        public static async Task FindFLightsAsync(string from, string to, DateOnly start, DateOnly end, string? @class = null)
        {
            if (@class is null)
            {
                await FindFLightsAsync(from, to, start, end);
            }


        }

        public static async Task FindFLightsAsync(string from, string to, DateOnly start, DateOnly end, string? @class = null)
        {
            if (@class is null)
            {
                await FindFLightsAsync(from, to, start, end);
            }


        }
    }
}
