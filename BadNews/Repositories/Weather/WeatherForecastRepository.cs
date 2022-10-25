using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";

        private readonly Random random = new();

        public readonly string ApiKey;

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            ApiKey = weatherOptions?.Value.ApiKey;
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            var weatherForecast = new OpenWeatherClient(null);
            var openWeatherForecast = await weatherForecast.GetWeatherFromApiAsync();
            return openWeatherForecast == null 
                ? BuildRandomForecast() 
                : WeatherForecast.CreateFrom(openWeatherForecast);
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
