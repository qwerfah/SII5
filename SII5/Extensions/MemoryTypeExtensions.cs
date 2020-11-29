using SII5.Models;
using System.ComponentModel;

namespace SII5.Extensions
{
    public static class MemoryTypeExtensions
    {
        private static string GetCustomDescription(object objEnum)
        {
            var fi = objEnum.GetType().GetField(objEnum.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : objEnum.ToString();
        }

        public static string Description(this MemoryType value)
        {
            return GetCustomDescription(value);
        }
    }
}
