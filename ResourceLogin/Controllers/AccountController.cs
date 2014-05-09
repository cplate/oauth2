using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceLogin.Encoding;
using ResourceLogin.Models;

namespace ResourceLogin.Controllers
{
    // Simple login mechanism to simulate user authenticating to a resource
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            // No checking of password for this sample.  Just care about the username
            // as that's what we're including in the token to send back to the authorization server

            // Corresponds to shared secret the authorization server knows about for this resource
            const string encryptionKey = "WebAPIsAreAwesome";
                
            // Build token with info the authorization server needs to know
            var tokenContent = model.UserName + ";" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + ";" + model.RememberMe;
            var encryptedToken = EncodingUtility.Encode(tokenContent, encryptionKey);

            // Redirect back to the authorization server, including the authentication token 
            // Name of authentication token corresponds to that known by the authorization server
            returnUrl += (returnUrl.Contains("?") ? "&" : "?");
            returnUrl += "resource-authentication-token=" + encryptedToken;
            var url = new Uri(returnUrl);
            var redirectUrl = url.ToString();

            // URL Encode the values of the querystring parameters
            if (url.Query.Length > 1)
            {                    
                var helper = new UrlHelper(HttpContext.Request.RequestContext);
                var qsParts = HttpUtility.ParseQueryString(url.Query);
                redirectUrl = url.GetLeftPart(UriPartial.Path) + "?" + String.Join("&",qsParts.AllKeys.Select(x => x + "=" + helper.Encode(qsParts[x])));
            }

            return Redirect(redirectUrl);
        }
    }
}
