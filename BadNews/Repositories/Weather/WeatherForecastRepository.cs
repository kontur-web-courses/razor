using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly string apiKey;
        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            this.apiKey = weatherOptions?.Value.ApiKey;
        }

        private const string defaultWeatherImageUrl = "/images/cloudy.png";

        private readonly Random random = new Random();

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            var client = new OpenWeatherClient(apiKey);
            var weather = WeatherForecast.CreateFrom(client.GetWeatherFromApiAsync().Result);
            return weather;
        }

        private WeatherForecast BuildRandomForecast()
        {
            var temperature = random.Next(-20, 20 + 1);
            return new WeatherForecast
            {
                TemperatureInCelsius = temperature,
                IconUrl = defaultWeatherImageUrl
            };
        }
    }
}
