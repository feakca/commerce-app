using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Models
{
    public class OrderModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string County { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Order Note")]
        public string OrderNote { get; set; }

        [Required]
        [Display(Name = "Holder Name")]
        public string CardName { get; set; }
        [Required]
        [DataType(DataType.CreditCard)]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }
        [Required]
        [Display(Name = "Expiration(Month)")]
        public string ExpirationMonth { get; set; }
        [Required]
        [Display(Name = "Expiration(Year)")]
        public string ExpirationYear { get; set; }
        [Required]
        [Display(Name = "CVV2")]
        public string Cvv { get; set; }

        public BasketModel BasketModel { get; set; }
    }
}
