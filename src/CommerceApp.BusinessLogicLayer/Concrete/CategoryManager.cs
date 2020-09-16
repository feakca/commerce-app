using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.DataAccessLayer.Concrete.EFCore;
using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.BusinessLogicLayer.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public void Create(Category entity)
        {
            _categoryRepository.Create(entity);
        }

        public void Update(Category entity)
        {
            _categoryRepository.Update(entity);
        }

        public void Delete(Category entity)
        {
            _categoryRepository.Delete(entity);
        }

        public List<Category> GetAll()
        {
            return _categoryRepository.GetAll();
        }

        public Category GetById(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public Category GetByIdWithProducts(int id)
        {
            return _categoryRepository.GetByIdWithProducts(id);
        }

        public void ProductDeleteFromCategory(int productId, int categoryId)
        {
            _categoryRepository.ProductDeleteFromCategory(productId, categoryId);
        }
    }
}
