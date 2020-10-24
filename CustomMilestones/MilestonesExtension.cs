using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.IO;
using CustomMilestones.Helpers;
using CustomMilestones.Models;
using ICities;
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
        private static readonly string _xmlFilePath = DataLocation.executableDirectory + "\\CustomMilestone.xml";
        private static readonly string _modConfigFilePath = "Resources\\config.json";

        public override void OnCreated(IMilestones milestones)
        {
            base.OnCreated(milestones);

            #region 读取/创建Xml文件

            CustomMilestoneModel customMilestone = XmlHelper.FromXmlFile<CustomMilestoneModel>(_xmlFilePath);
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

                if (Singleton<UnlockManager>.exists)
                {
                    foreach (var milestone in Singleton<UnlockManager>.instance.m_properties.m_progressionMilestones)
                    {
                        int purchaseAreasCount = Singleton<UnlockManager>.instance.m_properties.m_AreaMilestones.Count(m => m.GetLevel() == milestone.GetLevel());
                        customMilestone.Milestones[milestone.GetLevel()].LocalizedName = milestone.GetLocalizedName();
                        customMilestone.Milestones[milestone.GetLevel()].RewardCash = milestone.m_rewardCash;
                        customMilestone.Milestones[milestone.GetLevel()].PurchaseAreasCount = purchaseAreasCount;
                    }
                }
                XmlHelper.ToXmlFile(customMilestone, _xmlFilePath);
            }

            #endregion
        }

        public override void OnRefreshMilestones()
        {
            if (managers.loading.currentMode == AppMode.Game && Singleton<UnlockManager>.exists)
            {
                RefreshMilestones();
            }
            milestonesManager.UnlockMilestone("Basic Road Created");
        }

        public void RefreshMilestones()
        {
            ModConfigModel config = JsonHelper.FromJsonFile<ModConfigModel>(Path.Combine(ModHelper.GetPath(), _modConfigFilePath)) ?? new ModConfigModel();
            CustomMilestoneModel customMilestone = XmlHelper.FromXmlFile<CustomMilestoneModel>(_xmlFilePath);

            if (customMilestone.Rebuild)
            {
                customMilestone.Rebuild = false;

                //读取默认道路信息
                for (uint index = 0; index < PrefabCollection<NetInfo>.LoadedCount(); index++)
                {
                    NetInfo net = PrefabCollection<NetInfo>.GetLoaded(index);
                    if (config.RoadIncludes.Contains(config.Renames.GetRename(net.name)) && !customMilestone.Exists(config.Renames.GetRename(net.name)))
                    {
                        if (config.BuildingExistsRoads.Contains(config.Renames.GetRename(net.name)))
                        {
                            customMilestone.Milestones[net.GetUnlockMilestone().GetLevel()].Buildings.Add(new ItemModel()
                            {
                                Name = config.Renames.GetRename(net.name),
                                LocalizedName = net.GetLocalizedTitle(),
                                Expansions = net.m_class.m_service.ToString() + "|" + net.m_class.m_subService.ToString() + "|" + net.category
                            });
                        }
                        else
                        {
                            customMilestone.Milestones[net.GetUnlockMilestone().GetLevel()].Roads.Add(new ItemModel()
                            {
                                Name = config.Renames.GetRename(net.name),
                                LocalizedName = net.GetLocalizedTitle(),
                                Expansions = net.m_class.m_service.ToString() + "|" + net.m_class.m_subService.ToString() + "|" + net.category
                            });
                        }
                    }
                }

                //读取默认建筑信息
                for (uint index = 0; index < PrefabCollection<BuildingInfo>.LoadedCount(); index++)
                {
                    BuildingInfo building = PrefabCollection<BuildingInfo>.GetLoaded(index);
                    if (config.BuildingIncludes.Contains(config.Renames.GetRename(building.name)) && !customMilestone.Exists(config.Renames.GetRename(building.name)))
                    {
                        if (config.RoadExistsBuildings.Contains(config.Renames.GetRename(building.name)))
                        {
                            customMilestone.Milestones[building.GetUnlockMilestone().GetLevel()].Roads.Add(new ItemModel()
                            {
                                Name = config.Renames.GetRename(building.name),
                                LocalizedName = building.GetLocalizedTitle(),
                                Expansions = building.m_class.m_service.ToString() + "|" + building.m_class.m_subService.ToString() + "|" + building.category
                            });
                        }
                        else
                        {
                            customMilestone.Milestones[building.GetUnlockMilestone().GetLevel()].Buildings.Add(new ItemModel()
                            {
                                Name = config.Renames.GetRename(building.name),
                                LocalizedName = building.GetLocalizedTitle(),
                                Expansions = building.m_class.m_service.ToString() + "|" + building.m_class.m_subService.ToString() + "|" + building.category
                            });
                        }
                    }
                }

                //读取默认功能信息
                foreach (var featureEnum in Utils.GetOrderedEnumData<UnlockManager.Feature>())
                {
                    if (config.Features.Contains(featureEnum.enumName) && !customMilestone.Exists(featureEnum.enumName, "Feature"))
                    {
                        var level = Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)featureEnum.enumValue].GetLevel();
                        if (config.ServiceExistsFeatures.Contains(featureEnum.enumName))
                        {
                            customMilestone.Milestones[level].Services.Add(new ItemModel()
                            {
                                Name = featureEnum.enumName,
                                LocalizedName = featureEnum.GetLocalizedName(),
                            });
                        }
                        else
                        {
                            customMilestone.Milestones[level].Features.Add(new ItemModel()
                            {
                                Name = featureEnum.enumName,
                                LocalizedName = featureEnum.GetLocalizedName(),
                            });
                        }
                    }
                }

                //读取默认服务信息
                foreach (var serviceEnum in Utils.GetOrderedEnumData<ItemClass.Service>())
                {
                    if (config.Services.Contains(serviceEnum.enumName) && !customMilestone.Exists(serviceEnum.enumName, "Service"))
                    {
                        var level = Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)serviceEnum.enumValue].GetLevel();
                        customMilestone.Milestones[level].Services.Add(new ItemModel()
                        {
                            Name = serviceEnum.enumName,
                            LocalizedName = serviceEnum.GetLocalizedName(),
                        });
                    }
                }

                //读取默认服务政策信息
                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Services"))
                {
                    var index = (int)(policyEnum.enumValue & (DistrictPolicies.Policies)31);
                    var level = Singleton<UnlockManager>.instance.m_properties.m_ServicePolicyMilestones[index].GetLevel();
                    customMilestone.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                //读取默认税收政策信息
                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("Taxation"))
                {
                    var index = (int)(policyEnum.enumValue & (DistrictPolicies.Policies)31);
                    var level = Singleton<UnlockManager>.instance.m_properties.m_TaxationPolicyMilestones[index].GetLevel();
                    customMilestone.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                //读取默认城市规划政策信息
                foreach (var policyEnum in Utils.GetOrderedEnumData<DistrictPolicies.Policies>("CityPlanning"))
                {
                    var index = (int)(policyEnum.enumValue & (DistrictPolicies.Policies)31);
                    var level = Singleton<UnlockManager>.instance.m_properties.m_CityPlanningPolicyMilestones[index].GetLevel();
                    customMilestone.Milestones[level].Policies.Add(new ItemModel()
                    {
                        Name = policyEnum.enumName,
                        LocalizedName = policyEnum.GetLocalizedName(),
                        Expansions = policyEnum.enumCategory
                    });
                }

                //读取默认信息面板信息
                foreach (var infoMode in Utils.GetOrderedEnumData<InfoManager.InfoMode>())
                {
                    var level = Singleton<UnlockManager>.instance.m_properties.m_InfoModeMilestones[(int)infoMode.enumValue].GetLevel();
                    customMilestone.Milestones[level].InfoViews.Add(new ItemModel()
                    {
                        Name = infoMode.enumName,
                        LocalizedName = Locale.Get("INFOVIEWS", infoMode.enumName)
                    });
                }

                XmlHelper.ToXmlFile(customMilestone, _xmlFilePath);
            }
            else
            {
                MilestoneInfo[] progressionMilestones = Singleton<UnlockManager>.instance.m_properties.m_progressionMilestones;

                //根据配置文件刷新里程碑信息
                foreach (MilestoneModel milestoneModel in customMilestone.Milestones)
                {
                    MilestoneInfo milestoneInfo = milestoneModel.Level > 0 ? progressionMilestones[milestoneModel.Level - 1] : null;

                    //刷新里程碑奖金、地块
                    var count = (milestoneModel.Level == 0 && milestoneModel.PurchaseAreasCount == 0) ? 1 : milestoneModel.PurchaseAreasCount;
                    var total = customMilestone.Milestones.Take(Array.IndexOf(customMilestone.Milestones, milestoneModel)).Sum(m => m.PurchaseAreasCount);
                    for (int i = total; i < total + count && i < 9; i++)
                    {
                        Singleton<UnlockManager>.instance.m_properties.m_AreaMilestones[i] = milestoneInfo;
                    }
                    if (milestoneInfo != null)
                    {
                        milestoneInfo.m_rewardCash = milestoneModel.RewardCash.Value;
                    }

                    //刷新道路
                    foreach (var roadModel in milestoneModel.Roads)
                    {
                        if (config.RoadIncludes.Contains(roadModel.Name) || config.RoadExistsBuildings.Contains(roadModel.Name))
                        {
                            RefreshRoadMilestone(roadModel.Name, milestoneInfo, config);
                        }
                    }

                    //刷新建筑
                    foreach (var buildingModel in milestoneModel.Buildings)
                    {
                        if (config.BuildingIncludes.Contains(buildingModel.Name) || config.BuildingExistsRoads.Contains(buildingModel.Name))
                        {
                            RefreshBuildingMilestone(buildingModel.Name, milestoneInfo, config);
                        }
                    }

                    //刷新功能
                    foreach (var featureModel in milestoneModel.Features)
                    {
                        if (config.Features.Contains(featureModel.Name))
                        {
                            RefreshFeatureMilestones(featureModel.Name, milestoneInfo, config);
                        }
                    }

                    //刷新服务
                    foreach (var serviceModel in milestoneModel.Services)
                    {
                        if (config.Services.Contains(serviceModel.Name) || config.ServiceExistsFeatures.Contains(serviceModel.Name))
                        {
                            RefreshServiceMilestones(serviceModel.Name, milestoneInfo, config);
                        }
                    }

                    //刷新政策
                    foreach (var policyModel in milestoneModel.Policies)
                    {
                        RefreshPolicyMilestones(policyModel.Name, milestoneInfo);
                    }
                }
            }
        }

        #region Refresh

        /// <summary>
        /// 刷新道路里程碑
        /// </summary>
        /// <param name="name">道路名称</param>
        /// <param name="milestoneInfo">新的里程碑进度</param>
        /// <param name="config">配置信息</param>
        /// <param name="inGroup">是否在组</param>
        private void RefreshRoadMilestone(string name, MilestoneInfo milestoneInfo, ModConfigModel config, bool inGroup = false)
        {
            if (config.RoadExistsBuildings.Contains(name))
            {
                RefreshBuildingMilestone(name, milestoneInfo, config);
            }
            else if (config.RoadGroups.TryGetValue(name, out List<string> roadGroup) && !inGroup)
            {
                foreach (string roadName in roadGroup)
                {
                    RefreshRoadMilestone(roadName, milestoneInfo, config, true);
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

        /// <summary>
        /// 刷新建筑里程碑
        /// </summary>
        /// <param name="name">建筑名称</param>
        /// <param name="milestoneInfo">新的里程碑进度</param>
        /// <param name="config">配置信息</param>
        /// <param name="inGroup">是否在组</param>
        private void RefreshBuildingMilestone(string name, MilestoneInfo milestoneInfo, ModConfigModel config, bool inGroup = false)
        {
            if (config.BuildingExistsRoads.Contains(name) || config.BuildingContainedRoads.Contains(name))
            {
                RefreshRoadMilestone(name, milestoneInfo, config);
            }
            else if (config.BuildingGroups.TryGetValue(name, out List<string> buildingGroup) && !inGroup)
            {
                foreach (string buildingName in buildingGroup)
                {
                    RefreshBuildingMilestone(buildingName, milestoneInfo, config, true);
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

        /// <summary>
        /// 刷新相关的里程碑（服务、子服务、功能等）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="service"></param>
        /// <param name="subService"></param>
        /// <param name="milestoneInfo"></param>
        private void RefreshRelatedMilestone(string name, string category, ItemClass.Service service, ItemClass.SubService subService, MilestoneInfo milestoneInfo)
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
            RefreshServiceMilestones(service.ToString(), milestoneInfo);
            RefreshSubServiceMilestones(subService, milestoneInfo);
        }

        /// <summary>
        /// 刷新功能里程碑
        /// </summary>
        /// <param name="name"></param>
        /// <param name="milestoneInfo"></param>
        /// <param name="config"></param>
        /// <param name="inGroup"></param>
        private void RefreshFeatureMilestones(string name, MilestoneInfo milestoneInfo, ModConfigModel config = null, bool inGroup = false)
        {
            if (config != null && config.FeatureGroups.TryGetValue(name, out List<string> featureGroup) && !inGroup)
            {
                foreach (string item in featureGroup)
                {
                    RefreshFeatureMilestones(item, milestoneInfo, config, true);
                }
            }
            else if (EnumHelper.TryToEnumData(name, out PositionData<UnlockManager.Feature> featureEnum))
            {
                if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)featureEnum.enumValue].GetLevel())
                {
                    Singleton<UnlockManager>.instance.m_properties.m_FeatureMilestones[(int)featureEnum.enumValue] = milestoneInfo;
                }
            }
        }

        /// <summary>
        /// 刷新服务里程碑
        /// </summary>
        /// <param name="name"></param>
        /// <param name="milestoneInfo"></param>
        /// <param name="config"></param>
        /// <param name="inGroup"></param>
        private void RefreshServiceMilestones(string name, MilestoneInfo milestoneInfo, ModConfigModel config = null, bool inGroup = false)
        {
            if (config != null && config.ServiceExistsFeatures.Contains(name))
            {
                RefreshFeatureMilestones(name, milestoneInfo);
            }
            else if (EnumHelper.TryToEnumData(name, out PositionData<ItemClass.Service> serviceEnum))
            {
                if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)serviceEnum.enumValue].GetLevel())
                {
                    Singleton<UnlockManager>.instance.m_properties.m_ServiceMilestones[(int)serviceEnum.enumValue] = milestoneInfo;
                }
            }
        }

        /// <summary>
        /// 刷新子服务里程碑
        /// </summary>
        /// <param name="subService"></param>
        /// <param name="milestoneInfo"></param>
        private void RefreshSubServiceMilestones(ItemClass.SubService subService, MilestoneInfo milestoneInfo)
        {
            if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_SubServiceMilestones[(int)subService].GetLevel())
            {
                Singleton<UnlockManager>.instance.m_properties.m_SubServiceMilestones[(int)subService] = milestoneInfo;
            }
        }

        /// <summary>
        /// 刷新政策里程碑
        /// </summary>
        /// <param name="name"></param>
        /// <param name="milestoneInfo"></param>
        private void RefreshPolicyMilestones(string name, MilestoneInfo milestoneInfo)
        {
            if (EnumHelper.TryToEnumData(name, out PositionData<DistrictPolicies.Policies> policyEnum))
            {
                RefreshFeatureMilestones(UnlockManager.Feature.Policies.ToString(), milestoneInfo);
                if (EnumHelper.TryToEnumData(policyEnum.GetCategory(), out PositionData<DistrictPolicies.Types> policyTypeEnum))
                {
                    if (milestoneInfo.GetLevel() < Singleton<UnlockManager>.instance.m_properties.m_PolicyTypeMilestones[(int)policyTypeEnum.enumValue].GetLevel())
                    {
                        Singleton<UnlockManager>.instance.m_properties.m_PolicyTypeMilestones[(int)policyTypeEnum.enumValue] = milestoneInfo;
                    }
                }
                var i = (int)(policyEnum.enumValue & (DistrictPolicies.Policies)31);
                switch (policyEnum.GetCategory())
                {
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
                }
            }
        }

        private void SetPrivateVariable<T>(object obj, string fieldName, T value)
        {
            obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
        }

        #endregion
    }
}
