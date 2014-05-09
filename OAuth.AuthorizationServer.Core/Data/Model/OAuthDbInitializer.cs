using System;
using System.Collections.Generic;
using System.Data.Entity;
using OAuth.AuthorizationServer.Core.Data.Extensions;

namespace OAuth.AuthorizationServer.Core.Data.Model
{
    // Using this to seed our database with data to support the sample
    public class OAuthDbInitializer : DropCreateDatabaseIfModelChanges<OAuthDataContext>
    {
        protected override void Seed(OAuthDataContext context)
        {
            base.Seed(context);

            var r1read = new Scope {Description = "Read info from Resource1", Identifier = "Resource1-Read"};
            var r1write = new Scope {Description = "Write info to Resource1", Identifier = "Resource1-Write"};
            var r2read = new Scope {Description = "Read info from Resource2", Identifier = "Resource2-Read"};
            var r2write = new Scope {Description = "Write info to Resource2", Identifier = "Resource2-Write"};
            context.Scopes.Add(r1read);
            context.Scopes.Add(r1write);
            context.Scopes.Add(r2read);
            context.Scopes.Add(r2write);

            var resource1 = new Resource { Identifier = "Resource1", AuthenticationTokenName = "resource-authentication-token", AuthenticationUrl = "http://localhost:60404/Account/Login", AuthenticationKey = "WebAPIsAreAwesome", PublicTokenEncryptionKey = "PFJTQUtleVZhbHVlPjxNb2R1bHVzPjZlZnpYTHNvSmQ5OVVQMjdOQ1hWSnpZSFVtMmlLTUVOSlo2eVpVSFNvTE5uYnFMdENZWjZXaEl3SXY5WVBGeit0Z1AvQWN0dXh3N2VWVzdsU0RaU0IvQU8ySk1kOXJ1MkJ4SHRVaUZVd2pySnNHTlRUS1NhVDFDOTA0YXFaTExaRUxmYnVvVllabnFKWkRvdjlreTkwaHZtTTYwY3FXbkU4TGc2aGZSYlFPOD08L01vZHVsdXM+PEV4cG9uZW50PkFRQUI8L0V4cG9uZW50PjwvUlNBS2V5VmFsdWU+",
                Scopes = new List<Scope> { r1read, r1write }};
            var resource2 = new Resource { Identifier = "Resource2", AuthenticationTokenName = "resource-authentication-token", AuthenticationUrl = "http://localhost:60404/Account/Login", AuthenticationKey = "WebAPIsAreAwesome", PublicTokenEncryptionKey = "PFJTQUtleVZhbHVlPjxNb2R1bHVzPjVFOEpmc2FScnNZdVpKVHNhTlJIcENsb3NvR0N1OFZtOCtuN0w5YlpGYzkyb3pYS1N0UFRtVWdCT0RUSlYzNzFDMU9hKzBBbyswdHMrQi9JYnR1RUdTeVNKWVh5U2FiKzdHYzRGNDUrSDJKVUFlOXpJcmJmVVFKdVprT25WRk1ISUJMY2RWaFJDNUd0L2JwWVdXeHk4RHcvSEV3WTdHb0Y1RHE0dDFabE9oYz08L01vZHVsdXM+PEV4cG9uZW50PkFRQUI8L0V4cG9uZW50PjwvUlNBS2V5VmFsdWU+",
                Scopes = new List<Scope> { r2read, r2write }};
            context.Resources.Add(resource1);
            context.Resources.Add(resource2);

            var client1 = new Client {Id = "client1", ClientSecret = "clientSecret1", Name = "Awesome Client 1", 
                Scopes = new List<Scope> { r1read, r1write, r2read, r2write }};
            var client2 = new Client {Id = "client2", ClientSecret = "clientSecret2", Name = "More Awesome Client 2", 
                Scopes = new List<Scope> { r1read, r1write }};
            context.Clients.Add(client1);
            context.Clients.Add(client2);

            var cryptoKey1 = new SymmetricCryptoKey { Bucket = "https://localhost/dnoa/oauth_authorization_code", ExpiresUtc = DateTime.Now.AddYears(1).AsUtc(), Handle = "2oBU", Secret = StringToByteArray("7E84C6E42D6CE5E18D1E99F8C3DAD40566411CC0FFE77F9D90CD263F5BA538CB") };
            var cryptoKey2 = new SymmetricCryptoKey { Bucket = "https://localhost/dnoa/oauth_refresh_token", ExpiresUtc = DateTime.Now.AddYears(1).AsUtc(), Handle = "wxFx", Secret = StringToByteArray("FE251484D381BAEA12A2DBEC028B5885F26E3F2B503BF28E1B404D31117AD332") };
            context.SymmetricCryptoKeys.Add(cryptoKey1);
            context.SymmetricCryptoKeys.Add(cryptoKey2);

            context.SaveChanges();            
        }

        private byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
