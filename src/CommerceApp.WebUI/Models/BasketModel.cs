using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Models
{
    public class BasketModel
    {
        public int BasketId { get; set; }
        public List<BasketItemModel> BasketItems { get; set; }

        public double TotalPrice()
        {
            return BasketItems.Sum(i => i.Price * i.Quantity);
        }
    }

    public class BasketItemModel
    {
        public int BasketItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
