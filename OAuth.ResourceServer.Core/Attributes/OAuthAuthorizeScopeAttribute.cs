using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using OAuth.ResourceServer.Core.Server;
using DNOA = DotNetOpenAuth.OAuth2;

namespace OAuth.ResourceServer.Core.Attributes
{
    // Attribute to apply to methods on WebAPI controller methods to restrict access to
    // those in possession of an authorization token with specified scopes
    public class OAuthAuthorizeScopeAttribute : AuthorizeAttribute
    {
        private static readonly RSACryptoServiceProvider Decrypter;
        private static readonly RSACryptoServiceProvider SignatureVerifier;

        // Get the keys from wherever they are stored
        static OAuthAuthorizeScopeAttribute()
        {
            Decrypter = new ResourceServerKeyManager().GetDecrypter();
            SignatureVerifier = new AuthorizationServerKeyManager().GetSignatureVerifier();
        }

        // Which scopes are required to gain access
        public string[] RequiredScopes { get; set; }

        public OAuthAuthorizeScopeAttribute(params string[] requiredScopes)
        {
            RequiredScopes = requiredScopes;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                base.OnAuthorization(actionContext);

                // Bail if no auth header or the header isn't bearing a token for us
                var authHeader = actionContext.Request.Headers.FirstOrDefault(x => x.Key == "Authorization");
                if (authHeader.Value == null || !authHeader.Value.Any())
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    return;
                }
                var authHeaderValue = authHeader.Value.FirstOrDefault(x => x.StartsWith("Bearer "));
                if (authHeaderValue == null)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    return;
                }

                // Have the DotNetOpenAuth resource server inspect the provided request using the configured keys
                // This checks both that the token is ok and that the token grants the scope required by
                // the required scope parameters to this attribute
                var resourceServer = new DNOA.ResourceServer(new StandardAccessTokenAnalyzer(SignatureVerifier, Decrypter));
                var principal = resourceServer.GetPrincipal(actionContext.Request,RequiredScopes);
                if (principal != null)
                {
                    // Things look good.  Set principal for the resource to use in identifying the user so it can act accordingly
                    Thread.CurrentPrincipal = principal;
                    HttpContext.Current.User = principal;
                    // Dont understand why the call to GetPrincipal is setting actionContext.Response to be unauthorized
                    // even when the principal returned is non-null
                    // If I do this code the same way in a delegating handler, that doesn't happen
                    actionContext.Response = null;
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }
            catch (SecurityTokenValidationException)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            catch (ProtocolFaultResponseException)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
