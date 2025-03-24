using CMS.DAL.Interface;
using CMS.DAL.Interface.Users;
using CMS.Modules.Modules;
using CMS.Modules.Modules.ParamModel;
using CMS.Modules.Modules.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Repository.Users
{
    public class GenderRepo : IGender
    {
        private readonly IDapperDao _dao;
        private string PROC = "[Users].[spGenderProc]";

        public GenderRepo(IDapperDao dao)
        {
            _dao = dao;
        }

        public async Task<Tuple<List<GenderResponseModel>, StatusModel>> Get(GenderParamModel model)
        {
            var data = await _dao.ExecuteQueryTupleListAsync<GenderResponseModel, StatusModel>(PROC, model);
            return Tuple.Create(data.Item1, data.Item2);
        }
    }
}
