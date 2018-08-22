using System.Configuration;
using System.Linq;

[ConfigurationCollection(typeof(LabelElement))]
public class LabelElementCollection : ConfigurationElementCollection
{
    protected override ConfigurationElement CreateNewElement()
    {
        return new LabelElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
        return ((LabelElement)element).Label;
    }

    public LabelElement this[string key]
    {
        get
        {
            return this.OfType<LabelElement>().FirstOrDefault(item => item.Label == key);
        }
    }
}