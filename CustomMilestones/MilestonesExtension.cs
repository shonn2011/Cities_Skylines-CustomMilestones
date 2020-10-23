using ColossalFramework;
using ColossalFramework.IO;
using CustomMilestones.Expansions;
using CustomMilestones.Helpers;
using CustomMilestones.Models;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

                foreach (var featureEnum in Utils.GetOrderedEnumData<UnlockManager.Feature>())
                {
                    if (modConfig.Features.Contains(featureEnum.enumName))
                    {
                        if (!customMilestoneModel.Exists(featureEnum.enumName, "Feature"))
                        {
                            var level = Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)featureEnum.enumValue].GetLevel();
                            if (modConfig.ServiceExistsFeatures.Contains(featureEnum.enumName))
                            {
                                customMilestoneModel.Milestones[level].Services.Add(new ItemModel()
                                {
                                    Name = featureEnum.enumName,
                                    LocalizedName = featureEnum.GetLocalizedName(),
                                });
                            }
                            else
                            {
                                customMilestoneModel.Milestones[level].Features.Add(new ItemModel()
                                {
                                    Name = featureEnum.enumName,
                                    LocalizedName = featureEnum.GetLocalizedName(),
                                });
                            }
                        }
                    }
                }

                foreach (var serviceEnum in Utils.GetOrderedEnumData<ItemClass.Service>())
                {
                    if (modConfig.Services.Contains(serviceEnum.enumName))
                    {
                        if (!customMilestoneModel.Exists(serviceEnum.enumName, "Service"))
                        {
                            var level = Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)serviceEnum.enumValue].GetLevel();
                            customMilestoneModel.Milestones[level].Services.Add(new ItemModel()
                            {
                                Name = serviceEnum.enumName,
                                LocalizedName = serviceEnum.GetLocalizedName(),
                            });
                        }
                    }
                }

                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Industry"))
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_SpecializationMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                    customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Residential"))
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_SpecializationMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                    customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Office"))
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_SpecializationMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                    customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Commercial"))
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_SpecializationMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                    customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Services"))
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_ServicePolicyMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                    customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Taxation"))
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_TaxationPolicyMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                    customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("CityPlanning"))
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_CityPlanningPolicyMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                    customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Special"))
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_SpecialPolicyMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                    customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                if (managers.application.SupportsExpansion(Expansion.Parks))
                {
                    foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Park"))
                    {
                        var level = Singleton<UnlockManager>.instance.m_properties.m_ParkPolicyMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                        customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                        {
                            Name = policyEnum.enumName,
                            LocalizedName = policyEnum.GetLocalizedName(),
                            Expansions = policyEnum.enumCategory
                        });
                    }
                }

                if (managers.application.SupportsExpansion(Expansion.Industry))
                {
                    foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("IndustryArea"))
                    {
                        var level = Singleton<UnlockManager>.instance.m_properties.m_ParkPolicyMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                        customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                        {
                            Name = policyEnum.enumName,
                            LocalizedName = policyEnum.GetLocalizedName(),
                            Expansions = policyEnum.enumCategory
                        });
                    }
                }

                if (managers.application.SupportsExpansion(Expansion.Campuses))
                {
                    foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("CampusArea"))
                    {
                        var level = Singleton<UnlockManager>.instance.m_properties.m_ParkPolicyMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                        customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                        {
                            Name = policyEnum.enumName,
                            LocalizedName = policyEnum.GetLocalizedName(),
                            Expansions = policyEnum.enumCategory
                        });
                    }

                    foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("CampusAreaVarsity"))
                    {
                        var level = Singleton<UnlockManager>.instance.m_properties.m_ParkPolicyMilestones[(int)(policyEnum.enumValue & (DistrictPolicies.Policies)31)].GetLevel();

                        customMilestoneModel.Milestones[level].Policies.Add(new ItemModel()
                        {
                            Name = policyEnum.enumName,
                            LocalizedName = policyEnum.GetLocalizedName(),
                            Expansions = policyEnum.enumCategory
                        });
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

                    foreach (var policy in milestoneModel.Policies)
                    {
                        RefreshPolicyMilestones(policy.Name, milestoneInfo);
                    }
                }
            }
        }

        #region Refresh

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
                if (milestoneInfo.GetLevel() < net.GetUnlockMilestone().GetLevel())
                {
                    net.m_UnlockMilestone = milestoneInfo;
                }
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
                if (milestoneInfo.GetLevel() < building.GetUnlockMilestone().GetLevel())
                {
                    building.m_UnlockMilestone = milestoneInfo;
                }
                RefreshRelatedMilestone(name, building.category, building.m_class.m_service, building.m_class.m_subService, milestoneInfo);
            }
        }

        private void RefreshFeatureMilestones(string name, MilestoneInfo milestoneInfo, ModConfigModel modConfig = null, bool inGroup = false)
        {
            if (modConfig != null && modConfig.FeatureGroups.TryGetValue(name, out List<string> featureGroup) && !inGroup)
            {
                foreach (string item in featureGroup)
                {
                    RefreshFeatureMilestones(item, milestoneInfo, modConfig, true);
                }
            }
            else if (name.TryToEnumData(out PositionData<UnlockManager.Feature> featureEnum) && milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)featureEnum.enumValue].GetLevel())
            {
                Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)featureEnum.enumValue] = milestoneInfo;
            }
        }

        private void RefreshServiceMilestones(string name, MilestoneInfo milestoneInfo, ModConfigModel modConfig = null, bool inGroup = false)
        {
            if (modConfig != null && modConfig.ServiceExistsFeatures.Contains(name))
            {
                RefreshFeatureMilestones(name, milestoneInfo);
            }
            else if (name.TryToEnumData(out PositionData<ItemClass.Service> serviceEnum))
            {
                if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)serviceEnum.enumValue].GetLevel())
                {
                    Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)serviceEnum.enumValue] = milestoneInfo;
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

        private void RefreshPolicyMilestones(string name, MilestoneInfo milestoneInfo)
        {
            if (name.TryToEnumData(out PositionData<DistrictPolicies.Policies> policyEnum))
            {
                var i = (int)(policyEnum.enumValue & (DistrictPolicies.Policies)31);
                switch (policyEnum.enumCategory)
                {
                    case "Industry":
                    case "Residential":
                    case "Office":
                    case "Commercial":
                        if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_SpecializationMilestones[i].GetLevel())
                        {
                            Singleton<UnlockManager>.instance.m_properties.m_SpecializationMilestones[i] = milestoneInfo;
                        }

                        break;
                    case "Services":
                        if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_ServicePolicyMilestones[i].GetLevel())
                        {
                            Singleton<UnlockManager>.instance.m_properties.m_ServicePolicyMilestones[i] = milestoneInfo;
                        }
                        break;
                    case "Taxation":
                        if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_TaxationPolicyMilestones[i].GetLevel())
                        {
                            Singleton<UnlockManager>.instance.m_properties.m_TaxationPolicyMilestones[i] = milestoneInfo;
                        }
                        break;
                    case "CityPlanning":
                        if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_CityPlanningPolicyMilestones[i].GetLevel())
                        {
                            Singleton<UnlockManager>.instance.m_properties.m_CityPlanningPolicyMilestones[i] = milestoneInfo;
                        }
                        break;
                    case "Special":
                        if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_SpecialPolicyMilestones[i].GetLevel())
                        {
                            Singleton<UnlockManager>.instance.m_properties.m_SpecialPolicyMilestones[i] = milestoneInfo;
                        }
                        break;
                    case "Park":
                    case "IndustryArea":
                    case "CampusArea":
                    case "CampusAreaVarsity":
                        if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_ParkPolicyMilestones[i].GetLevel())
                        {
                            Singleton<UnlockManager>.instance.m_properties.m_ParkPolicyMilestones[i] = milestoneInfo;
                        }
                        break;
                }

                if (policyEnum.enumCategory.TryToEnumData(out PositionData<DistrictPolicies.Types> policyTypeEnum))
                {
                    if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_PolicyTypeMilestones[(int)policyTypeEnum.enumValue].GetLevel())
                    {
                        Singleton<UnlockManager>.instance.m_properties.m_PolicyTypeMilestones[(int)policyTypeEnum.enumValue] = milestoneInfo;
                    }
                }
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
                    if (milestoneInfo.GetLevel() < intersectionAI.GetUnlockMilestone().GetLevel())
                    {
                        SetPrivateVariable(intersectionAI, "m_cachedUnlockMilestone", milestoneInfo);
                    }
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

        private void SetPrivateVariable<T>(object obj, string fieldName, T value)
        {
            obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
        }

        #endregion
    }
}
