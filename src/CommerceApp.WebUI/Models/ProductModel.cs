using CommerceApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        [Display(Name = "Name", Prompt = "Enter product name..")]
        [Required]
        [StringLength(60)]
        public string Name { get; set; }
        [Display(Name = "Price", Prompt = "Enter price info..")]
        [Range(1, 100000)]
        [Required]
        public double? Price { get; set; }
        [Display(Name = "Description", Prompt = "Enter description info..")]
        [Required]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }
        [Display(Name = "Home Page")]
        public bool IsHome { get; set; }
        public string Url { get; set; }
        public List<Category> SelectedCategories { get; set; } = new List<Category>();
    }
}
