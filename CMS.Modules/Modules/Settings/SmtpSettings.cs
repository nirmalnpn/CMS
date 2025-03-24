using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules.Settings
{
    public class SmtpSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? SenderName { get; set; }
        public string? Subject { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int? ExpairedTime { get; set; }
        public bool EnableSsl { get; set; }
    }
}
