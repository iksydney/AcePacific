using System.ComponentModel;
using System.Reflection;

namespace AcePacific.Common.EnumHelper
{
    public class EnumHelper
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            try
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attributes.Length > 0 ? attributes[0].Description : value.ToString();
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

    }
}
