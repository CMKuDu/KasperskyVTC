using Kaspersky.Infrastructure.Entity.Login;
using Kaspersky.Infrastructure.Entity.SignUp;
using Kaspersky.Infrastructure.ResponseService;

namespace Kaspersky.Infrastructure.InterfaceService
{
    public interface IAccountService
    {
        public Task<CommonResponseBase> SignUpAsync(SignUpModel model);
        public Task<CommonResponseBase> SignInAsync(LoginModel model);
        public Task<dynamic> GetInfo(string userId);
        Task<ResponseBaseToken> RefreshAccessToken(string refreshToken);
    }
}
