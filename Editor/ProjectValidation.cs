using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace KadenZombie8.BIMOS.Editor
{
    /// <summary>
    /// Throws errors with fix buttons if project is configured incorrectly for BIMOS
    /// </summary>
    public static class ProjectValidation
    {
        private const string _category = "BIMOS";
        const string _projectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";

        private const string _openXRLoaderTypeName = "UnityEngine.XR.OpenXR.OpenXRLoader";

        private static readonly BuildTargetGroup[] _buildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        private static bool TryGetManager(BuildTargetGroup targetGroup, out XRManagerSettings manager)
        {
            manager = null;

            EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
            if (!buildTargetSettings) return false;

            var settings = buildTargetSettings.SettingsForBuildTarget(targetGroup);
            if (!settings) return false;

            manager = settings.Manager;
            if (!manager) return false;

            return true;
        }

        private static bool IsOpenXRLoaderActive(BuildTargetGroup targetGroup)
        {
            if (!TryGetManager(targetGroup, out var manager)) return false;
            var activeLoaders = manager.activeLoaders;
            return activeLoaders.Any(loader => loader != null && loader.GetType().FullName.Equals(_openXRLoaderTypeName));
        }

        private static void AssignOpenXRLoader(BuildTargetGroup targetGroup)
        {
            if (!TryGetManager(targetGroup, out var manager)) return;
            XRPackageMetadataStore.AssignLoader(manager, _openXRLoaderTypeName, targetGroup);
        }

        private static bool HasLayer(string layerName) => LayerMask.NameToLayer(layerName) != -1;

        private static void CreateLayer(string layerName)
        {
            if (string.IsNullOrEmpty(layerName)) return;
            if (HasLayer(layerName)) return;

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layers = tagManager.FindProperty("layers");

            for (int i = 8; i <= 31; i++)
            {
                var layer = layers.GetArrayElementAtIndex(i);
                if (!string.IsNullOrEmpty(layer.stringValue)) continue;

                layer.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"[BIMOS] Created layer {layerName}.");
                return;
            }

            Debug.LogError($"[BIMOS] Could not create layer {layerName}. All user layers are in use.");
        }

        private static readonly List<BuildValidationRule> _buildValidationRules = new()
        {
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must be using Unity 6 or greater",
                Category = _category,
                CheckPredicate = () =>
                {
                    var version = Application.unityVersion;
                    if (!int.TryParse(version.Split(".")[0], out var major)) return false;
                    return major >= 6000;
                },
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Plug-in Provider must be OpenXR for PC",
                Category = _category,
                CheckPredicate = () => IsOpenXRLoaderActive(BuildTargetGroup.Standalone),
                FixIt = () => AssignOpenXRLoader(BuildTargetGroup.Standalone),
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Plug-in Provider must be OpenXR for Android",
                Category = _category,
                CheckPredicate = () => IsOpenXRLoaderActive(BuildTargetGroup.Android),
                FixIt = () => AssignOpenXRLoader(BuildTargetGroup.Android),
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must enable PC controller profiles and features",
                Category = _category,
                CheckPredicate = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);

                    if (!settings.GetFeature<OculusTouchControllerProfile>().enabled) return false;
                    if (!settings.GetFeature<PalmPoseInteraction>().enabled) return false;

                    return true;
                },
                FixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
                    if (!settings) return;

                    settings.GetFeature<OculusTouchControllerProfile>().enabled
                        = settings.GetFeature<PalmPoseInteraction>().enabled
                        = true;
                },
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must enable Android controller profiles and features",
                Category = _category,
                CheckPredicate = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
                    if (!settings) return false;

                    if (!settings.GetFeature<OculusTouchControllerProfile>().enabled) return false;
                    if (!settings.GetFeature<MetaQuestTouchPlusControllerProfile>().enabled) return false;
                    if (!settings.GetFeature<MetaQuestTouchProControllerProfile>().enabled) return false;
                    if (!settings.GetFeature<PalmPoseInteraction>().enabled) return false;
                    if (!settings.GetFeature<MetaQuestFeature>().enabled) return false;

                    return true;
                },
                FixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
                    if (!settings) return;

                    settings.GetFeature<OculusTouchControllerProfile>().enabled
                        = settings.GetFeature<MetaQuestTouchPlusControllerProfile>().enabled
                        = settings.GetFeature<MetaQuestTouchProControllerProfile>().enabled
                        = settings.GetFeature<PalmPoseInteraction>().enabled
                        = settings.GetFeature<MetaQuestFeature>().enabled
                        = true;
                },
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must have a layer called \"BIMOSRig\"",
                Category = _category,
                CheckPredicate = () => HasLayer("BIMOSRig"),
                FixIt = () => CreateLayer("BIMOSRig"),
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must have a layer called \"BIMOSMenu\"",
                Category = _category,
                CheckPredicate = () => HasLayer("BIMOSMenu"),
                FixIt = () => CreateLayer("BIMOSMenu"),
                Error = true
            }
        };

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            foreach (var buildTargetGroup in _buildTargetGroups)
                BuildValidator.AddRules(buildTargetGroup, _buildValidationRules);

            // Delay evaluating conditions for issues to give time for Package Manager and UPM cache to fully initialize.
            EditorApplication.delayCall += ShowWindowIfIssuesExist;
        }

        static void ShowWindowIfIssuesExist()
        {
            foreach (var validation in _buildValidationRules)
                if (validation.CheckPredicate == null || !validation.CheckPredicate.Invoke())
                {
                    ShowWindow();
                    return;
                }
        }

        internal static void ShowWindow()
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified.
            EditorApplication.delayCall += () => SettingsService.OpenProjectSettings(_projectValidationSettingsPath);
        }
    }
}
