using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace WebApp.Models
{
    public class TemperatureModel : PageModel
    {
        public static List<TemperatureData> InitialTemperatures { get; set; } = new List<TemperatureData>();

        public async Task OnGetAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("https://your_server_api_url/api/temperature");
                InitialTemperatures = JsonConvert.DeserializeObject<List<TemperatureData>>(response);
            }
        }
    }

    public class TemperatureData
    {
        public string Device { get; set; }
        public double Temperature { get; set; }
    }
}