using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            ProductListViewModel productListViewModel = new ProductListViewModel()
            {
                Products = _productService.GetHomePageProduct()
            };
            return View(productListViewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
