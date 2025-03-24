using CMS.Modules.Modules;
using CMS.Modules.Modules.ParamModel;
using CMS.Modules.Modules.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Interface.Users
{
    public interface IGender
    {
        Task<Tuple<List<GenderResponseModel>,StatusModel>>Get(GenderParamModel model);
    }
}
