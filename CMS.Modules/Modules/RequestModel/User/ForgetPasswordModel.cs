using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules.RequestModel.User
{
    public class ForgetPasswordModel
    {
        [Required(ErrorMessage ="Email is reqiured")]
        [EmailAddress]
        public string? Email { get; set; }
    }
    public class ResetPasswordModel
    {
        public string? Email { get; set; }
        public string? OtpCode { get; set; }
        [StringLength(32)]
        [Required(ErrorMessage = "New Password is reqiured")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is reqiured")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirm password do not match.")]
        public string? ConfirmPassword { get; set; }
        public int? flag { get; set; }
    }
    public class ValidateOtpModel1
    {
        [Required(ErrorMessage = "Email is reqiured")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage ="OPT is reqiured")]
        public string? OtpCode { get; set; }
        [Required(ErrorMessage ="Password is reqiured")]
        public string? NewPassword { get; set; }
    }

    public class OtpResult
    {
        public bool Success { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
    public class EmailResult
    {
        public bool Success { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
