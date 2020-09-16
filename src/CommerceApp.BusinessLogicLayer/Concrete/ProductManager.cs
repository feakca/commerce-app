using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.DataAccessLayer.Concrete.EFCore;
using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace CommerceApp.BusinessLogicLayer.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void Create(Product entity)
        {
            _productRepository.Create(entity);
        }

        public bool Create(Product entity, int[] categoryIds)
        {
            if (categoryIds.Length != 0)
            {
                _productRepository.Create(entity, categoryIds);
                return true;
            }
            else return false;
        }

        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public bool Update(Product entity, int[] categoryIds)
        {
            if (categoryIds.Length != 0)
            {
                _productRepository.Update(entity, categoryIds);
                return true;
            }
            else return false;
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public int GetCountByCategory(string categoryUrl)
        {
            return _productRepository.GetCountByCategory(categoryUrl);
        }

        public int GetCountByCategoryWithSearch(string categoryUrl, string queryString)
        {
            return _productRepository.GetCountByCategoryWithSearch(categoryUrl, queryString);
        }

        public List<Product> GetHomePageProduct()
        {
            return _productRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string productUrl)
        {
            return _productRepository.GetProductDetails(productUrl);
        }

        public List<Product> GetProductsByCategory(string categoryUrl, int page, int pageSize)
        {
            return _productRepository.GetProductsByCategory(categoryUrl, page, pageSize);
        }

        public List<Product> GetProductsByCategoryWithSearch(string categoryUrl, int page, int pageSize, string queryString)
        {
            return _productRepository.GetProductsByCategoryWithSearch(categoryUrl, page, pageSize, queryString);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }
    }
}
