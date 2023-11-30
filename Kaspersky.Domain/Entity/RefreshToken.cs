namespace Kaspersky.Domain.Entity
{
    public class RefreshToken
    {
        public int Id { get; set; }  // Đây là một ID duy nhất cho mỗi refreshToken, bạn có thể tùy chọn sử dụng kiểu dữ liệu phù hợp
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
    }
}
