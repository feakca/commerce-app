using CommerceApp.BusinessLogicLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private ICategoryService _categoryService;

        public CategoriesViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public IViewComponentResult Invoke()
        {
            if (RouteData.Values["categoryUrl"] != null)
            {
                ViewBag.selectedCategory = RouteData?.Values["categoryUrl"];
            }
            return View(_categoryService.GetAll());
        }
    }
}
