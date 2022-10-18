using System;
using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder newsModelBuilder;

        public NewsController(INewsModelBuilder newsModelBuilder)
        {
            this.newsModelBuilder = newsModelBuilder;
        }

        public IActionResult Index(int? year, int pageIndex = 0)
        {
            var indexModel = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(indexModel);
        }

        public IActionResult FullArticle(Guid id)
        {
            var fullArticleModel = newsModelBuilder.BuildFullArticleModel(id);
            if (fullArticleModel== null)
                return NotFound();
            return View(fullArticleModel);
        }
    }
}