using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CustomMilestones.Helpers
{
    public static class EnumHelper
    {
        public static bool TryToEnumData<T>(string enumName, out PositionData<T> position) where T : struct, IConvertible
        {
            try
            {
                position = Utils.GetOrderedEnumData<T>().First(m => m.enumName == enumName);
                return true;
            }
            catch
            {
                position = default;
                return false;
            }
        }

        public static string GetCategory<T>(this PositionData<T> position) where T : struct, IConvertible
        {
            Type enumType = position.enumValue.GetType();
            string name = Enum.GetName(enumType, position.enumValue);
            if (name != null)
            {
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    if (Attribute.GetCustomAttribute(fieldInfo, typeof(EnumPositionAttribute), false) is EnumPositionAttribute enumPosition)
                    {
                        return enumPosition.category;
                    }
                }
            }
            return null;
        }
    }
}
