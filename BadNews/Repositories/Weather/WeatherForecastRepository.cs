using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";

        private readonly Random random = new Random();
        private string weather;

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            weather = weatherOptions?.Value.ApiKey;
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            var openWeatherClient = new OpenWeatherClient(weather);
            var data = WeatherForecast.CreateFrom(await openWeatherClient.GetWeatherFromApiAsync());
            if (data != null)
                return data;
            return BuildRandomForecast();
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
