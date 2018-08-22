using System.Web.Http;

namespace AdventureWorksMVC
{
    public class XmlSerializerConfig
    {
        public static void CustomizeConfig(HttpConfiguration config)
        {
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
