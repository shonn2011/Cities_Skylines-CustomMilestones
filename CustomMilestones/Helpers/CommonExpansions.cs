using CustomMilestones.Models;
using System.Collections.Generic;
using System.Linq;

namespace CustomMilestones.Helpers
{
    public static class CommonExpansions
    {
        public static bool Exists(this CustomMilestoneModel customMilestone, string name, string category = "")
        {
            switch (category)
            {
                case "Feature":
                    return customMilestone.Milestones.Any(m => m.Features.Any(n => n.Name == name));
                case "Service":
                    return customMilestone.Milestones.Any(m => m.Services.Any(n => n.Name == name));
                case "Policy":
                    return customMilestone.Milestones.Any(m => m.Policies.Any(n => n.Name == name));
                case "InfoView":
                    return customMilestone.Milestones.Any(m => m.Policies.Any(n => n.Name == name));
                default:
                    return customMilestone.Milestones.Any(m => m.Roads.Any(n => n.Name == name)) || customMilestone.Milestones.Any(m => m.Buildings.Any(n => n.Name == name));
            }
        }

        public static uint GetLevel(this MilestoneInfo milestone)
        {
            if (milestone != null)
            {
                switch (milestone.GetComparisonValue())
                {
                    case 550:
                        return 1;
                    case 1100:
                        return 2;
                    case 1700:
                        return 3;
                    case 3000:
                        return 4;
                    case 6000:
                        return 5;
                    case 9000:
                        return 6;
                    case 12500:
                        return 7;
                    case 20000:
                        return 8;
                    case 25000:
                        return 9;
                    case 40000:
                        return 10;
                    case 55000:
                        return 11;
                    case 80000:
                        return 12;
                    case 100000:
                        return 13;
                    default:
                        return 0;
                }
            }
            return 0;
        }

        public static string GetRename(this Dictionary<string, string> keyValuePairs, string name)
        {
            if (!keyValuePairs.TryGetValue(name, out string rename))
            {
                rename = name;
            }
            return rename;
        }
    }
}
