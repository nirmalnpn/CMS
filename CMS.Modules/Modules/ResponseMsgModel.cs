using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules
{
    public class ResponseMsgModel
    {
        public string? IdCheckMsg { get; set; }
        public string? RoleMsg { get; set; }
        public string? OTPMsg { get; set; }
        public string? CheckParamMsg { get; set; }
        public string? ServerErrorMsg { get; set; }
        public string? UnauthorizedMsg { get; set; }
        public string? NotDataFound { get; set; }
        public string? FileUpload { get; set; }
    }
}
