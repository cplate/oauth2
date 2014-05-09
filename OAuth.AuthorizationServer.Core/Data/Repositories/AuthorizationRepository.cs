using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using OAuth.AuthorizationServer.Core.Data.Model;

namespace OAuth.AuthorizationServer.Core.Data.Repositories
{
    public class AuthorizationRepository : GenericRepository<Authorization>
    {
        public AuthorizationRepository() : base(new OAuthDataContext())
        {            
        }

        public AuthorizationRepository(IObjectContextAdapter context)
            : base(context)
        {
        }

        // Determine what authorizations are in force for a given client/user pair
        public IEnumerable<Authorization> FindCurrent(string clientIdentifier, string userIdentifier, DateTime afterUtc)
        {
            var auths = from auth in Context.Authorizations 
                         where auth.Client.Id == clientIdentifier 
                         && auth.CreatedOnUtc <= afterUtc 
                         && (!auth.ExpirationDateUtc.HasValue || auth.ExpirationDateUtc.Value >= DateTime.UtcNow) 
                         && auth.User.Id == userIdentifier
                        select auth;
            return auths.ToList();
        }
    }
}
