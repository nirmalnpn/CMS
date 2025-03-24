using CMS.DAL.Interface;
using CMS.DAL.Interface.Users;
using CMS.Modules.Modules;
using CMS.Modules.Modules.ParamModel;
using CMS.Modules.Modules.RequestModel;
using CMS.Modules.Modules.RequestModel.User;
using CMS.Modules.Modules.ResponseModel;
using CMS.Shared;
using CMS.Shared.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SixLabors.ImageSharp;
using System;
using System.Reflection;


namespace CMS.Api.Controllers.Users
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UserController : BaseAPIController
    {
        private readonly IUser _user;
        private readonly TokenHandler _tokenHandler;
        //private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CustomExceptionMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public UserController(IUser user, TokenHandler tokenHandler, ILogger<CustomExceptionMiddleware> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _user = user;
            _tokenHandler = tokenHandler;
            logger = logger;
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
        }

        #region User List
        /// <summary>List</summary>
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnModel<List<UserResponseModel>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [HttpGet("GetList", Name = "GetList")]
        public async Task<IActionResult> GetList(int UserID)
        {
            // Validate UserID
            if (UserID <= 0)
            {
                var msg = new StatusModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = GlobalSettings.ResponseMessages.IdCheckMsg
                };
                return BadRequest(msg);
            }
            _logger.LogInformation("Log Message from Information mathod");
            // Prepare the query parameters
            var model = new UserParamModel
            {
                flag = 1,
                UserID = UserID
            };

            var (response, statusMsg) = await _user.GetList(model);
            var data = new ReturnModel<List<UserResponseModel>>
            {
                Status = statusMsg.Status,
                Message = statusMsg.Message,
                Result = response
            };
            // Check if result is null or empty
            if (data.Status == StatusCodes.Status200OK && (data.Result == null || !data.Result.Any()))
            {
                data.Status = StatusCodes.Status404NotFound;
                data.Message = GlobalSettings.ResponseMessages.NotDataFound;
            }

            // Return the appropriate response based on the status code
            return data.Status switch
            {
                StatusCodes.Status200OK => Ok(data),                   // Success
                StatusCodes.Status404NotFound => NotFound(statusMsg),  // Resource not found             
                _ => BadRequest(statusMsg)                            // Any other unhandled status
            };
        }
        #endregion
        #region Add User
        /// <summary>User Create</summary>     

        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StatusModel))]
        [HttpPost("Create", Name = "Create")]
        public async Task<IActionResult> Create(UserCreateRequestModel model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    }
                    );
            }
            //set flag
            model.flag = 2;

            var response = await _user.UserCreate(model);
            if (response.Item1.Status == StatusCodes.Status201Created)
            {
                _user.SendMail(model.Email);
            }
            var data = new ReturnMessages
            {
                Status = response.Item1.Status,
                Message = response.Item1.Message
            };
            // Map the status code to the appropriate response
            return data.Status switch
            {
                StatusCodes.Status201Created => StatusCode(StatusCodes.Status201Created, new
                {
                    Status = data.Status,
                    Message = data.Message
                }),              // Success
                StatusCodes.Status404NotFound => NotFound(data),  // Resource not found
                StatusCodes.Status409Conflict => Conflict(data),  // Conflict, e.g., duplicate entry
                _ => BadRequest(data)                             // Any other unhandled status
            };
        }
        #endregion
        #region User Login
        /// <summary>User Login</summary> 
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDetailsResponseModel))]
        [AllowAnonymous]
        [HttpPost("Login", Name = "Login")]
        public async Task<IActionResult> Login(LoginParamModel model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Log Message from Information mathod");
                return BadRequest(
                    new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    }
                    );
            }
            _logger.LogInformation("Log Message from Information mathod");
            var (response, statusMsg) = await _user.Login(model);
            object UserData = null;

            var data = new ReturnModel<UserDetailsResponseModel>
            {
                Status = statusMsg.Status,
                Message = statusMsg.Message,
                Result = response
            };
            // Check if result is null or empty
            if (data.Status == StatusCodes.Status200OK && (data.Result == null))
            {
                data.Status = StatusCodes.Status404NotFound;
                data.Message = GlobalSettings.ResponseMessages.NotDataFound;
            }
            if (statusMsg.Status == StatusCodes.Status200OK)
            {
                data.Result.Token = _tokenHandler.GenerateToken(data.Result);
                var User = data.Result;

                UserData = new
                {
                    Status = statusMsg.Status,
                    Message = statusMsg.Message,
                    Result = new
                    {
                        UserID = User.UserID,
                        Name = User.Name,
                        Address = User.Address,
                        Email = User.Email,
                        Contact = User.Contact,
                        Gender = new
                        {
                            GenderID = User.GenderID,
                            Gender = User.Gender
                        },
                        Role = new
                        {
                            RoleID = User.RoleID,
                            Role = User.Role
                        },
                        ProfileImage = User.ProfileImage,
                        status = User.Status,
                        token = User.Token
                    }
                };
            }


            // Return the appropriate response based on the status code
            return data.Status switch
            {
                StatusCodes.Status200OK => Ok(UserData),                // Success
                StatusCodes.Status404NotFound => NotFound(statusMsg),  // Resource not found             
                _ => BadRequest(statusMsg)                            // Any other unhandled status
            };
        }
        #endregion
        #region Update User
        /// <summary>User Update</summary>
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserUpdateResuestModel))]
        [HttpPut("Update", Name = "UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateResuestModel userRequest, IFormFile? MediaFile)
        {
            // Declare variables for file paths
            var fullPath = string.Empty;
            var uploadsFolderPath = string.Empty;
            var uploadFilePath = string.Empty;

            // Validate UserID
            if (userRequest.UserID <= 0)
            {
                var msg = new StatusModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = GlobalSettings.ResponseMessages.IdCheckMsg
                };
                return BadRequest(msg);
            }

            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // Process the request if a user file is uploaded
            if (MediaFile != null && MediaFile.Length > 0)
            {
                // Generate a unique filename for the uploaded file to avoid collisions
                var fileName = Path.GetFileNameWithoutExtension(MediaFile.FileName);
                var fileExtension = Path.GetExtension(MediaFile.FileName);
                var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{fileExtension}";

                // Construct relative and absolute paths for file storage
                var mediaDirectoryName = _configuration["MEDIA:DIRECTORY_NAME"];
                var userDirectoryName = _configuration["MEDIA:USER_DIRECTORY_NAME"];

                // Check if WebRootPath is null
                if (string.IsNullOrEmpty(_environment?.WebRootPath))
                {
                    _logger.LogError("_environment.WebRootPath is null.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Server configuration error: WebRootPath is not set.");
                }

                // Check if mediaDirectoryName or userDirectoryName is null or empty
                if (string.IsNullOrEmpty(mediaDirectoryName) || string.IsNullOrEmpty(userDirectoryName))
                {
                    _logger.LogError("Configuration values for MEDIA:DIRECTORY_NAME or USER_DIRECTORY_NAME are missing or null.");

                    return StatusCode(StatusCodes.Status500InternalServerError, "Server configuration error: Media directories are not set.");
                }

                // Combine the paths
                uploadsFolderPath = Path.Combine(_environment.WebRootPath, mediaDirectoryName, userDirectoryName);

                // Log the values for debugging purposes
                _logger.LogInformation("WebRootPath: {WebRootPath}", _environment.WebRootPath);
                _logger.LogInformation("Media Directory Name: {MediaDirectoryName}", mediaDirectoryName);
                _logger.LogInformation("User Directory Name: {UserDirectoryName}", userDirectoryName);
                _logger.LogInformation("Uploads Folder Path: {uploadsFolderPath}", uploadsFolderPath);

                // Prepare the query parameters for fetching the media details
                var param = new UserParamModel
                {
                    flag = 3,
                    UserID = userRequest.UserID
                };

                // Fetch the user details            
                var (userDetails, statusMsg) = await _user.Details(param);

                // If user retrieval is successful, prepare paths for deletion
                if (statusMsg.Status == StatusCodes.Status200OK && userDetails != null)
                {
                    if (!string.IsNullOrEmpty(userDetails.ProfileImage))
                    {
                        fullPath = Path.Combine(_environment.WebRootPath, userDetails.ProfileImage);
                    }
                    else
                    {
                        fullPath = string.Empty;
                        _logger.LogWarning("User profile image is null or empty.");
                    }
                }
                else
                {
                    // Log and return an error if user details are not found
                    _logger.LogWarning("User not found.");
                    return NotFound(statusMsg);
                }

                // Combine the uploads folder path with the unique file name to get the final file path
                var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                // Update user profile image with the relative file path to store in the database
                userRequest.ProfileImage = Path.Combine(mediaDirectoryName, userDirectoryName, uploadFilePath, uniqueFileName);
                userRequest.flag = 4;

                // Call the service to update the user information in the database
                var data = await _user.Update(userRequest);
                if (data.Status == StatusCodes.Status200OK)
                {
                    try
                    {
                        // Delete the old file if it exists
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                        // Ensure the uploads folder exists, create it if necessary
                        if (!Directory.Exists(uploadsFolderPath))
                        {
                            Directory.CreateDirectory(uploadsFolderPath);
                        }

                        // Save the uploaded file to the specified path
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await MediaFile.CopyToAsync(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error and return a 500 Internal Server Error response if file upload fails
                        _logger.LogError(ex, "Error occurred while uploading the file.");
                        var errorRes = new
                        {
                            Status = StatusCodes.Status500InternalServerError,
                            Message = GlobalSettings.ResponseMessages.ServerErrorMsg
                        };
                        return StatusCode(StatusCodes.Status500InternalServerError, errorRes);
                    }
                }

                // Return appropriate response based on the service call result
                return data.Status switch
                {
                    StatusCodes.Status200OK => Ok(data),              // Success
                    StatusCodes.Status404NotFound => NotFound(data),  // Resource not found
                    StatusCodes.Status409Conflict => Conflict(data),  // Conflict, e.g., duplicate entry
                    _ => BadRequest(data)                             // Any other unhandled status
                };
            }
            else
            {
                // If no image file is uploaded, handle the update without file replacement
                userRequest.flag = 4;
                userRequest.ProfileImage = null;

                // Process the media update request and return appropriate response
                var data = await _user.Update(userRequest);
                return data.Status switch
                {
                    StatusCodes.Status200OK => Ok(data),              // Success
                    StatusCodes.Status404NotFound => NotFound(data),  // Resource not found
                    StatusCodes.Status409Conflict => Conflict(data),  // Conflict, e.g., duplicate entry
                    _ => BadRequest(data)                             // Any other unhandled status
                };
            }
        }




        #endregion
        #region User Details
        /// <summary>Details</summary>
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnModel<List<UserResponseModel>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [HttpGet("GeById", Name = "UserDetails")]
        public async Task<IActionResult> GetById(int UserID)
        {
            // Validate UserID
            if (UserID <= 0)
            {
                var msg = new StatusModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = GlobalSettings.ResponseMessages.IdCheckMsg
                };
                return BadRequest(msg);
            }
            _logger.LogInformation("Log Message from Information mathod");
            // Prepare the query parameters
            var model = new UserParamModel
            {
                flag = 3,
                UserID = UserID
            };

            var (response, statusMsg) = await _user.Details(model);
            var data = new ReturnModel<UserResponseModel>
            {
                Status = statusMsg.Status,
                Message = statusMsg.Message,
                Result = response
            };
            // Check if result is null or empty
            if (data.Status == StatusCodes.Status200OK && (data.Result == null))
            {
                data.Status = StatusCodes.Status404NotFound;
                data.Message = GlobalSettings.ResponseMessages.NotDataFound;
            }

            // Return the appropriate response based on the status code
            return data.Status switch
            {
                StatusCodes.Status200OK => Ok(data),                   // Success
                StatusCodes.Status404NotFound => NotFound(statusMsg),  // Resource not found             
                _ => BadRequest(statusMsg)                            // Any other unhandled status
            };
        }
        #endregion
        #region Update User Status
        /// <summary>Status</summary>
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnMessages))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [HttpPut("UpdateStatus", Name = "UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(UserParamModel model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }
            // Validate UserID
            if (model.UserID <= 0)
            {
                var msg = new StatusModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = GlobalSettings.ResponseMessages.IdCheckMsg
                };
                return BadRequest(msg);
            }
            // Validate UserID
            if (model.RoleID <= 0)
            {
                var msg = new StatusModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = GlobalSettings.ResponseMessages.RoleMsg
                };
                return BadRequest(msg);
            }

            _logger.LogInformation("Log Message from Information mathod");
            model.flag = 5;

            var statusMsg = await _user.User(model);
            var data = new ReturnMessages
            {
                Status = statusMsg.Status,
                Message = statusMsg.Message

            };

            // Return the appropriate response based on the status code
            return data.Status switch
            {
                StatusCodes.Status200OK => Ok(data),                   // Success
                StatusCodes.Status404NotFound => NotFound(statusMsg),  // Resource not found             
                StatusCodes.Status401Unauthorized => Unauthorized(statusMsg),  // Unauthorized             
                _ => BadRequest(statusMsg)                            // Any other unhandled status
            };
        }
        #endregion
        #region Change Password
        /// <summary>Change Password</summary>
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnMessages))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [HttpPut("ChangePassword", Name = "ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserChangePasswordRequestModel model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }
            // Validate UserID
            if (model.UserID <= 0)
            {
                var msg = new StatusModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = GlobalSettings.ResponseMessages.IdCheckMsg
                };
                return BadRequest(msg);
            }
            _logger.LogInformation("Log Message from Information mathod");
            model.flag = 6;

            var statusMsg = await _user.ChangePassword(model);
            var data = new ReturnMessages
            {
                Status = statusMsg.Status,
                Message = statusMsg.Message

            };

            // Return the appropriate response based on the status code
            return data.Status switch
            {
                StatusCodes.Status200OK => Ok(data),                   // Success
                StatusCodes.Status404NotFound => NotFound(statusMsg),  // Resource not found             
                StatusCodes.Status401Unauthorized => Unauthorized(statusMsg),  // Unauthorized user             
                _ => BadRequest(statusMsg)                            // Any other unhandled status
            };
        }
        #endregion

        #region Forget Password
        /// <summary>Forget Password</summary>
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnMessages))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Check email or user
            var response = await _user.CheckEmail(model.Email);
            if (response.Status == 200)
            {
                var result = await _user.SendOtpAsync(model.Email);

                if (result.Success)
                {
                    var data = new ReturnMessages
                    {
                        Status = StatusCodes.Status200OK,
                        Message = GlobalSettings.ResponseMessages.OTPMsg
                    };
                    return Ok(data);
                }
            }
            return BadRequest(response);
        }
        #endregion
        #region Verify OTP
        /// <summary>Verify OTP</summary>
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnMessages))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [HttpPost("ValidateOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateOTP(string Email, string otpCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var statusMsg = await _user.VerifiedOtp(Email, otpCode);

            var data = new ReturnMessages
            {
                Status = statusMsg.Status,
                Message = statusMsg.Message
            };

            // Return the appropriate response based on the status code
            return data.Status switch
            {
                StatusCodes.Status200OK => Ok(data),                   // Success
                StatusCodes.Status404NotFound => NotFound(statusMsg),  // Resource not found                            
                _ => BadRequest(statusMsg)                            // Any other unhandled status
            };
        }
        #endregion
        #region Reset Password
        /// <summary>Reset Password</summary>
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnMessages))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.flag = 7; //for reset password
            var statusMsg = await _user.ResetPassword(model);

            var data = new ReturnMessages
            {
                Status = statusMsg.Status,
                Message = statusMsg.Message
            };

            // Return the appropriate response based on the status code
            return data.Status switch
            {
                StatusCodes.Status200OK => Ok(data),                   // Success
                StatusCodes.Status404NotFound => NotFound(statusMsg),  // Resource not found                            
                _ => BadRequest(statusMsg)                            // Any other unhandled status
            };
        }
        #endregion
        #region OTP List
        /// <summary>List</summary>
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnModel<List<OTPModel>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusModel))]
        [HttpGet("GeOTPtList", Name = "GeOTPtList")]
        public async Task<IActionResult> GeOTPtList(int UserID)
        {
            // Validate UserID
            if (UserID <= 0)
            {
                var msg = new StatusModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = GlobalSettings.ResponseMessages.IdCheckMsg
                };
                return BadRequest(msg);
            }
            _logger.LogInformation("Log Message from Information mathod");
            // Prepare the query parameters
            var model = new UserParamModel
            {
                flag = 1,
                UserID = UserID
            };

            var (response, statusMsg) = await _user.GetOTPList(UserID);
            var data = new ReturnModel<List<OTPModel>>
            {
                Status = statusMsg.Status,
                Message = statusMsg.Message,
                Result = response
            };
            // Check if result is null or empty
            if (data.Status == StatusCodes.Status200OK && (data.Result == null || !data.Result.Any()))
            {
                data.Status = StatusCodes.Status404NotFound;
                data.Message = GlobalSettings.ResponseMessages.NotDataFound;
            }

            // Return the appropriate response based on the status code
            return data.Status switch
            {
                StatusCodes.Status200OK => Ok(data),                   // Success
                StatusCodes.Status404NotFound => NotFound(statusMsg),  // Resource not found             
                _ => BadRequest(statusMsg)                            // Any other unhandled status
            };
        }
        #endregion
    }
}
