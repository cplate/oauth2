using System.Configuration;

namespace Client1.Config
{
    // Grab settings relating to the resource api locations
    public class ResourceConfig
    {
        public static string Resource1BaseUrl { get { return ConfigurationManager.AppSettings["Resource1BaseUrl"]; } }
        public static string Resource2BaseUrl { get { return ConfigurationManager.AppSettings["Resource2BaseUrl"]; } }
        
    }
}