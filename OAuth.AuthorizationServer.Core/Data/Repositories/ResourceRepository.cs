using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using OAuth.AuthorizationServer.Core.Data.Model;

namespace OAuth.AuthorizationServer.Core.Data.Repositories
{
    public class ResourceRepository : GenericRepository<Resource>
    {
        public ResourceRepository()
            : base(new OAuthDataContext())
        {
        }

        public ResourceRepository(IObjectContextAdapter context)
            : base(context)
        {
        }

        // These aren't changed by the authorization server so load once
        private static IEnumerable<Resource> _resources;
        private IEnumerable<Resource> Resources
        {
            get
            {
                if (_resources != null) return _resources;

                _resources = Find(x => x.Scopes).ToList();

                return _resources;
            }
        } 

        // Find the single resource that supports all of the given scopes
        // If there are multiple, throw exception as then we don't know which
        // resource is desired
        public Resource FindWithSupportedScopes(HashSet<string> desiredScopeIdentifiers)
        {
            var matches = Resources.Where(x => desiredScopeIdentifiers.IsSubsetOf(x.SupportedScopes)).ToList();
            if (matches.Count > 1)
            {
                throw new Exception("Ambiguous scopes");
            }
            return matches.FirstOrDefault();
        }

        public override Resource GetById(object id)
        {
            return Resources.FirstOrDefault(x => x.Id == (int)id);
        }
    }
}
