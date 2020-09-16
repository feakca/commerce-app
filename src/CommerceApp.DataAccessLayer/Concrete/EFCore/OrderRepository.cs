using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommerceApp.DataAccessLayer.Concrete.EFCore
{
    public class OrderRepository : GenericRepository<Order, CommerceContext>, IOrderRepository
    {
        public List<Order> GetOrders(string userId)
        {
            using (var context = new CommerceContext())
            {
                var orders = context.Orders.Include(i => i.OrderItems).ThenInclude(i => i.Product).AsQueryable();
                if (!string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(i => i.UserId == userId);
                }
                else
                {
                    orders = orders.Where(i => i.OrderState == OrderStateEnum.Waiting || i.OrderState == OrderStateEnum.Shipped);
                }
                return orders.ToList();
            }
        }

        public void UpdateOrderState(int orderId, string[] orderStates)
        {
            using (var context = new CommerceContext())
            {
                if (orderStates.Any(i => i == "completed"))
                {
                    var order = context.Orders.Where(i => i.Id == orderId).FirstOrDefault();
                    if (order != null)
                    {
                        order.OrderState = OrderStateEnum.Completed;
                        Update(order);
                    }
                }
                else if (orderStates.Any(i => i == "shipped"))
                {
                    var order = context.Orders.Where(i => i.Id == orderId).FirstOrDefault();
                    if (order != null)
                    {
                        order.OrderState = OrderStateEnum.Shipped;
                        Update(order);
                    }
                }
                else
                {
                    var order = context.Orders.Where(i => i.Id == orderId).FirstOrDefault();
                    if (order != null)
                    {
                        order.OrderState = OrderStateEnum.Waiting;
                        Update(order);
                    }
                }
            }
        }
    }
}
