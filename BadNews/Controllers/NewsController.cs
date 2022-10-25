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

        public IActionResult Index(int pageIndex = 0)
        {
            var pageModel = newsModelBuilder.BuildIndexModel(pageIndex, false, null);
            return View(pageModel);
        }
        
        public IActionResult FullArticle(Guid? id)
        {
            var pageModel = id.HasValue ? newsModelBuilder.BuildFullArticleModel(id.Value) : null;
            return View(pageModel);
        }
    }
}