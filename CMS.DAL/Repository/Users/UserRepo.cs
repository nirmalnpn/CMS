using CMS.DAL.Interface;
using CMS.DAL.Interface.Users;
using CMS.Modules.Modules;
using CMS.Modules.Modules.ParamModel;
using CMS.Modules.Modules.RequestModel.User;
using CMS.Modules.Modules.ResponseModel;
using CMS.Modules.Modules.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Repository.Users
{
    public class UserRepo : IUser
    {
        private readonly IDapperDao _dao;
        private readonly string PROC = "[Users].[spUserProc]";
        private readonly string PROC1 = "[Users].[spLogin]";
        private readonly string PROC2 = "[Masters].[spOTP]";
        private readonly IEmailService _emailService;
        private readonly IOtpRepository _otpRepository;
        //private readonly SmtpSettings _smtpSettings;
        private readonly IConfiguration _configuration;
        public UserRepo(IDapperDao dao, EmailService emailService, IOtpRepository otpRepository, IConfiguration configuration)
        {
            _dao = dao;
            _emailService = emailService;
            _otpRepository = otpRepository;
            _configuration = configuration;
            // _smtpSettings = smtpSettings;
        }

        public async Task<ReturnMessages> ChangePassword(UserChangePasswordRequestModel model)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC, model);
            return data;
        }

        public async Task<Tuple<UserResponseModel, StatusModel>> Details(UserParamModel model)
        {
            var data = await _dao.ExecuteQueryTupleAsync<UserResponseModel, StatusModel>(PROC, model);
            return Tuple.Create(data.Item1, data.Item2);
        }

        public async Task<Tuple<List<UserResponseModel>, StatusModel>> GetList(UserParamModel model)
        {
            var data = await _dao.ExecuteQueryTupleListAsync<UserResponseModel, StatusModel>(PROC, model);
            return Tuple.Create(data.Item1, data.Item2);
        }

        public async Task<Tuple<UserDetailsResponseModel, StatusModel>> Login(LoginParamModel model)
        {
            var data = await _dao.ExecuteQueryTupleAsync<UserDetailsResponseModel, StatusModel>(PROC1, model);
            return Tuple.Create(data.Item1, data.Item2);
        }

        public async Task<Tuple<UserResuestModel, StatusModel>> Query(UserResuestModel model)
        {
            var data = await _dao.ExecuteQueryTupleAsync<UserResuestModel, StatusModel>(PROC, model);
            return Tuple.Create(data.Item1, data.Item2);
        }

        public async Task<Tuple<ReturnMessages>> Query1(UserResuestModel model)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC, model);
            return Tuple.Create(data);
        }



        public async Task<ReturnMessages> Update(UserUpdateResuestModel model)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC, model);
            return data;
        }

        public async Task<ReturnMessages> User(UserParamModel model)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC, model);
            return data;
        }

        public async Task<Tuple<ReturnMessages>> UserCreate(UserCreateRequestModel model)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC, model);
            return Tuple.Create(data);
        }
        public async Task<OtpResult> SendOtpAsync(string email)
        {
            // Generate the OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Send the OTP via email
            var emailResult = await _emailService.SendEmailAsync(email, _configuration["Smtp:Subject"].ToString(), $"Your OTP code is: {otpCode}");

            if (emailResult.Success)
            {
                var model = new OPTRequestModel
                {
                    Email = email,
                    OtpCode = Convert.ToInt32(otpCode),
                    ExpairedDate = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Smtp:ExpairedTime"])),
                    Flag = "AddOTP"

                };
                // Store the OTP in the database
                await _otpRepository.StoreOtpAsync(model);
                return new OtpResult { Success = true };
            }

            return new OtpResult { Success = false, Errors = new[] { "Failed to send OTP email." } };
        }

        public async Task<ReturnMessages> ValidateOtpAsync(ValidateOtpModel model)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC2, model);
            return data;
        }

        public async Task<ReturnMessages> VerifiedOtp(string Email, string otpCode)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC2, new { @Flag = "VerifyOTP", @Email = Email, @otpCode = otpCode });
            return data;
        }

        public async Task<ReturnMessages> CheckEmail(string Email)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC2, new { @Flag = "CheckEmail", @Email = Email });
            return data;
        }
        public async Task<ReturnMessages> ResetPassword(ResetPasswordModel model)
        {
            var data = await _dao.ExecuteQueryFirstAsync<ReturnMessages>(PROC, model);
            return data;
        }

        public async Task<Tuple<List<OTPModel>, StatusModel>> GetOTPList(int UserID)
        {
            var data = await _dao.ExecuteQueryTupleListAsync<OTPModel, StatusModel>(PROC2, new { @Flag = "GetOTPList", @UserID = UserID });
            return Tuple.Create(data.Item1, data.Item2);
        }

        public async Task<OtpResult> SendMail(string email)
        {
            var emailResult = await _emailService.SendEmailAsync(email, "User create", $"Your user is: {email}");
            return new OtpResult { Success = true };
        }
    }
}
