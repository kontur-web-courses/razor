using System;
using System.Threading.Tasks;

namespace BadNews.Repositories.Weather
{
    using Microsoft.Extensions.Options;

    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly OpenWeatherClient openWeatherClient;

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            openWeatherClient = new OpenWeatherClient(weatherOptions.Value.ApiKey);
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            var openWeatherForecast = await openWeatherClient.GetWeatherFromApiAsync();
            return WeatherForecast.CreateFrom(openWeatherForecast);
        }
    }
}