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
    public class ProjectValidation : MonoBehaviour
    {
        private const string _category = "BIMOS";
        const string _projectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";

        private const string _openXRLoaderTypeName = "UnityEngine.XR.OpenXR.OpenXRLoader";

        private static readonly BuildTargetGroup[] _buildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        private static readonly List<BuildValidationRule> _buildValidationRules = new()
        {
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must be using Unity 6 or greater",
                Category = _category,
                CheckPredicate = () =>
                {
                    var major = int.Parse(Application.unityVersion.Split(".")[0]);
                    return major >= 6000;
                },
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Plug-in Provider must be OpenXR for PC",
                Category = _category,
                CheckPredicate = () =>
                {
                    EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
                    XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Standalone);
                    var activeLoaders = settings.Manager.activeLoaders;
                    if (activeLoaders.Count <= 0)
                        return false;

                    var activeLoader = activeLoaders[0];
                    var loaderTypeName = activeLoader.GetType().FullName;
                    return activeLoader.GetType().FullName.Equals(_openXRLoaderTypeName);
                },
                FixIt = () =>
                {
                    EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
                    XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Standalone);
                    XRPackageMetadataStore.AssignLoader(settings.Manager, _openXRLoaderTypeName, BuildTargetGroup.Standalone);
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Plug-in Provider must be OpenXR for Android",
                Category = _category,
                CheckPredicate = () =>
                {
                    EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
                    XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Android);
                    var activeLoaders = settings.Manager.activeLoaders;
                    if (activeLoaders.Count <= 0)
                        return false;

                    var activeLoader = activeLoaders[0];
                    var loaderTypeName = activeLoader.GetType().FullName;
                    return activeLoader.GetType().FullName.Equals(_openXRLoaderTypeName);
                },
                FixIt = () =>
                {
                    EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
                    XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Android);
                    XRPackageMetadataStore.AssignLoader(settings.Manager, _openXRLoaderTypeName, BuildTargetGroup.Android);
                },
                FixItAutomatic = true,
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
                    if (!settings.GetFeature<OculusTouchControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<PalmPoseInteraction>().enabled)
                        return false;

                    return true;
                },
                FixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
                    settings.GetFeature<OculusTouchControllerProfile>().enabled
                        = settings.GetFeature<PalmPoseInteraction>().enabled
                        = true;
                },
                FixItAutomatic = true,
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
                    if (!settings.GetFeature<OculusTouchControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<MetaQuestTouchPlusControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<MetaQuestTouchProControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<PalmPoseInteraction>().enabled)
                        return false;
                    else if (!settings.GetFeature<MetaQuestFeature>().enabled)
                        return false;

                    return true;
                },
                FixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
                    settings.GetFeature<OculusTouchControllerProfile>().enabled
                        = settings.GetFeature<MetaQuestTouchPlusControllerProfile>().enabled
                        = settings.GetFeature<MetaQuestTouchProControllerProfile>().enabled
                        = settings.GetFeature<PalmPoseInteraction>().enabled
                        = settings.GetFeature<MetaQuestFeature>().enabled
                        = true;
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must have a layer called \"BIMOSRig\"",
                Category = _category,
                CheckPredicate = () =>
                {
                    for (int i = 0; i < 32; i++)
                        if (LayerMask.LayerToName(i).Equals("BIMOSRig"))
                            return true;

                    return false;
                },
                FixIt = () =>
                {
                    SettingsService.OpenProjectSettings("Project/Tags and Layers");
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must have a layer called \"BIMOSMenu\"",
                Category = _category,
                CheckPredicate = () =>
                {
                    for (int i = 0; i < 32; i++)
                        if (LayerMask.LayerToName(i).Equals("BIMOSMenu"))
                            return true;

                    return false;
                },
                FixIt = () =>
                {
                    SettingsService.OpenProjectSettings("Project/Tags and Layers");
                },
                FixItAutomatic = true,
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
