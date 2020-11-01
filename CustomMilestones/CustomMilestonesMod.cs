using ColossalFramework.IO;
using CustomMilestones.Helpers;
using CustomMilestones.Models;
using CustomMilestones.TranslationFramework;
using ICities;

namespace CustomMilestones
{
    public class CustomMilestonesMod : IUserMod
    {
        public static readonly string modSettingFilePath = DataLocation.executableDirectory + "\\CustomMilestone-Setting.json";

        public string Name => Translations.Translate("ModName", "Custom Milestones") + " " + ModHelper.GetVersion();

        public string Description => Translations.Translate("ModDescription", "Custom Milestones");

        public void OnSettingsUI(UIHelperBase helper)
        {
            ModSettingModel modSetting = JsonHelper.FromJsonFile<ModSettingModel>(modSettingFilePath) ?? new ModSettingModel();
            UIHelperBase group = helper.AddGroup(Translations.Translate("ModName", "Custom Milestones"));

            group.AddCheckbox(Translations.Translate("Label_FreePurchaseArea", "Free purchase area"), modSetting.FreePurchaseArea, (isChecked) =>
            {
                modSetting.FreePurchaseArea = isChecked;
                JsonHelper.ToJsonFile(modSetting, modSettingFilePath);
            }); 
            
            group.AddCheckbox(Translations.Translate("Label_BuildNonDefaul", "Build game non-defaul assets"), modSetting.BuildAllAssets, (isChecked) =>
            {
                modSetting.BuildAllAssets = isChecked;
                JsonHelper.ToJsonFile(modSetting, modSettingFilePath);
            });
        }
    }
}
