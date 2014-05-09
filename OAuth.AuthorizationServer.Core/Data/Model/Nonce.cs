using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth.AuthorizationServer.Core.Data.Model
{
    // Used internally by DotNetOpenAuth
    public class Nonce
    {
        [Key, Column(Order = 0), MaxLength(500)]
        public string Context { get; set; }
        [Key, Column(Order = 1)]
        public string Code { get; set; }
        [Key, Column(Order = 2)]
        public System.DateTime Timestamp { get; set; }
    }
}
