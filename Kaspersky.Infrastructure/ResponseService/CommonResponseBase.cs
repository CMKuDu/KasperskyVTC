namespace Kaspersky.Infrastructure.ResponseService
{
    public class CommonResponseBase
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public Dictionary<string, List<string>> ValidationErrors { get; set; } = new Dictionary<string, List<string>>();
    }

    public class ResponseBaseToken : CommonResponseBase
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

}
