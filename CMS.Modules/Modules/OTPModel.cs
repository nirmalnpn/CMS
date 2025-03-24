using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules
{
    public class OTPModel
    {
        public int? ID { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public int? OtpCode { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpairedDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }
    public class OPTRequestModel
    {
        [Required(ErrorMessage = "Email is reqiured")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage ="OPT Code is reqiured")]
        public int? OtpCode { get; set; }
        [Required(ErrorMessage ="Expaired date is reqiured")]
        public DateTime? ExpairedDate { get; set; }
        public string? Flag { get; set; }
    }
    public class ValidateOtpModel
    {       
        public string? Email { get; set; }
        public int? OtpCode { get; set; }
        public DateTime? ExpairedDate { get; set; }
        public string? Flag { get; set; }
    }
    public class OPTPramModel    {
       
        public string? Email { get; set; }
        public int? OtpCode { get; set; }
    }
    
}

