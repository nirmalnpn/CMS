using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules.ResponseModel
{
    public class UserResponseModel
    {
        public int? UserID { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? GenderID { get; set; }
        public string? Gdnder { get; set; }
        public int? RoleID { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? Contact { get; set; }
        public int? MediaID { get; set; }
        public string? ProfileImage { get; set; }
        public bool? Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Extra1 { get; set; }
        public string? Extra2 { get; set; }       
    }
}
