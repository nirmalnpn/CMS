using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules.RequestModel.User
{
    public class UserCreateRequestModel{
        [Required(ErrorMessage ="Name is reqiured")]
        [StringLength(50)]
        public string? Name { get; set; }
        [EmailAddress(ErrorMessage ="Please enter a valid email address")]
        public string? Email { get; set; }
        [Required(ErrorMessage ="Password is reqiured")]
        [StringLength(32)]
        public string? Password { get; set; }
        [Required(ErrorMessage ="Confirm password is reqiured")]
        [Compare("Password", ErrorMessage ="The password and confirm password do not match.")]      
        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage ="Role is reqiured")]
        [Range(1, int.MaxValue, ErrorMessage = "RoleID must be greater than or equal to 1")]
        public int? RoleID { get; set; }
        public int? flag { get; set; }
    }
}
