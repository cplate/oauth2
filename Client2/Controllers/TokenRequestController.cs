using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Client2.Config;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;

namespace Client2.Controllers
{
    // Handle obtaining an authorization token to a resource on behalf of a user
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class TokenRequestController : Controller
    {
        // Show user view to allow selection of scopes and flow
        public ActionResult Index()
        {
            return View();
        }

        // Kick off desired flow
        public ActionResult StartFlow(string[] scopes, string flowType)
        {
            var scopeList = scopes ?? Enumerable.Empty<string>();
            if (flowType == "code")
            {
                return invokeCodeFlow(scopeList);
            }
            else if (flowType == "implicit")
            {
                return invokeImplicitFlow(scopeList);
            }
            else if (flowType == "clientCredentials")
            {
                return invokeClientCredentialsFlow(scopeList);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        #region CodeFlow

        // First step in code flow is getting an access code, which will be passed to our callback action on this controller
        // Note that the user will be prompted by the authorization server to authorize the client (and login to the resource if necessary)
        // before control is given to our callback                
        private ActionResult invokeCodeFlow(IEnumerable<string> scopes)
        {
            var state = new AuthorizationState();
            foreach (var s in scopes)
            {
                state.Scope.Add(s);
            }
            state.Callback = new Uri(Request.Url, Url.Action("ExchangeAccessCodeForAuthToken", "TokenRequest"));
            
            // Here DotNetOpenAuth figures out what the request should look like (i.e., builds appropriate url)
            var r = ClientConfig.AuthorizationServerClient.PrepareRequestUserAuthorization(state);
            return r.AsActionResult();                
        }

        // When this callback is invoked in the code flow, the user has chosen to approve or deny access for this client
        // If approved, store token in cookie for use by the rest of the app
        public ActionResult ExchangeAccessCodeForAuthToken()
        {
            var accessTokenResponseState = ClientConfig.AuthorizationServerClient.ProcessUserAuthorization(this.Request);
            var gotToken = accessTokenResponseState.AccessToken != null;

            if (gotToken)
            {
                var cookie = new HttpCookie(ClientConfig.AccessTokenName, accessTokenResponseState.AccessToken)
                                 {Expires = accessTokenResponseState.AccessTokenExpirationUtc.Value, Path = "/"};
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Index", "Main", new { msg = gotToken ? "Token Granted" : "No Access Token Was Granted"});
        }

        #endregion

        #region Implicit Flow

        // Here we request the token directly, which is passed back on a url fragment
        // Typically this flow wouldn't be used from the server side and would instead be done via javascript
        // in the browser, but doing it here in this sample to reduce the maze a bit
        // Our callback will parse the fragment with javascript, as the token doesn't get sent to the server
        // Note that with this flow there are no client credentials involved, so the callback provided here
        // must match the one configured for the client in the authorization server (if one is configured)
        // Again, note that the user will be prompted to authorize the client (and potentially login to the resource)
        // before control is given to our callback      
        private ActionResult invokeImplicitFlow(IEnumerable<string> scopes)
        {            
            var scopesQueryString = HttpUtility.UrlEncode(String.Join(" ",scopes));
            var callbackQueryString = HttpUtility.UrlEncode(new Uri(Request.Url, Url.Action("CacheTokenFromImplicitFlow", "TokenRequest")).AbsoluteUri);

            return Redirect(String.Format("{0}?scope={1}&redirect_uri={2}&response_type=token&client_id={3}",
                AuthorizationServerConfig.AuthorizationEndpoint,scopesQueryString,callbackQueryString,ClientConfig.ClientId));            
        }

        // Process auth token provided by authorization server
        // Has to be done by javascript on the browser side because
        // it is passed as a fragment and doesn't come down to the server
        public ActionResult CacheTokenFromImplicitFlow()
        {
            // View needs to know this clients auth token name as the js is actually putting the token in a cookie
            // for access by the rest of the app
            return View((object)ClientConfig.AccessTokenName);
        }

        #endregion

        #region Client Credentials Flow

        // Client Credentials flow obtains a token that is independent of user
        // Client is verified using the client secret included when we configured the ClientConfig.AuthorizationServerClient
        private ActionResult invokeClientCredentialsFlow(IEnumerable<string> scopes)
        {
            string msg = null;
            bool gotToken = false;
            try
            {
                // This seems to throw ProtocolException if client is unauthorized rather than just returning no token
                var state = ClientConfig.AuthorizationServerClient.GetClientAccessToken(scopes);
                gotToken = state.AccessToken != null;

                if (gotToken)
                {
                    var cookie = new HttpCookie(ClientConfig.AccessTokenName, state.AccessToken) { Expires = state.AccessTokenExpirationUtc.Value, Path="/" };
                    Response.Cookies.Add(cookie);
                }
            }
            catch (ProtocolException ex)
            {                
                msg = ", Message was: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            // Can do this directly without DotNotOpenAuth via below
            //var req2 = HttpWebRequest.Create(AuthorizationServerConfig.TokenEndpoint);
            //req2.Method = "POST";
            //req2.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new UTF8Encoding().GetBytes(ClientConfig.ClientId + ":" + ClientConfig.ClientSecret)));
            //req2.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            //using (var streamWriter = new StreamWriter(req2.GetRequestStream()))
            //{
            //    streamWriter.Write("scope={0}&grant_type={1}", HttpUtility.UrlEncode(String.Join(" ", scopes)), "client_credentials");
            //}
            //var resp2 = req2.GetResponse();
            //var accessTokenResponseStr = new StreamReader(resp2.GetResponseStream()).ReadToEnd();
            //dynamic accessTokenInfo = JsonConvert.DeserializeObject(accessTokenResponseStr);

            //var cookie = new HttpCookie(ClientConfig.AccessTokenName, accessTokenInfo.access_token.ToString());
            //cookie.Expires = DateTime.Now.AddSeconds(int.Parse(accessTokenInfo.expires_in.ToString()));
            //Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Main", new { msg = gotToken ? "Token Granted" : "No Access Token Was Granted" + msg });
        }

        #endregion
    }
}
