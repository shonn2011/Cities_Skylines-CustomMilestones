using ColossalFramework;
using CustomMilestones.Helpers;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            EnumHelper.TryToEnumData("PowerSaving", out PositionData<DistrictPolicies.Policies> policyEnum);
           var a= policyEnum.GetCategory();
        }
    }
}
