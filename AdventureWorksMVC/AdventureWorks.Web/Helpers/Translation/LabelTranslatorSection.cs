using System.Configuration;

public class LabelTranslatorSection : ConfigurationSection
{
    private const string LABELS = "labels";

    [ConfigurationProperty(LABELS, IsDefaultCollection = true)]
    public LabelElementCollection Labels
    {
        get { return (LabelElementCollection)this[LABELS]; }
        set { this[LABELS] = value; }
    }
}