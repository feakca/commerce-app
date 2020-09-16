using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.BusinessLogicLayer.Abstract
{
    public interface IOrderService
    {
        void Create(Order entity);
        List<Order> GetOrders(string userId);
        void UpdateOrderState(int orderId, string[] orderStates);
    }
}
