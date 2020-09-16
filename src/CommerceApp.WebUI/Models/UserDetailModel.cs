using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Models
{
    public class UserDetailModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        public List<string> SelectedRoles { get; set; }
    }
}
