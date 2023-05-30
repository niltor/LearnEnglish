using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace EnglishCopilot.Converter;
class PickerLocaleDisplayConverter : BaseConverterOneWay<Locale, string>
{
    public override string DefaultConvertReturnValue { get; set; } = string.Empty;

    public override string ConvertFrom(Locale value, CultureInfo? culture)
    {
        return $"{value.Language} {value.Name}";
    }
}