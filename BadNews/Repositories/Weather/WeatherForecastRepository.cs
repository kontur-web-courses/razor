using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly OpenWeatherClient openWeatherClient;

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            var weatherApiKey = weatherOptions?.Value.ApiKey;
            openWeatherClient = new OpenWeatherClient(weatherApiKey);
        }
        
        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            return await BuildRandomForecast();
        }

        private async Task<WeatherForecast> BuildRandomForecast()
        {
            var weather = await openWeatherClient.GetWeatherFromApiAsync();
            return WeatherForecast.CreateFrom(weather);
        }
    }
}
