using System.Threading.Tasks;
using BadNews.Repositories.Weather;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Components
{
    public class WeatherViewComponent : ViewComponent
    {
        private IWeatherForecastRepository _weatherForecastRepository;

        public WeatherViewComponent(IWeatherForecastRepository weatherForecastRepository)
        {
            _weatherForecastRepository = weatherForecastRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _weatherForecastRepository.GetWeatherForecastAsync());
        }
    }
}