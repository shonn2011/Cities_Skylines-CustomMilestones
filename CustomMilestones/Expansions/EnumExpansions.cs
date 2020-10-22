using CustomMilestones.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomMilestones.Expansions
{
    public static class EnumExpansions
    {
        public static bool TryToEnum<T>(this string enumName, out T t) where T :Enum
        {
            bool isError = false;
            try
            {
                isError = true;
                t = (T)Enum.Parse(typeof(T), enumName);
            }
            catch
            {
                t = default;
            }
            return isError;
        }
    }
}
