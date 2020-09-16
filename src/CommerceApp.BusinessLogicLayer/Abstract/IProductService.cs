using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.BusinessLogicLayer.Abstract
{
    public interface IProductService
    {
        Product GetById(int id);
        List<Product> GetAll();
        void Create(Product entity);
        bool Create(Product entity, int[] categoryIds);
        void Update(Product entity);
        bool Update(Product entity, int[] categoryIds);
        void Delete(Product entity);

        List<Product> GetProductsByCategory(string categoryUrl, int page, int pageSize);
        Product GetProductDetails(string productUrl);
        int GetCountByCategory(string categoryUrl);
        List<Product> GetHomePageProduct();
        int GetCountByCategoryWithSearch(string categoryUrl, string queryString);
        List<Product> GetProductsByCategoryWithSearch(string categoryUrl, int page, int pageSize, string queryString);
        Product GetByIdWithCategories(int id);
    }
}
