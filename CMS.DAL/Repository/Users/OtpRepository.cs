using CMS.DAL.Interface;
using CMS.DAL.Interface.Users;
using CMS.Modules.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Repository.Users
{
    public class OtpRepository : IOtpRepository
    {       
        private readonly string PROC2 = "Masters.spOTP";
        private readonly IDapperDao _dao;

        public OtpRepository(IDapperDao dao)
        {
            _dao = dao;
        }
        public async Task<ReturnMessages> StoreOtpAsync(OPTRequestModel model)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC2, model);
            return data;
        }
    }
}
