using System;
using System.Configuration;

namespace Client2.Config
{
    // Grab settings relating to the authorization server for the client to use
    public static class AuthorizationServerConfig
    {
        public static Uri AuthorizationEndpoint = new Uri(ConfigurationManager.AppSettings["AuthorizationServerAuthorizeUrl"]);
        public static Uri TokenEndpoint = new Uri(ConfigurationManager.AppSettings["AuthorizationServerTokenUrl"]);
    }
}