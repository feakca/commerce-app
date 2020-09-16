using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommerceApp.DataAccessLayer.Concrete.EFCore
{
    public class CategoryRepository : GenericRepository<Category, CommerceContext>, ICategoryRepository
    {
        public Category GetByIdWithProducts(int id)
        {
            using (var context = new CommerceContext())
            {
                return context.Categories.Where(i => i.CategoryId == id).Include(i => i.ProductCategories).ThenInclude(i => i.Product).FirstOrDefault();
            }
        }

        public void ProductDeleteFromCategory(int productId, int categoryId)
        {
            using (var context = new CommerceContext())
            {
                var command = "DELETE ProductCategory WHERE ProductId=@productId AND CategoryId=@categoryId";
                var productIdParameter = new SqlParameter("@productId", productId);
                var categoryIdParameter = new SqlParameter("@categoryId", categoryId);
                context.Database.ExecuteSqlRaw(command, productIdParameter, categoryIdParameter);
            }
        }
    }
}
