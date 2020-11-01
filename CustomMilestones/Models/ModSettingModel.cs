using System;

namespace CustomMilestones.Models
{
    [Serializable]
    public class ModSettingModel
    {
        public ModSettingModel()
        {
            FreePurchaseArea = true;
            BuildAllAssets = false;
        }

        public bool FreePurchaseArea { get; set; }

        public bool BuildAllAssets { get; set; }
    }
}
