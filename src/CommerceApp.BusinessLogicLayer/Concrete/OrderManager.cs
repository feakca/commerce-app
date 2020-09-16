using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.BusinessLogicLayer.Concrete
{
    public class OrderManager : IOrderService
    {
        private IOrderRepository _orderRepository;

        public OrderManager(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void Create(Order entity)
        {
            _orderRepository.Create(entity);
        }

        public List<Order> GetOrders(string userId)
        {
            return _orderRepository.GetOrders(userId);
        }

        public void UpdateOrderState(int orderId, string[] orderStates)
        {
            _orderRepository.UpdateOrderState(orderId, orderStates);
        }
    }
}
