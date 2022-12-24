using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";

        private readonly Random random = new Random();
        private readonly OpenWeatherClient client;

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            var apiKey = weatherOptions?.Value.ApiKey;
            client = new OpenWeatherClient(apiKey);
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            try
            {
                var weather = await client.GetWeatherFromApiAsync();
                return WeatherForecast.CreateFrom(weather);
            }
            catch (Exception)
            {
                return BuildRandomForecast();
            }
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
