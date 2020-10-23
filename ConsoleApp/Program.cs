using ColossalFramework;
using CustomMilestones.Expansions;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //var a = (UnlockManager.Feature)Enum.Parse(typeof(UnlockManager.Feature), "sdasd");
            "MonumentLevel5".TryToEnumData(out PositionData<UnlockManager.Feature> position);

        }
    }
}
