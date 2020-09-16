using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.Entity
{
    public class Order
    {
        public int Id { get; set; }
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

        public string PaymentId { get; set; }
        public string ConversationId { get; set; }

        public OrderStateEnum OrderState { get; set; } = OrderStateEnum.Waiting;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStateEnum
    {
        Waiting = 0,
        Shipped = 1,
        Completed = 2
    }
}
