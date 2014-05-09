using System.Configuration;
using DotNetOpenAuth.OAuth2;

namespace Client1.Config
{
    // Grab settings relating to the client app
    public static class ClientConfig
    {
        public static string ClientId { get { return ConfigurationManager.AppSettings["ClientId"]; } }
        public static string ClientSecret { get { return ConfigurationManager.AppSettings["ClientSecret"]; } }
        public static string ClientCallbackUrl { get { return ConfigurationManager.AppSettings["ClientCallbackUrl"]; } }
        public static string AccessTokenName { get { return ConfigurationManager.AppSettings["AccessTokenName"]; } }

        // This needs to be static for the app for the code flow to ensure state is matched properly, i.e.,
        // When our code flow callback is called we ensure the state in the request matches the state of a request we actually made
        public static WebServerClient AuthorizationServerClient
        {
            get
            {
                var serverInfo = new AuthorizationServerDescription
                {
                    AuthorizationEndpoint = AuthorizationServerConfig.AuthorizationEndpoint,
                    TokenEndpoint = AuthorizationServerConfig.TokenEndpoint,
                    ProtocolVersion = ProtocolVersion.V20
                };
                var client = new WebServerClient(serverInfo, clientIdentifier: ClientId);
                client.ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ClientSecret);
                return client;
            }
        }
    }

}