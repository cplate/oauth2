using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using OAuth.ResourceServer.Core.Attributes;

namespace Resource2API.Controllers
{
    // Sample controller with methods accessible by those with Resource1-Read or Resource1-Write scopes
    public class NumberController : ApiController
    {
        private static int _number;

        [OAuthAuthorizeScope("Resource2-Read")]
        public IEnumerable<string> Get()
        {
            return new [] { User.Identity.Name, _number.ToString(CultureInfo.InvariantCulture) }; // passing the name back just to show we've got it
        }

        [OAuthAuthorizeScope("Resource2-Write")]
        public void Put(int id, int value)
        {
            _number = value;
        }
    }
}