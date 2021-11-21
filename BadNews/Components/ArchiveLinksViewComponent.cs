using BadNews.Repositories.News;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Components
{
    public class ArchiveLinksViewComponent : ViewComponent
    {
        private readonly INewsRepository _newsRepository;

        public ArchiveLinksViewComponent(INewsRepository newsRepository)
        {
            this._newsRepository = newsRepository;
        }

        public IViewComponentResult Invoke()
        {
            var years = _newsRepository.GetYearsWithArticles();
            return View(years);
        }
    }
}