using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules.ResponseModel
{
    public class GenderResponseModel
    {
        public int? GenderID { get; set; }
        public string? Gender { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }        
    }
}
