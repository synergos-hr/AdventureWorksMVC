using System.Configuration;

public static class Translator
{
    private readonly static LabelTranslatorSection config = ConfigurationManager.GetSection("labelTranslations") as LabelTranslatorSection;

    public static string Translate(string label)
    {
        return config.Labels[label] != null ? config.Labels[label].TranslateTo : label;
    }
}