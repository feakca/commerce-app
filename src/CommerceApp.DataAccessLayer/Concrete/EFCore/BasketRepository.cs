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
    public class BasketRepository : GenericRepository<Basket, CommerceContext>, IBasketRepository
    {
        public void RemoveFromBasket(int basketId, int productId)
        {
            using (var context = new CommerceContext())
            {
                var command = "DELETE FROM BasketItems WHERE BasketId=@basketId AND ProductId=@productId";
                var basketIdParameter = new SqlParameter("@basketId", basketId);
                var productIdParameter = new SqlParameter("@productId", productId);
                context.Database.ExecuteSqlRaw(command, basketIdParameter, productIdParameter);
            }
        }

        public Basket GetBasketByUserId(string userId)
        {
            using (var context = new CommerceContext())
            {
                return context.Baskets.Where(i => i.UserId == userId).Include(i => i.BasketItems).ThenInclude(i => i.Product).FirstOrDefault();
            }
        }

        public override void Update(Basket entity)
        {
            using (var context = new CommerceContext())
            {
                context.Baskets.Update(entity);
                context.SaveChanges();
            }
        }

        public void ClearBasket(int basketId)
        {
            using (var context = new CommerceContext())
            {
                var command = "DELETE FROM BasketItems WHERE BasketId=@basketId";
                var basketIdParameter = new SqlParameter("@basketId", basketId);
                context.Database.ExecuteSqlRaw(command, basketIdParameter);
            }
        }
    }
}
