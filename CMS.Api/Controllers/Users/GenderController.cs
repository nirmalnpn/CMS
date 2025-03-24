using CMS.DAL.Interface.Users;
using CMS.Modules.Modules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CMS.Modules.Modules.ResponseModel;
using CMS.Modules.Modules.ParamModel;

namespace CMS.Api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {
        private readonly IGender _gender;

        public GenderController(IGender gender)
        {
            _gender = gender;
        }
        #region Gender List
        /// <summary>Data List</summary>
        /// <returns>Data List</returns>
        /// /// <response code="200">Returns the List</response>
        /// <response code="400">If the item is null</response>
        [HttpGet]
        [Route("GetList")]
        public async Task<IActionResult> Get()
        {
            GenderParamModel model = new GenderParamModel();
            model.flag = 1;
            var data = new ReturnModel<List<GenderResponseModel>>();
            var response = await _gender.Get(model);
            var statusMsg = response.Item2;
            data.Status = statusMsg.Status;
            data.Message = statusMsg.Message;
            data.Result = response.Item1;
            if (data.Status == 200)
            {
                var dataList = new
                {
                    code = data.Status,
                    Message = data.Message,
                    result = data.Result
                    .Select(item => new
                    {
                        genderId=item.GenderID,
                        gender=item.Gender,
                        status=item.Status
                    }).ToList()
                };
                return Ok(dataList);
            }
            else
            {
                return NotFound(statusMsg);
            }

        }
        #endregion
    }
}
