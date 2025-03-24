using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules.RequestModel.User
{
    public class UserResuestModel
    {
        public int? UserID { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? GenderID { get; set; }
        public int? RoleID { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Contact { get; set; }
        public int? MediaID { get; set; }
        public bool? Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Extra1 { get; set; }
        public string? Extra2 { get; set; }
        public int? flag { get; set; }

    }
    public class UserUpdateResuestModel
    {
        [Required(ErrorMessage ="UserID is reqiured")]
        public int? UserID { get; set; }
        [Required(ErrorMessage ="Name is Required")]
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? GenderID { get; set; }    
        public string? Contact { get; set; }    
        public bool? Status { get; set; }
        public string? Remarks { get; set; }  
        public string? Extra1 { get; set; }
        public string? Extra2 { get; set; }
        public string? ProfileImage { get; set; }
        public int? flag { get; set; }
    }
    public class UserChangePasswordRequestModel
    {
        [Required(ErrorMessage = "UserID is reqiured")]
        [Range(1, int.MaxValue, ErrorMessage = "UserID must be greater than or equal to 1")]
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Old Password is reqiured")]
        [StringLength(32)]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is reqiured")]
        [StringLength(32)]
        public string? NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm password is reqiured")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirm password do not match.")]
        public string? ConfirmPassword { get; set; }      
        public int? flag { get; set; }
    }
}

