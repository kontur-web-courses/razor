using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";
        private readonly string apiKey;
        private readonly OpenWeatherClient openWeatherClient;
        private readonly Random random = new Random();

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            apiKey = weatherOptions?.Value.ApiKey;
            openWeatherClient = apiKey is not null ? new OpenWeatherClient(apiKey) : null;
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            var serviceResponse = await TryGetWeatherFromOpenWeather();
            if (serviceResponse is null) return BuildRandomForecast();
            return WeatherForecast.CreateFrom(serviceResponse);
        }

        private async Task<OpenWeatherForecast> TryGetWeatherFromOpenWeather()
        {
            try
            {
                return await openWeatherClient?.GetWeatherFromApiAsync();
            }
            catch (Exception e)
            {
                return null;
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
