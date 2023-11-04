using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AcePacific.Common.Extensions
{
    public static class EnumExtensions
    {
        public const int OK = 200;

        public static string GetDescription(this Enum value)
        {
            try
            {
                if (value is null)
                    return null;

                Type type = value.GetType();
                string name = Enum.GetName(type, value);
                if (name != null)
                {
                    FieldInfo field = type.GetField(name);
                    if (field != null)
                    {
                        DescriptionAttribute attr =
                            Attribute.GetCustomAttribute(field,
                                typeof(DescriptionAttribute)) as DescriptionAttribute;

                        if (attr != null)
                        {
                            return attr.Description;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {

                return "N/A";
            }
        }

        public static bool IsValidEnumValue(this System.Enum value)
        {
            if (value.HasFlags())
                return IsFlagsEnumDefined(value);
            else
                return Enum.IsDefined(value.GetType(), value);
        }

        private static bool IsFlagsEnumDefined(System.Enum value)
        {
            // modeled after Enum's InternalFlagsFormat
            Type underlyingenumtype = Enum.GetUnderlyingType(value.GetType());
            switch (Type.GetTypeCode(underlyingenumtype))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                    {
                        object obj = Activator.CreateInstance(underlyingenumtype);
                        long svalue = System.Convert.ToInt64(value);
                        if (svalue < 0)
                            throw new ArgumentException(
                                string.Format("Can't process negative {0} as {1} enum with flags", svalue, value.GetType().Name));
                    }
                    break;
                default:
                    break;
            }

            ulong flagsset = System.Convert.ToUInt64(value);
            Array values = Enum.GetValues(value.GetType());//.Cast<ulong />().ToArray<ulong />();
            int flagno = values.Length - 1;
            ulong initialflags = flagsset;
            ulong flag = 0;
            //start with the highest values
            while (flagno >= 0)
            {
                flag = System.Convert.ToUInt64(values.GetValue(flagno));
                if ((flagno == 0) && (flag == 0))
                {
                    break;
                }
                //if the flags set contain this flag
                if ((flagsset & flag) == flag)
                {
                    //unset this flag
                    flagsset -= flag;
                    if (flagsset == 0)
                        return true;
                }
                flagno--;
            }
            if (flagsset != 0)
            {
                return false;
            }
            if (initialflags != 0 || flag == 0)
            {
                return true;
            }
            return false;
        }

        public static bool HasFlags(this System.Enum value)
        {
            return value.GetType().GetCustomAttributes(typeof(System.FlagsAttribute),
                false).Length > 0;
        }

        public static bool SafeConvertToEnum<EnumType>(this object value, out EnumType retv) where EnumType : struct
        {
            Type enumType = typeof(EnumType);
            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format("{0} is not an Enum.",
                    enumType.Name));
            if (value == null)
            {
                retv = default(EnumType);
                return false;
            }
            Type valType = value.GetType();
            bool isString = valType == typeof(string);
            bool isOrdinal = valType.IsPrimitive ||
                typeof(decimal) == valType || valType.IsEnum;
            if (!isOrdinal && !isString)
                throw new System.ArgumentException
                    (string.Format("{0} can not be converted to an enum", valType.Name));

            try
            {
                checked
                {
                    if (valType == Enum.GetUnderlyingType(enumType))
                        retv = (EnumType)value;
                    else
                    {
                        if (isString)
                            retv = (EnumType)Enum.Parse(typeof(EnumType), value as string);
                        else
                            if (valType.IsEnum)
                        {
                            Enum en = (Enum)value;
                            object zero = Activator.CreateInstance(valType);
                            value = (en.CompareTo(zero) >= 0) ?
                            Convert.ToUInt64(value) : Convert.ToUInt64(value);
                        }
                        retv = (EnumType)Enum.Parse(typeof(EnumType), value.ToString());
                    }
                }
                if (!((System.Enum)(object)retv).IsValidEnumValue())
                {

                    retv = default(EnumType);
                    return false;
                }
            }
            catch (ArgumentException)
            {
                retv = default;
                return false;
            }
            catch (OverflowException)
            {
                retv = default;
                return false;
            }
            catch (InvalidCastException)
            {
                retv = default(EnumType);
                return false;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format
                    (@"Can't convert value {0}\nfrom the type of {1} into the underlying enum type of {2}\nbecause {3}",
                            value, valType.Name,
                            Enum.GetUnderlyingType(enumType).Name, ex.Message), ex);
            }
            return true;
        }

        //public static CustomKeyValuePair[] GetItems(this Type enumType)
        //{ 
        //    return enumType.GetItems();
        //}
    }
}
