using ColossalFramework;
using System;
using System.Linq;

namespace CustomMilestones.Expansions
{
    public static class EnumExpansions
    {
        public static bool TryToEnumData<T>(this string enumName, out PositionData<T> position) where T : struct, IConvertible
        {
            try
            {
                position = Utils.GetOrderedEnumData<T>().First(m => m.enumName == enumName);
                return false;
            }
            catch
            {
                position = default;
                return true;
            }
        }
    }
}
