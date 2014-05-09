using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using OAuth.AuthorizationServer.Core.Data.Model;

namespace OAuth.AuthorizationServer.Core.Data.Repositories
{
    public class ClientRepository : GenericRepository<Client>
    {   
        public ClientRepository() : base(new OAuthDataContext())
        {            
        }

        public ClientRepository(IObjectContextAdapter context)
            : base(context)
        {
        }

        // These aren't changed by the authorization server so load once
        private static IEnumerable<Client> _clients; 
        private IEnumerable<Client> Clients
        {
            get
            {                
                if (_clients != null) return _clients;

                _clients = Find(x => x.Scopes).ToList();

                return _clients;
            }
        }

        public override Client GetById(object id)
        {
            return Clients.FirstOrDefault(x => x.Id == (string)id);
        }
    }
}
