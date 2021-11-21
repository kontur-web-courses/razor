using System;
using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder _newsModelBuilder;

        public NewsController(INewsModelBuilder newsModelBuilder)
        {
            this._newsModelBuilder = newsModelBuilder;
        }

        public IActionResult Index(int? year, int pageIndex = 0)
        {
            var model = _newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            
            return View(model);
        }

        public IActionResult FullArticle(Guid id)
        {
            var model = _newsModelBuilder.BuildFullArticleModel(id);
            if (model == null)
            {
                return NotFound();
            }
            
            return View(model);
        }
    }
}