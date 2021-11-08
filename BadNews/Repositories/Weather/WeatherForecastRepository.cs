using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";

        private readonly Random random = new Random();

        private readonly OpenWeatherClient openWeatherClient;
        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            var apiKey = weatherOptions?.Value.ApiKey;
            openWeatherClient = new OpenWeatherClient(apiKey);
        }
        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            WeatherForecast weatherForecast = null;
            try
            {
                var openWeatherForecast = await openWeatherClient.GetWeatherFromApiAsync();
                weatherForecast = WeatherForecast.CreateFrom(openWeatherForecast);
            }
            catch (TaskCanceledException e) {
                weatherForecast = BuildRandomForecast();
            }
            
            return weatherForecast;
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
