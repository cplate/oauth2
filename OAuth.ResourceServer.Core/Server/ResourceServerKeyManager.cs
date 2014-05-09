using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace OAuth.ResourceServer.Core.Server
{
    public class ResourceServerKeyManager
    {
        // Responsible for providing the key to verify the token is intended for this resource
        public RSACryptoServiceProvider GetDecrypter()
        {
            var decrypter = new RSACryptoServiceProvider();
            var base64EncodedKey = ConfigurationManager.AppSettings["ResourceServerDecryptionKey"];
            decrypter.FromXmlString(Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedKey)));
            return decrypter;
        }
    }
}
