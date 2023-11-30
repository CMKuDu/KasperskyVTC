using Kaspersky.Domain.Entity;
using Kaspersky.Infrastructure.InterfaceService;
using Kaspersky.Persistence.Contracts;

namespace Kaspersky.Infrastructure.Service
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RefreshTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddRefreshToken(RefreshToken refreshToken)
        {
            await _unitOfWork.RefreshToken.Add(refreshToken);
            _unitOfWork.Compele();
        }

        public async Task<RefreshToken> GetRefreshToken(string tokenId)
        {
            return await _unitOfWork.RefreshToken.GetRefreshToken(tokenId);

        }
        public RefreshToken GenerateRefreshToken(string userId)
        {
            // Triển khai logic để tạo Refresh Token
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                CreationDate = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMonths(1), // Thời gian hết hạn, có thể tùy chỉnh theo yêu cầu
                Used = false,
                Invalidated = false
            };

            return refreshToken;
        }

        public async Task RemoveRefreshToken(string tokenId)
        {
            var refreshToken = await _unitOfWork.RefreshToken.GetByIdAsync(tokenId);
            if (refreshToken != null)
            {
                _unitOfWork.RefreshToken.Delete(refreshToken);
                _unitOfWork.Compele();
            }
        }
        public async Task<RefreshToken> GetRefreshTokenByToken(string refreshToken)
        {
            return await _unitOfWork.RefreshToken.GetRefreshTokenByToken(refreshToken);
        }
    }
}
