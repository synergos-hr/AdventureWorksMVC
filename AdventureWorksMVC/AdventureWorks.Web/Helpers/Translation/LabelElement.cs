using System.Configuration;

public class LabelElement : ConfigurationElement
{
    private const string LABEL = "label";
    private const string TRANSLATE_TO = "translateTo";

    [ConfigurationProperty(LABEL, IsKey = true, IsRequired = true)]
    public string Label
    {
        get { return (string)this[LABEL]; }
        set { this[LABEL] = value; }
    }

    [ConfigurationProperty(TRANSLATE_TO, IsRequired = true)]
    public string TranslateTo
    {
        get { return (string)this[TRANSLATE_TO]; }
        set { this[TRANSLATE_TO] = value; }
    }
}
