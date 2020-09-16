using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        [StringLength(15)]
        [Required]
        public string Name { get; set; }
        public string Url { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
