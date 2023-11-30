using Kaspersky.Domain.Entity;

namespace Kaspersky.Infrastructure.InterfaceService
{
    public interface IRefreshTokenService
    {
        Task AddRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshToken(string tokenId);
        Task RemoveRefreshToken(string tokenId);
        RefreshToken GenerateRefreshToken(string userId);
        Task<RefreshToken> GetRefreshTokenByToken(string refreshToken);
    }
}
