using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Client2.Config;
using Client2.Models;

namespace Client2.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class MainController : Controller
    {
        private static int _number = 1;

        // Show user main screen indicating their authorization status and what they can do in current state
        public ActionResult Index(string msg)
        {
            var tokenCookie = Request.Cookies[ClientConfig.AccessTokenName];
            return View(new TokenInfoModel { TokenCookie = tokenCookie, Message = msg });
        }        

        // Remove current authorization token so reauthorization process can happen
        public ActionResult RemoveToken()
        {
            var tokenCookie = Request.Cookies[ClientConfig.AccessTokenName];
            if (tokenCookie != null)
            {
                tokenCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(tokenCookie);
            }
            return View();
        }
       
        // Make a call to an api using the authorization token
        public ActionResult CallApi(string apiAction)
        {
            var tokenCookie = Request.Cookies[ClientConfig.AccessTokenName];
            if (tokenCookie == null)
            {
                // Need to get a token
                return View("Index", new TokenInfoModel { TokenCookie = null, Message = "Token is missing.  It likely expired."});
            }

            // Assemble the API URL.  This won't be as ugly for real apps.
            var relativePath = apiAction.Contains("Number") ? "Number" : "Shape";
            var method = apiAction.Contains("Update") ? "PUT" : "GET";
            var baseUrl = apiAction.Contains("Resource1") ? ResourceConfig.Resource1BaseUrl : ResourceConfig.Resource2BaseUrl;
            
            if (method == "PUT")
            {
                relativePath += "/1?value=" + _number++;
            }

            // Submit the request to the API with our token
            var req = WebRequest.Create(baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + relativePath);
            req.Headers.Add("Authorization", "Bearer " + tokenCookie.Value);
            req.Method = method;
            req.ContentLength = 0;

            string content;
            try
            {
                var resp = req.GetResponse();
                content = new StreamReader(resp.GetResponseStream()).ReadToEnd();                
            } 
            catch (WebException ex)
            {
                content = String.Format("An error occurred, message was {0}", ex.Message);
            }

            return View((object)content);
        }
    }
}
