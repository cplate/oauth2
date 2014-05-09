using System.Data.Entity.Infrastructure;
using OAuth.AuthorizationServer.Core.Data.Model;

namespace OAuth.AuthorizationServer.Core.Data.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository()
            : base(new OAuthDataContext())
        {
        }

        public UserRepository(IObjectContextAdapter context)
            : base(context)
        {
        }
    }
}
