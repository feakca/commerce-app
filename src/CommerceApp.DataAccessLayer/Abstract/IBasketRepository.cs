using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.DataAccessLayer.Abstract
{
    public interface IBasketRepository : IRepository<Basket>
    {
        Basket GetBasketByUserId(string userId);
        void RemoveFromBasket(int basketId, int productId);
        void ClearBasket(int basketId);
    }
}
