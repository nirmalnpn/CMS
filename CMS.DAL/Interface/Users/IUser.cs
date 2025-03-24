using CMS.Modules.Modules;
using CMS.Modules.Modules.ParamModel;
using CMS.Modules.Modules.RequestModel.User;
using CMS.Modules.Modules.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Interface.Users
{
    public interface IUser
    {
        Task<Tuple<List<UserResponseModel>,StatusModel>> GetList(UserParamModel model);
        Task<Tuple<UserResponseModel,StatusModel>> Details(UserParamModel model);
        Task<Tuple<UserResuestModel,StatusModel>> Query(UserResuestModel model);
        Task<Tuple<ReturnMessages>>Query1(UserResuestModel model);
        Task<ReturnMessages>User(UserParamModel model);
        Task<ReturnMessages>ChangePassword(UserChangePasswordRequestModel model);
        Task<Tuple<ReturnMessages>>UserCreate(UserCreateRequestModel model);
        Task<ReturnMessages>Update(UserUpdateResuestModel model);
        Task<Tuple<UserDetailsResponseModel, StatusModel>> Login(LoginParamModel model);

        Task<OtpResult> SendOtpAsync(string email);
        //Task<OtpResult> ValidateOtpAsync1(string email, string otpCode, string newPassword);
        Task<ReturnMessages> VerifiedOtp(string Email, string otpCode);
        Task<ReturnMessages> ValidateOtpAsync(ValidateOtpModel model);
        Task<ReturnMessages>CheckEmail(string Email);
        Task<ReturnMessages> ResetPassword(ResetPasswordModel model);
        Task<Tuple<List<OTPModel>, StatusModel>> GetOTPList(int UserID);
        Task<OtpResult> SendMail(string email);
    }
}
