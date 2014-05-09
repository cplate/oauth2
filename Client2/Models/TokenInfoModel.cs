using System.Web;

namespace Client2.Models
{
    public class TokenInfoModel
    {
        public HttpCookie TokenCookie { get; set; }
        public string Message { get; set; }
    }
}