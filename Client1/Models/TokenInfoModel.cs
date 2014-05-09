using System.Web;

namespace Client1.Models
{
    public class TokenInfoModel
    {
        public HttpCookie TokenCookie { get; set; }
        public string Message { get; set; }
    }
}