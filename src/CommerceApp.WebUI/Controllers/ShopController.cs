using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.Entity;
using CommerceApp.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ShopController : Controller
    {
        const int pageSize = 3;
        private IProductService _productService;

        public ShopController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult List(string categoryUrl, string q, int page = 1)
        {
            ProductListViewModel productListViewModel = new ProductListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    Query = q,
                    TotalItems = _productService.GetCountByCategoryWithSearch(categoryUrl, q),
                    ItemsPerPage = pageSize,
                    CurrentPage = page,
                    CurrentCategory = categoryUrl
                },
                Products = _productService.GetProductsByCategoryWithSearch(categoryUrl, page, pageSize, q)
            };
            return View(productListViewModel);
        }

        public IActionResult Search(string categoryUrl, string q, int page = 1)
        {
            ProductListViewModel productListViewModel = new ProductListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategoryWithSearch(categoryUrl, q),
                    ItemsPerPage = pageSize,
                    CurrentPage = page,
                    CurrentCategory = categoryUrl
                },
                Products = _productService.GetProductsByCategoryWithSearch(categoryUrl, page, pageSize, q)
            };
            return View(productListViewModel);
        }

        public IActionResult Details(string productUrl)
        {
            if (productUrl == null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails(productUrl);
            if (product == null)
            {
                return NotFound();
            }
            return View(new ProductDetailModel()
            {
                Product = product,
                Categories = product.ProductCategories.Select(i => i.Category).ToList()
            });
        }
    }
}
