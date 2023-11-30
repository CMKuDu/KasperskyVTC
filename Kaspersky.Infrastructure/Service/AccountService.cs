using Kaspersky.Infrastructure.Entity.Login;
using Kaspersky.Infrastructure.Entity.SignUp;
using Kaspersky.Infrastructure.InterfaceService;
using Kaspersky.Infrastructure.ResponseService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Kaspersky.Persistence.Data;
using Kaspersky.Persistence.Contracts;
using Kaspersky.Persistence.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

namespace Kaspersky.Infrastructure.Service
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshTokenService _refreshTokenService;
        public AccountService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IRefreshTokenService refreshTokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<dynamic> GetInfo(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Người dùng không tồn tại
                return new { Status = "Error", Message = "User not found" };
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userInfo = new
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles,
                Status = "Success",
                Message = string.Empty
            };
            return userInfo;
        }

        public async Task<ResponseBaseToken> RefreshAccessToken(string refreshToken)
        {
            var userPrincipal = _httpContextAccessor.HttpContext!.User;
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new ResponseBaseToken
                {
                    Status = ResponseConst.Error,
                    Message = "User ID not found in claims"
                };
            }

            // Kiểm tra xem Refresh Token có hợp lệ không
            var storedRefreshToken = await _unitOfWork.RefreshToken.GetRefreshTokenByToken(refreshToken);

            if (storedRefreshToken == null || storedRefreshToken.Invalidated || storedRefreshToken.Used)
            {
                return new ResponseBaseToken
                {
                    Status = ResponseConst.Error,
                    Message = "Invalid refresh token"
                };
            }

            // Tạo AccessToken mới
            var user = await _userManager.FindByIdAsync(userId);
            var newAccessToken = GetToken(user);

            // Đánh dấu Refresh Token đã được sử dụng
            storedRefreshToken.Used = true;
            _unitOfWork.RefreshToken.Update(storedRefreshToken);

            return new ResponseBaseToken
            {
                Status = ResponseConst.Success,
                Message = "Access token refreshed successfully",
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken)
            };
        }

        public async Task<CommonResponseBase> SignInAsync(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);
            if (!result.Succeeded)
            {
                return new ResponseBaseToken
                {
                    Status = ResponseConst.Error,
                    Message = LoginConst.UserDoesnotExist
                };
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            // Lấy userId từ thông tin đăng nhập
            var refreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);
            await _unitOfWork.RefreshToken.Add(refreshToken);
            _unitOfWork.Compele();
            var newAccessToken = GetToken(user);

            return new ResponseBaseToken
            {
                Status = ResponseConst.Success,
                Message = $"Welcome {model.Email}!",
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<CommonResponseBase> SignUpAsync(SignUpModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                // Email đã tồn tại, trả về thông báo lỗi
                return new CommonResponseBase
                {
                    Status = ResponseConst.Error,
                    Message = SignupConst.UserExists,

                    ValidationErrors = new Dictionary<string, List<string>>
                    {
                        { "Email", new List<string> { "The Email is already taken." } }
                    }
                };
            }
            if (string.IsNullOrWhiteSpace(model.Email) || !IsEmailFormatValid(model.Email))
            {
                // Xử lý các trường hợp không hợp lệ của Email
                var validationErrors = new Dictionary<string, List<string>>();

                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    validationErrors.Add("Email", new List<string> { "The Email field is required." });
                }

                if (!IsEmailFormatValid(model.Email))
                {
                    if (validationErrors.ContainsKey("Email"))
                    {
                        validationErrors["Email"].Add("The Email field is not a valid e-mail address.");
                    }
                    else
                    {
                        validationErrors.Add("Email", new List<string> { "The Email field is not a valid e-mail address." });
                    }
                }

                return new CommonResponseBase
                {
                    Status = ResponseConst.Error,
                    Message = "Validation failed.",
                    ValidationErrors = validationErrors
                };
            }

            if (!IsPasswordFormatValid(model.Password) || model.Password != model.ConfirPassword)
            {
                var errorMessage = !IsPasswordFormatValid(model.Password)
                    ? "Password must contain at least one lowercase letter, one uppercase letter, and one digit."
                    : "Password and ConfirmPassword do not match.";

                return new CommonResponseBase
                {
                    Status = ResponseConst.Error,
                    Message = errorMessage
                };
            }
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                // Thành công, trả về thông báo thành công
                return new CommonResponseBase
                {
                    Status = ResponseConst.Success,
                    Message = SignupConst.CreatedSuccessfully
                };
            }
            else
            {
                // Thất bại, trả về thông báo thất bại cùng với các lỗi chi tiết
                var errorMessages = result.Errors.Select(error => error.Description).ToList();
                return new CommonResponseBase
                {
                    Status = ResponseConst.Error,
                    Message = SignupConst.CreatedFailure,
                    Errors = errorMessages.Any() ? errorMessages : null!
                };
            }
        }
        private JwtSecurityToken GetToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? ""));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512Signature)
            );

            return token;
        }
        private bool IsPasswordFormatValid(string password)
        {
            // Biểu thức chính quy kiểm tra mật khẩu có ít nhất một chữ thường, một chữ hoa và một số
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?!.*[@#$%^&+=]).{8,}$");

            return regex.IsMatch(password);
        }
        private bool IsEmailFormatValid(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
