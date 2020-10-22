using System.Collections.Generic;
using System.Xml.Serialization;

namespace CustomMilestones.Models
{
    [XmlRoot(ElementName = "CustomMilestone")]
    public class CustomMilestoneModel
    {
        [XmlAttribute]
        public bool Rebuild { get; set; }

        [XmlElement("Milestone")]
        public MilestoneModel[] Milestones { get; set; }
    }

    public class MilestoneModel
    {
        [XmlAttribute]
        public uint Level { get; set; }

        [XmlAttribute]
        public string LocalizedName { get; set; }

        [XmlAttribute]
        public int? RewardCash { get; set; }

        [XmlAttribute]
        public int PurchaseAreasCount { get; set; }

        [XmlElement("Road")]
        public List<ItemModel> Roads = new List<ItemModel>();

        [XmlElement("Building")]
        public List<ItemModel> Buildings = new List<ItemModel>();

        [XmlElement("Service")]
        public List<ItemModel> Services = new List<ItemModel>();

        [XmlElement("Feature")]
        public List<ItemModel> Features = new List<ItemModel>();

        [XmlElement("Policy")]
        public List<ItemModel> Policies = new List<ItemModel>();

        [XmlElement("InfoView")]
        public List<ItemModel> InfoViews = new List<ItemModel>();
    }

    public class ItemModel
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string LocalizedName { get; set; }

        [XmlIgnore]
        public string Expansions { get; set; }
    }
}
