using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.BusinessLogicLayer.Abstract
{
    public interface ICategoryService
    {
        Category GetById(int id);
        List<Category> GetAll();
        void Create(Category entity);
        void Update(Category entity);
        void Delete(Category entity);
        Category GetByIdWithProducts(int id);
        void ProductDeleteFromCategory(int productId, int categoryId);
    }
}
