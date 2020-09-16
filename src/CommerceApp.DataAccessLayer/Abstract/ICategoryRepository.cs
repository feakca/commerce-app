using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.DataAccessLayer.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Category GetByIdWithProducts(int id);
        void ProductDeleteFromCategory(int productId, int categoryId);
    }
}
