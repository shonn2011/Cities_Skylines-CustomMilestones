using System;

namespace CustomMilestones.Models
{
    [Serializable]
    public class ModSettingModel
    {
        public ModSettingModel()
        {
            FreePurchaseArea = true;
        }

        public bool FreePurchaseArea { get; set; }
    }
}
