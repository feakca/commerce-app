using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.BusinessLogicLayer.Abstract
{
    public interface IBasketService
    {
        void InitializeBasket(string userId);
        Basket GetBasketByUserId(string userId);
        void AddToBasket(string userId, int productId, int quantity);
        void RemoveFromBasket(string userId, int productId);
        void ClearBasket(int basketId);
    }
}
