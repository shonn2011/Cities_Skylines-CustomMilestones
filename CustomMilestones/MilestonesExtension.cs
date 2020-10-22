using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.IO;
using CustomMilestones.Expansions;
using CustomMilestones.Helpers;
using CustomMilestones.Models;
using ICities;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomMilestones
{
    public class MilestonesExtension : MilestonesExtensionBase
    {
        private static readonly string _customMilestoneFilePath = DataLocation.executableDirectory + "\\CustomMilestone.xml";
        private static readonly string _modConfigVirtualFilePath = "Resources\\config.json";

        public override void OnCreated(IMilestones milestones)
        {
            base.OnCreated(milestones);
            CustomMilestoneModel customMilestone = XmlHelper.FromXmlFile<CustomMilestoneModel>(_customMilestoneFilePath);
            if (customMilestone == null || customMilestone.Rebuild)
            {
                customMilestone = new CustomMilestoneModel()
                {
                    Rebuild = true,
                    Milestones = new MilestoneModel[]
                        {
                        new MilestoneModel() { Level = 0, PurchaseAreasCount = 1 },
                        new MilestoneModel() { Level = 1 },
                        new MilestoneModel() { Level = 2 },
                        new MilestoneModel() { Level = 3 },
                        new MilestoneModel() { Level = 4 },
                        new MilestoneModel() { Level = 5 },
                        new MilestoneModel() { Level = 6 },
                        new MilestoneModel() { Level = 7 },
                        new MilestoneModel() { Level = 8 },
                        new MilestoneModel() { Level = 9 },
                        new MilestoneModel() { Level = 10 },
                        new MilestoneModel() { Level = 11 },
                        new MilestoneModel() { Level = 12 },
                        new MilestoneModel() { Level = 13 }
                        }
                };
                foreach (var milestone in Singleton<UnlockManager>.instance.m_properties.m_progressionMilestones)
                {
                    int purchaseAreasCount = Singleton<UnlockManager>.instance.m_properties.m_AreaMilestones.Count(m => m.GetLevel() == milestone.GetLevel());
                    customMilestone.Milestones[milestone.GetLevel()].LocalizedName = milestone.GetLocalizedName();
                    customMilestone.Milestones[milestone.GetLevel()].RewardCash = milestone.m_rewardCash;
                    customMilestone.Milestones[milestone.GetLevel()].PurchaseAreasCount = purchaseAreasCount;
                }
                XmlHelper.ToXmlFile(customMilestone, _customMilestoneFilePath);
            }
        }

        public override void OnRefreshMilestones()
        {
            milestonesManager.UnlockMilestone("Basic Road Created");
            RefreshMilestones();
        }

        public void RefreshMilestones()
        {
            ModConfigModel modConfig = JsonHelper.FromJsonFile<ModConfigModel>(Path.Combine(ModHelper.GetModPath(), _modConfigVirtualFilePath)) ?? new ModConfigModel();
            CustomMilestoneModel customMilestoneModel = XmlHelper.FromXmlFile<CustomMilestoneModel>(_customMilestoneFilePath);
            if (customMilestoneModel.Rebuild)
            {
                customMilestoneModel.Rebuild = false;

                for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
                {
                    NetInfo net = PrefabCollection<NetInfo>.GetLoaded(i);
                    if (modConfig.RoadIncludes.Contains(modConfig.Renames.GetRename(net.name)))
                    {
                        if (!customMilestoneModel.Exists(modConfig.Renames.GetRename(net.name)))
                        {
                            if (modConfig.BuildingExistsRoads.Contains(modConfig.Renames.GetRename(net.name)))
                            {
                                customMilestoneModel.Milestones[net.GetUnlockMilestone().GetLevel()].Buildings.Add(new ItemModel()
                                {
                                    Name = modConfig.Renames.GetRename(net.name),
                                    LocalizedName = net.GetLocalizedTitle(),
                                    Expansions = net.m_class.m_service.ToString() + "|" + net.m_class.m_subService.ToString() + "|" + net.category
                                });
                            }
                            else
                            {
                                customMilestoneModel.Milestones[net.GetUnlockMilestone().GetLevel()].Roads.Add(new ItemModel()
                                {
                                    Name = modConfig.Renames.GetRename(net.name),
                                    LocalizedName = net.GetLocalizedTitle(),
                                    Expansions = net.m_class.m_service.ToString() + "|" + net.m_class.m_subService.ToString() + "|" + net.category
                                });
                            }
                        }
                    }
                }

                for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
                {
                    BuildingInfo building = PrefabCollection<BuildingInfo>.GetLoaded(i);
                    if (modConfig.BuildingIncludes.Contains(modConfig.Renames.GetRename(building.name)))
                    {
                        if (!customMilestoneModel.Exists(modConfig.Renames.GetRename(building.name)))
                        {
                            if (modConfig.RoadExistsBuildings.Contains(modConfig.Renames.GetRename(building.name)))
                            {
                                customMilestoneModel.Milestones[building.GetUnlockMilestone().GetLevel()].Roads.Add(new ItemModel()
                                {
                                    Name = modConfig.Renames.GetRename(building.name),
                                    LocalizedName = building.GetLocalizedTitle(),
                                    Expansions = building.m_class.m_service.ToString() + "|" + building.m_class.m_subService.ToString() + "|" + building.category
                                });
                            }
                            else
                            {
                                customMilestoneModel.Milestones[building.GetUnlockMilestone().GetLevel()].Buildings.Add(new ItemModel()
                                {
                                    Name = modConfig.Renames.GetRename(building.name),
                                    LocalizedName = building.GetLocalizedTitle(),
                                    Expansions = building.m_class.m_service.ToString() + "|" + building.m_class.m_subService.ToString() + "|" + building.category
                                });
                            }
                        }
                    }
                }

                foreach (var feature in Utils.GetOrderedEnumData<UnlockManager.Feature>())
                {
                    if (modConfig.Features.Contains(feature.enumName))
                    {
                        if (!customMilestoneModel.Exists(feature.enumName, "Feature"))
                        {
                            var level = Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)feature.enumValue].GetLevel();
                            if (modConfig.ServiceExistsFeatures.Contains(feature.enumName))
                            {
                                customMilestoneModel.Milestones[level].Services.Add(new ItemModel()
                                {
                                    Name = feature.enumName,
                                    LocalizedName = feature.GetLocalizedName(),
                                });
                            }
                            else
                            {
                                customMilestoneModel.Milestones[level].Features.Add(new ItemModel()
                                {
                                    Name = feature.enumName,
                                    LocalizedName = feature.GetLocalizedName(),
                                });
                            }
                        }
                    }
                }

                foreach (var service in Utils.GetOrderedEnumData<ItemClass.Service>())
                {
                    if (modConfig.Services.Contains(service.enumName))
                    {
                        if (!customMilestoneModel.Exists(service.enumName, "Service"))
                        {
                            var level = Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)service.enumValue].GetLevel();

                            customMilestoneModel.Milestones[level].Services.Add(new ItemModel()
                            {
                                Name = service.enumName,
                                LocalizedName = service.GetLocalizedName(),
                            });
                        }
                    }
                }

                //foreach (var infoMode in Utils.GetOrderedEnumData<InfoManager.InfoMode>())
                //{
                //    var level = Singleton<UnlockManager>.instance.m_properties.m_InfoModeMilestones[(int)infoMode.enumValue].GetLevel();

                //    customMilestoneModel.Milestones[level].InfoViews.Add(new ItemModel()
                //    {
                //        Name = infoMode.enumName,
                //        LocalizedName = Locale.Get("INFOVIEWS", infoMode.enumName)
                //    });
                //}

                XmlHelper.ToXmlFile(customMilestoneModel, _customMilestoneFilePath);
            }
            else
            {
                MilestoneInfo[] progressionMilestones = Singleton<UnlockManager>.instance.m_properties.m_progressionMilestones;
                foreach (MilestoneModel milestoneModel in customMilestoneModel.Milestones)
                {
                    MilestoneInfo milestoneInfo = milestoneModel.Level > 0 ? progressionMilestones[milestoneModel.Level - 1] : null;
                    var count = (milestoneModel.Level == 0 && milestoneModel.PurchaseAreasCount == 0) ? 1 : milestoneModel.PurchaseAreasCount;
                    var total = customMilestoneModel.Milestones.Take(Array.IndexOf(customMilestoneModel.Milestones, milestoneModel)).Sum(m => m.PurchaseAreasCount);
                    for (int i = total; i < total + count && i < 9; i++)
                    {
                        Singleton<UnlockManager>.instance.m_properties.m_AreaMilestones[i] = milestoneInfo;
                    }
                    if (milestoneInfo != null)
                    {
                        milestoneInfo.m_rewardCash = milestoneModel.RewardCash.Value;
                    }

                    foreach (var roadModel in milestoneModel.Roads)
                    {
                        if (modConfig.RoadIncludes.Contains(roadModel.Name) || modConfig.RoadExistsBuildings.Contains(roadModel.Name))
                        {
                            RefreshRoadMilestone(roadModel.Name, milestoneInfo, modConfig);
                        }
                    }

                    foreach (var buildingModel in milestoneModel.Buildings)
                    {
                        if (modConfig.BuildingIncludes.Contains(buildingModel.Name) || modConfig.BuildingExistsRoads.Contains(buildingModel.Name))
                        {
                            RefreshBuildingMilestone(buildingModel.Name, milestoneInfo, modConfig);
                        }
                    }

                    foreach (var featureModel in milestoneModel.Features)
                    {
                        if (modConfig.Features.Contains(featureModel.Name))
                        {
                            RefreshFeatureMilestones(featureModel.Name, milestoneInfo, modConfig);
                        }
                    }

                    foreach (var service in milestoneModel.Services)
                    {
                        if (modConfig.Services.Contains(service.Name) || modConfig.ServiceExistsFeatures.Contains(service.Name))
                        {
                            RefreshServiceMilestones(service.Name, milestoneInfo, modConfig);
                        }
                    }
                }
            }
        }

        private void RefreshRoadMilestone(string name, MilestoneInfo milestoneInfo, ModConfigModel modConfig, bool inGroup = false)
        {
            if (modConfig.RoadExistsBuildings.Contains(name))
            {
                RefreshBuildingMilestone(name, milestoneInfo, modConfig);
            }
            else if (modConfig.RoadGroups.TryGetValue(name, out List<string> roadGroup) && !inGroup)
            {
                foreach (string roadName in roadGroup)
                {
                    RefreshRoadMilestone(roadName, milestoneInfo, modConfig, true);
                }
            }
            else if (PrefabCollection<NetInfo>.LoadedExists(name))
            {
                NetInfo net = PrefabCollection<NetInfo>.FindLoaded(name);
                net.m_UnlockMilestone = CompareLowest(milestoneInfo, net.GetUnlockMilestone());
                RefreshRelatedMilestone(name, net.category, net.m_class.m_service, net.m_class.m_subService, net.m_UnlockMilestone);
            }
        }

        private void RefreshBuildingMilestone(string name, MilestoneInfo milestoneInfo, ModConfigModel modConfig, bool inGroup = false)
        {
            if (modConfig.BuildingExistsRoads.Contains(name) || modConfig.BuildingContainedRoads.Contains(name))
            {
                RefreshRoadMilestone(name, milestoneInfo, modConfig);
            }
            else if (modConfig.BuildingGroups.TryGetValue(name, out List<string> buildingGroup) && !inGroup)
            {
                foreach (string buildingName in buildingGroup)
                {
                    RefreshBuildingMilestone(buildingName, milestoneInfo, modConfig, true);
                }
            }
            else if (PrefabCollection<BuildingInfo>.LoadedExists(name))
            {
                BuildingInfo building = PrefabCollection<BuildingInfo>.FindLoaded(name);
                building.m_UnlockMilestone = CompareLowest(milestoneInfo, building.GetUnlockMilestone());
                RefreshRelatedMilestone(name, building.category, building.m_class.m_service, building.m_class.m_subService, milestoneInfo);
            }
        }

        private void RefreshRelatedMilestone(string name, string category, ItemClass.Service serviceRelated, ItemClass.SubService subServiceRelated, MilestoneInfo milestoneInfo)
        {
            switch (category)
            {
                case "LandscapingPaths":
                case "LandscapingWaterStructures":
                    RefreshFeatureMilestones(UnlockManager.Feature.Landscaping.ToString(), milestoneInfo);
                    break;
                case "IndustryFishing":
                    RefreshServiceMilestones(ItemClass.Service.Garbage.ToString(), milestoneInfo);
                    RefreshFeatureMilestones(UnlockManager.Feature.Fishing.ToString(), milestoneInfo);
                    break;
                case "IndustryWarehouses":
                    RefreshServiceMilestones(ItemClass.Service.Garbage.ToString(), milestoneInfo);
                    RefreshFeatureMilestones(UnlockManager.Feature.IndustryAreas.ToString(), milestoneInfo);
                    break;
                case "MonumentFootball":
                    RefreshFeatureMilestones(UnlockManager.Feature.Football.ToString(), milestoneInfo);
                    break;
                case "MonumentConcerts":
                    RefreshFeatureMilestones(UnlockManager.Feature.Concerts.ToString(), milestoneInfo);
                    break;
                case "RoadsIntersection":
                    BuildingInfo building = PrefabCollection<BuildingInfo>.FindLoaded(name);
                    IntersectionAI intersectionAI = building.m_buildingAI as IntersectionAI;
                    SetPrivateVariable(intersectionAI, "m_cachedUnlockMilestone", milestoneInfo);
                    break;
                case "FireDepartmentDisaster":
                    RefreshServiceMilestones(ItemClass.Service.FireDepartment.ToString(), milestoneInfo);
                    RefreshFeatureMilestones(UnlockManager.Feature.DisasterResponse.ToString(), milestoneInfo);
                    break;
                case "PublicTransportShip":
                    RefreshFeatureMilestones(UnlockManager.Feature.Ferry.ToString(), milestoneInfo);
                    break;
                case "PublicTransportPlane":
                    if (name.Contains("Blimp"))
                    {
                        RefreshFeatureMilestones(UnlockManager.Feature.Blimp.ToString(), milestoneInfo);
                    }
                    else if (name.Contains("Helicopter"))
                    {
                        RefreshFeatureMilestones(UnlockManager.Feature.Helicopter.ToString(), milestoneInfo);
                    }
                    break;
                case "PublicTransportTrolleybus":
                    RefreshFeatureMilestones(UnlockManager.Feature.Trolleybus.ToString(), milestoneInfo);
                    break;
                case "BeautificationExpansion1":
                    RefreshFeatureMilestones(UnlockManager.Feature.ParkAreas.ToString(), milestoneInfo);
                    break;
            }

            RefreshServiceMilestones(serviceRelated.ToString(), milestoneInfo);

            RefreshSubServiceMilestones(subServiceRelated, milestoneInfo);
        }

        private void RefreshFeatureMilestones(string name, MilestoneInfo milestoneInfo, ModConfigModel modConfig = null, bool inGroup = false)
        {
            if (modConfig != null && modConfig.FeatureGroups.TryGetValue(name, out List<string> featureGroup) && !inGroup)
            {
                foreach (string featureName in featureGroup)
                {
                    RefreshFeatureMilestones(featureName, milestoneInfo, modConfig, true);
                }
            }
            else if (name.TryToEnum(out UnlockManager.Feature feature) && milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)feature].GetLevel())
            {
                Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)feature] = milestoneInfo;
            }
        }

        private void RefreshServiceMilestones(string name, MilestoneInfo milestoneInfo, ModConfigModel modConfig = null, bool inGroup = false)
        {
            if (modConfig.ServiceExistsFeatures.Contains(name))
            {
                RefreshFeatureMilestones(name, milestoneInfo);
            }
            else if (name.TryToEnum(out ItemClass.Service service))
            {
                if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)service].GetLevel())
                {
                    Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)service] = milestoneInfo;
                }
            }
        }

        private void RefreshSubServiceMilestones(ItemClass.SubService subService, MilestoneInfo milestoneInfo)
        {
            if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_SubServiceMilestones[(int)subService].GetLevel())
            {
                Singleton<UnlockManager>.instance.m_properties.m_SubServiceMilestones[(int)subService] = milestoneInfo;
            }
        }

        private void SetPrivateVariable<T>(object obj, string fieldName, T value)
        {
            obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
        }

        private MilestoneInfo CompareLowest(MilestoneInfo current, MilestoneInfo original)
        {
            return current.GetLevel() < original.GetLevel() ? current : original;
        }
    }
}
