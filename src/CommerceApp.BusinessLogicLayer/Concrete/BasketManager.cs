using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.BusinessLogicLayer.Concrete
{
    public class BasketManager : IBasketService
    {
        private IBasketRepository _basketRepository;
        public BasketManager(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public void AddToBasket(string userId, int productId, int quantity)
        {
            var basket = GetBasketByUserId(userId);
            if (basket != null)
            {
                var index = basket.BasketItems.FindIndex(i => i.ProductId == productId);
                if (index < 0)
                {
                    basket.BasketItems.Add(new BasketItem()
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        BasketId = basket.Id
                    });
                }
                else
                {
                    basket.BasketItems[index].Quantity += quantity;
                }
            }
            _basketRepository.Update(basket);
        }

        public void ClearBasket(int basketId)
        {
            _basketRepository.ClearBasket(basketId);
        }

        public Basket GetBasketByUserId(string userId)
        {
            return _basketRepository.GetBasketByUserId(userId);
        }

        public void InitializeBasket(string userId)
        {
            _basketRepository.Create(new Basket() { UserId = userId });
        }

        public void RemoveFromBasket(string userId, int productId)
        {
            var basket = _basketRepository.GetBasketByUserId(userId);
            if (basket != null)
            {
                _basketRepository.RemoveFromBasket(basket.Id, productId);
            }
        }
    }
}
