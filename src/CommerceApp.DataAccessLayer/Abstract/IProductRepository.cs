using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.DataAccessLayer.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetProductDetails(string productUrl);
        List<Product> GetProductsByCategory(string categoryUrl, int page, int pageSize);
        List<Product> GetHomePageProducts();
        int GetCountByCategory(string categoryUrl);
        int GetCountByCategoryWithSearch(string categoryUrl, string queryString);
        List<Product> GetProductsByCategoryWithSearch(string categoryUrl, int page, int pageSize, string queryString);
        Product GetByIdWithCategories(int id);
        void Update(Product entity, int[] categoryIds);
        void Create(Product entity, int[] categoryIds);
    }
}
