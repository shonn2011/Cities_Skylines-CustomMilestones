using System;
using System.Collections.Generic;

namespace CustomMilestones.Models
{
    [Serializable]
    public class ModConfigModel
    {
        public ModConfigModel()
        {
            RoadIncludes = new List<string>();
            RoadExistsBuildings = new List<string>();
            RoadGroups = new Dictionary<string, List<string>>();
            BuildingIncludes = new List<string>();
            BuildingExistsRoads = new List<string>();
            BuildingContainedRoads = new List<string>();
            BuildingGroups = new Dictionary<string, List<string>>();
            Renames = new Dictionary<string, string>();
            Features = new List<string>();
            FeatureGroups = new Dictionary<string, List<string>>();
            Services = new List<string>();
            ServiceExistsFeatures = new List<string>();
        }

        public List<string> RoadIncludes { get; set; }

        public List<string> RoadExistsBuildings { get; set; }

        public Dictionary<string, List<string>> RoadGroups { get; set; }

        public List<string> BuildingIncludes { get; set; }

        public List<string> BuildingExistsRoads { get; set; }

        public List<string> BuildingContainedRoads { get; set; }

        public Dictionary<string, List<string>> BuildingGroups { get; set; }

        public Dictionary<string, string> Renames { get; set; }

        public List<string> Features { get; set; }

        public Dictionary<string, List<string>> FeatureGroups { get; set; }

        public List<string> Services { get; set; }

        public List<string> ServiceExistsFeatures { get; set; }
    }
}
