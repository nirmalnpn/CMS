using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Modules
{
    public class ReturnModel<T>
    {
        public int? Status { get; set; }
        public string?  Message { get; set; }
        public T? Result { get; set; }
    }
}
