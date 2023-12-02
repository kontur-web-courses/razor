using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";

        private readonly Random random = new Random();

        private readonly OpenWeatherClient client;

        private ILogger<WeatherForecastRepository> log;

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions, ILogger<WeatherForecastRepository> log)
        {
            this.log = log;
            client = new OpenWeatherClient(weatherOptions?.Value.ApiKey);
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            try
            {
                var weather = await client.GetWeatherFromApiAsync();
                return WeatherForecast.CreateFrom(weather);
            }
            catch
            {
                log.LogWarning("Ошибка получения погоды");
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