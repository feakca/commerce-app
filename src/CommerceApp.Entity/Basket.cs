using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.Entity
{
    public class Basket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
