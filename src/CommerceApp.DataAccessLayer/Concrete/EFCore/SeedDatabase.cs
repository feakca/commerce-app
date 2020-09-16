using CommerceApp.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommerceApp.DataAccessLayer.Concrete.EFCore
{
    public static class SeedDatabase
    {
        private static List<Category> _categories = new List<Category>()
        {
            new Category() { Name = "Telefon",Url = "telefon" },
            new Category() { Name = "Bilgisayar",Url = "bilgisayar" },
            new Category() { Name = "Elektronik",Url = "elektronik" },
            new Category() { Name = "Beyaz Eşya",Url = "beyaz-esya" }
        };
        private static List<Product> _products = new List<Product>()
        {
            new Product() { Name = "iPhone 5s",Url = "iphone-5s",Price = 3000,Description = "iPhone 5s",ImageUrl = "iphone-5s.jpg",IsApproved = true,IsHome = true},
            new Product() { Name = "iPhone 6s",Url = "iphone-6s",Price = 5000,Description = "iPhone 6s",ImageUrl =  "iphone-6s.jpg",IsApproved = true,IsHome = true},
            new Product() { Name = "iPhone 11",Url = "iphone-11",Price = 8000,Description = "iPhone 11",ImageUrl =  "iphone-11.jpg",IsApproved = true,IsHome = true},
            new Product() { Name = "Samsung S8",Url = "samsung-s8",Price = 3000,Description = "Samsung S8",ImageUrl = "samsung-s8.jpg",IsApproved = true,IsHome = true},
            new Product() { Name = "Samsung S10",Url = "samsung-s10",Price = 5000,Description = "Samsung S10",ImageUrl = "samsung-s10.jpg",IsApproved = true,IsHome = true},
            new Product() { Name = "Samsung S20",Url = "samsung-s20",Price = 8000,Description = "Samsung S20",ImageUrl = "samsung-s20.jpg",IsApproved = true,IsHome = true}
        };
        private static List<ProductCategory> _productCategories = new List<ProductCategory>()
        {
            new ProductCategory() { Product = _products[0],Category = _categories[0]},
            new ProductCategory() { Product = _products[0],Category = _categories[2]},
            new ProductCategory() { Product = _products[1],Category = _categories[0]},
            new ProductCategory() { Product = _products[1],Category = _categories[2]},
            new ProductCategory() { Product = _products[2],Category = _categories[0]},
            new ProductCategory() { Product = _products[2],Category = _categories[2]},
            new ProductCategory() { Product = _products[3],Category = _categories[0]},
            new ProductCategory() { Product = _products[3],Category = _categories[2]},
            new ProductCategory() { Product = _products[4],Category = _categories[0]},
            new ProductCategory() { Product = _products[4],Category = _categories[2]},
            new ProductCategory() { Product = _products[5],Category = _categories[0]},
            new ProductCategory() { Product = _products[5],Category = _categories[2]}
        };

        public static void Seed()
        {
            using (var context = new CommerceContext())
            {
                if (context.Database.GetPendingMigrations().Count() == 0)
                {
                    if (context.Categories.Count() == 0)
                    {
                        context.Categories.AddRange(_categories);
                    }

                    if (context.Products.Count() == 0)
                    {
                        context.Products.AddRange(_products);
                        context.AddRange(_productCategories);
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
