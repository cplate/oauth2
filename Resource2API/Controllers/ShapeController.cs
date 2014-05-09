using System.Collections.Generic;
using System.Web.Http;
using OAuth.ResourceServer.Core.Attributes;

namespace Resource2API.Controllers
{
    // Sample controller with methods accessible by those with Resource1-Read scope
    public class ShapeController : ApiController
    {
        [OAuthAuthorizeScope("Resource2-Read")]
        public IEnumerable<string> Get()
        {
            return new [] { User.Identity.Name, "square" }; // passing the name back just to show we've got it
        }      
    }
}