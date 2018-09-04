using System.Web.Http;

namespace AdventureWorks.Web
{
    public class XmlSerializerConfig
    {
        public static void CustomizeConfig(HttpConfiguration config)
        {
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
