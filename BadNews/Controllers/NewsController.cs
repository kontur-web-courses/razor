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

        public IActionResult Index(int pageIndex = 0)
        {
            var model = newsModelBuilder.BuildIndexModel(pageIndex, true, null);
            return View(model);
        }

        public IActionResult FullArticle(Guid id)
        {
            var model = newsModelBuilder.BuildFullArticleModel(id);
            return model == null ? NotFound() : View(model);
        }
    }
}