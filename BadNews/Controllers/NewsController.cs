using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BadNews.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder newsModelBuilder;

        public NewsController(INewsModelBuilder newsModelBuilder)
        {
            this.newsModelBuilder = newsModelBuilder;
        }

        public IActionResult FullArticle(Guid id)
        {
            var model = newsModelBuilder.BuildFullArticleModel(id);
            if (model == null)
                return NotFound();
            return View(model);
        }

        public IActionResult Index(int? year, int pageIndex = 0)
        {
            return View(newsModelBuilder.BuildIndexModel(pageIndex, true, year));
        }
    }
}