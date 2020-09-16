using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Models
{
    public class OrderListModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string OrderNote { get; set; }

        public OrderStateEnum OrderState { get; set; }

        public List<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();

        public double TotalPrice()
        {
            return OrderItems.Sum(i => i.Price * i.Quantity);
        }
    }

    public class OrderItemModel
    {
        public int OrderItemId { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
