using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Common.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value, int index = 0)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null)
                return value.ToString();
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? GetByIndex(attributes[0].Description, index, '|') ?? value.ToString() : value.ToString();
        }

        public static string GetDisplayName(this Enum value, int index = 0)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null)
                return value.ToString();
            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            return attributes != null && attributes.Length > 0 ? GetByIndex(attributes[0].Name, index, '|') ?? value.ToString() : value.ToString();
        }

        private static string? GetByIndex(string? source, int index, char separator)
        {
            if (source == null)
                return null;
            var names = source.Split(separator);
            if (names.Length >= index)
                index = names.Length - 1;
            if (index < 0)
                index = 0;
            return names[index];
        }
    }
}
