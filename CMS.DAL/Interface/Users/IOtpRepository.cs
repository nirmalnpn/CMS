using CMS.Modules.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Interface.Users
{
    public interface IOtpRepository
    {
        Task<ReturnMessages> StoreOtpAsync(OPTRequestModel model);
    }
}
