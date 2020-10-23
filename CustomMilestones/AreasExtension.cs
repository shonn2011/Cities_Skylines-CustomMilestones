using CustomMilestones.Helpers;
using CustomMilestones.Models;
using ICities;

namespace CustomMilestones
{
    public class AreasExtension : AreasExtensionBase
    {
        public override void OnCreated(IAreas areas)
        {
            areas.maxAreaCount = 25;
        }

        public override int OnGetAreaPrice(uint ore, uint oil, uint forest, uint fertility, uint water, bool road, bool train, bool ship, bool plane, float landFlatness, int originalPrice)
        {
            ModSettingModel modSetting = JsonHelper.FromJsonFile<ModSettingModel>(CustomMilestonesMod.modSettingFilePath) ?? new ModSettingModel();
            return modSetting.FreePurchaseArea ? 0 : originalPrice;
        }
    }
}
