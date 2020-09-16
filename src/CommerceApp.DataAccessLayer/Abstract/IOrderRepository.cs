using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.DataAccessLayer.Abstract
{
    public interface IOrderRepository : IRepository<Order>
    {
        List<Order> GetOrders(string userId);
        void UpdateOrderState(int orderId, string[] orderStates);
    }
}
