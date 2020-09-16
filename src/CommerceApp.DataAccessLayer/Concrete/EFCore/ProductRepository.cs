using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommerceApp.DataAccessLayer.Concrete.EFCore
{
    public class ProductRepository : GenericRepository<Product, CommerceContext>, IProductRepository
    {
        public void Create(Product entity, int[] categoryIds)
        {
            using (var context = new CommerceContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Products.Add(entity);
                        context.SaveChanges();

                        var product = context.Products.Include(i => i.ProductCategories).FirstOrDefault(i => i.ProductId == entity.ProductId);
                        if (product != null)
                        {
                            product.ProductCategories = categoryIds.Select(i => new ProductCategory()
                            {
                                ProductId = product.ProductId,
                                CategoryId = i
                            }).ToList();
                            context.SaveChanges();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public Product GetByIdWithCategories(int id)
        {
            using (var context = new CommerceContext())
            {
                return context.Products.Where(p => p.ProductId == id).Include(i => i.ProductCategories).ThenInclude(i => i.Category).FirstOrDefault();
            }
        }

        public int GetCountByCategory(string categoryUrl)
        {
            using (var context = new CommerceContext())
            {
                var products = context.Products.Where(i => i.IsApproved).AsQueryable();
                if (!string.IsNullOrEmpty(categoryUrl))
                {
                    products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category).Where(i => i.ProductCategories.Any(i => i.Category.Url == categoryUrl));
                }
                return products.Count();
            }
        }

        public int GetCountByCategoryWithSearch(string categoryUrl, string queryString)
        {
            using (var context = new CommerceContext())
            {
                var products = context.Products.Where(i => i.IsApproved).AsQueryable();
                if (!string.IsNullOrEmpty(categoryUrl))
                {
                    products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category).Where(i => i.ProductCategories.Any(i => i.Category.Url == categoryUrl));
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category).Where(i => i.ProductCategories.Any(i => i.Category.Url == categoryUrl)).Where(i => i.Name.ToLower().Contains(queryString));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category).Where(i => i.Name.ToLower().Contains(queryString));
                    }
                }
                return products.Count();
            }
        }

        public List<Product> GetHomePageProducts()
        {
            using (var context = new CommerceContext())
            {
                return context.Products.Where(i => i.IsHome && i.IsApproved).ToList();
            }
        }

        public Product GetProductDetails(string productUrl)
        {
            using (var context = new CommerceContext())
            {
                return context.Products.Where(i => i.Url == productUrl).Include(i => i.ProductCategories).ThenInclude(i => i.Category).FirstOrDefault();
            }
        }

        public List<Product> GetProductsByCategory(string categoryUrl, int page, int pageSize)
        {
            using (var context = new CommerceContext())
            {
                var products = context.Products.Where(i => i.IsApproved).AsQueryable();
                if (!string.IsNullOrEmpty(categoryUrl))
                {
                    products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category).Where(i => i.ProductCategories.Any(i => i.Category.Url == categoryUrl));
                }
                return products.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            }
        }

        public List<Product> GetProductsByCategoryWithSearch(string categoryUrl, int page, int pageSize, string queryString)
        {
            using (var context = new CommerceContext())
            {
                var products = context.Products.Where(i => i.IsApproved).AsQueryable();
                if (!string.IsNullOrEmpty(categoryUrl))
                {
                    products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category).Where(i => i.ProductCategories.Any(i => i.Category.Url == categoryUrl));
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category).Where(i => i.ProductCategories.Any(i => i.Category.Url == categoryUrl)).Where(i => i.Name.ToLower().Contains(queryString));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category).Where(i => i.Name.ToLower().Contains(queryString));
                    }
                }
                return products.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            }
        }

        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new CommerceContext())
            {
                var product = context.Products.Include(i => i.ProductCategories).FirstOrDefault(i => i.ProductId == entity.ProductId);
                if (product != null)
                {
                    product.Name = entity.Name;
                    product.Description = entity.Description;
                    product.Price = entity.Price;
                    product.ImageUrl = entity.ImageUrl;
                    product.Url = entity.Url;
                    product.IsApproved = entity.IsApproved;
                    product.IsHome = entity.IsHome;

                    product.ProductCategories = categoryIds.Select(i => new ProductCategory()
                    {
                        ProductId = entity.ProductId,
                        CategoryId = i
                    }).ToList();

                    context.SaveChanges();
                }
            }
        }
    }
}
