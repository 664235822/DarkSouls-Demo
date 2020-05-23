//Copyright © 2019 Procedural Worlds Pty Limited. All Rights Reserved.
#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
#endif
#if HDPipeline
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif
#if LWPipeline
#if UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering.LWRP;
#else
using UnityEngine.Experimental.Rendering.LightweightPipeline;
#endif
#endif
#if CTS_PRESENT
using CTS;
#endif
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Gaia
{
    public static class GaiaPipelineUtils
    {
        //private static string m_currentOperation = "None";

        #region Utils

        /// <summary>
        /// Configures projects for the pipeline selected
        /// </summary>
        /// <param name="renderPipelineSettings"></param>
        /// <param name="renderPipelineAsset"></param>
        /// <param name="terrainMaterial"></param>
        /// <param name="terrainShader"></param>
        public static void SetupPipeline(GaiaConstants.EnvironmentRenderer renderPipelineSettings, string renderPipelineAsset, string terrainMaterial, string terrainShader, string waterShader, bool finaliseEnvironment)
        {
            switch (renderPipelineSettings)
            {
                case GaiaConstants.EnvironmentRenderer.BuiltIn:
                    #region Built-In
                    //Terrain object
                    Terrain[] terrains = GetActiveTerrains();
                    //Water Material
                    Material waterMaterial = GetWaterMaterial();

                    GraphicsSettings.renderPipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(GetAssetPath(renderPipelineAsset));
                    RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath("Ambient Skies Skybox"));

                    if (waterMaterial != null)
                    {
                        waterMaterial.shader = Shader.Find(waterShader);
                        waterMaterial.SetFloat("_FoamOpacity", 0.2f);
                    }

#if GAIA_PRESENT
                    GaiaSettings[] gaiaSettingsArray = Resources.FindObjectsOfTypeAll<GaiaSettings>();
                    if (gaiaSettingsArray != null)
                    {
                        foreach (GaiaSettings settings in gaiaSettingsArray)
                        {
                            settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
                            EditorUtility.SetDirty(settings);
                        }
                    }
#endif

#if !CTS_PRESENT
                if (terrains != null)
                {
                    foreach (Terrain activeTerrains in terrains)
                    {
                        activeTerrains.materialType = Terrain.MaterialType.BuiltInStandard;
                        activeTerrains.materialTemplate = null;
                    }
                }
#else
                    if (terrains != null && Object.FindObjectOfType<CompleteTerrainShader>() != null)
                    {
                        CompleteTerrainShader[] ctsShader = Object.FindObjectsOfType<CompleteTerrainShader>();
                        if (ctsShader != null)
                        {
                            foreach (CompleteTerrainShader ctsSettings in ctsShader)
                            {
                                CTSTerrainManager.Instance.BroadcastProfileUpdate(ctsSettings.Profile);
                            }
                        }
                    }
                    else
                    {
                        if (terrains != null)
                        {
                            foreach (Terrain activeTerrains in terrains)
                            {
                                activeTerrains.materialType = Terrain.MaterialType.BuiltInStandard;
                                activeTerrains.materialTemplate = null;
                            }
                        }
                    }
#endif
                    BakeGlobalReflectionProbe(false);

                    if (finaliseEnvironment)
                    {
                        SetupHDEnvironmentalVolume(false, true, false, true, false, false, false, renderPipelineSettings, "High Definition Wind", "High Definition Environment Volume", 1f, 1f, "Density Volume", "FFFFFF", 150f, new Vector3(0.35f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleDay", "FFF6E9FF", 171.5f, 1.14f, 1200f, 15f);
#if UNITY_2019_1_OR_NEWER
                        WaterSetup(renderPipelineSettings, true);
#else
                        WaterSetup(renderPipelineSettings, false);
#endif
                        FixPostProcessingV2(renderPipelineSettings);
                        DestroyParent("Ambient Skies Environment");
                    }

                    SceneView.RepaintAll();

                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }

                    #endregion
                    break;
                case GaiaConstants.EnvironmentRenderer.LightWeight2018x:
                    #region Lightweight
                    //Terrain object
                    Terrain[] terrainsLW = GetActiveTerrains();
                    //Water Material
                    Material waterMaterialLW = GetWaterMaterial();

                    GraphicsSettings.renderPipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(GetAssetPath(renderPipelineAsset));
                    RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath("Ambient Skies Skybox"));

                    
                    if (waterMaterialLW != null)
                    {
                        waterMaterialLW.shader = Shader.Find(waterShader);
                        waterMaterialLW.SetFloat("_FoamOpacity", 0f);
                    }

#if GAIA_PRESENT
                    GaiaSettings[] gaiaSettingsArrayLW = Resources.FindObjectsOfTypeAll<GaiaSettings>();
                    if (gaiaSettingsArrayLW != null)
                    {
                        foreach (GaiaSettings settings in gaiaSettingsArrayLW)
                        {
                            settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.LightWeight2018x;
                            EditorUtility.SetDirty(settings);
                        }
                    }
#endif

#if !CTS_PRESENT
                if (terrainsLW != null)
                {
                    foreach (Terrain activeTerrains in terrainsLW)
                    {
                        activeTerrains.materialType = Terrain.MaterialType.Custom;
                        activeTerrains.materialTemplate = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath(terrainMaterial));
                    }

                    Material material = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath(terrainMaterial));
                    if (material != null)
                    {
                        material.shader = Shader.Find(terrainShader);
                        material.enableInstancing = true;
                    }
                }
#else
                    if (terrainsLW != null && Object.FindObjectOfType<CompleteTerrainShader>() != null)
                    {
                        CompleteTerrainShader[] ctsShader = Object.FindObjectsOfType<CompleteTerrainShader>();
                        if (ctsShader != null)
                        {
                            foreach (CompleteTerrainShader ctsSettings in ctsShader)
                            {
                                CTSTerrainManager.Instance.BroadcastProfileUpdate(ctsSettings.Profile);
                            }
                        }
                    }
                    else
                    {
                        foreach (Terrain activeTerrains in terrainsLW)
                        {
                            activeTerrains.materialType = Terrain.MaterialType.Custom;
                            activeTerrains.materialTemplate = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath(terrainMaterial));
                        }
                        Material material = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath(terrainMaterial));
                        if (material != null)
                        {
                            material.shader = Shader.Find(terrainShader);
                            material.enableInstancing = true;
                        }
                    }
#endif

#if LWPipeline && UNITY_2018_3_OR_NEWER
                Camera camera = Object.FindObjectOfType<Camera>();
                if (camera != null)
                {
                    camera.gameObject.AddComponent<LWRPAdditionalCameraData>();
                }
                GameObject light = GetMainDirectionalLight();
                if (light != null)
                {
                    light.AddComponent<LWRPAdditionalLightData>();
                }
#endif
                    BakeGlobalReflectionProbe(false);

                    if (finaliseEnvironment)
                    {
                        SetupHDEnvironmentalVolume(false, true, false, true, false, false, false, renderPipelineSettings, "High Definition Wind", "High Definition Environment Volume", 1f, 1f, "Density Volume", "FFFFFF", 150f, new Vector3(0.35f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleDay", "FFF6E9FF", 171.5f, 1.14f, 1200f, 15f);
#if UNITY_2019_1_OR_NEWER
                        WaterSetup(renderPipelineSettings, true);
#else
                        WaterSetup(renderPipelineSettings, false);
#endif
                        FixPostProcessingV2(renderPipelineSettings);
                        DestroyParent("Ambient Skies Environment");
                    }

                    RenderPipelineAsset pipelineAsset = GraphicsSettings.renderPipelineAsset;
                    if (pipelineAsset != null)
                    {
                        EditorUtility.SetDirty(pipelineAsset);
                        AssetDatabase.SaveAssets();
                        SceneView.RepaintAll();
                    }

                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }

                    #endregion
                    break;
                case GaiaConstants.EnvironmentRenderer.HighDefinition2018x:
                    #region High Definition
                    //Terrain object
                    Terrain[] terrainsHD = GetActiveTerrains();
                    //Water Material
                    Material waterMaterialHD = GetWaterMaterial();

                    if (PlayerSettings.colorSpace == ColorSpace.Gamma)
                    {
                        if (EditorUtility.DisplayDialog("Incorrect Color Space!", "High Definition requires Linear Color Space. Would you like to change your color space to Linear?", "Yes", "Cancel"))
                        {
                            SetLinearDeferredLighting();
                        }
                    }

                    ApplyHDPipelineResources(renderPipelineSettings, renderPipelineAsset);

                    GraphicsSettings.renderPipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(GetAssetPath(renderPipelineAsset));

                    if (waterMaterialHD != null)
                    {
                        waterMaterialHD.shader = Shader.Find(waterShader);
                        waterMaterialHD.SetFloat("_FoamOpacity", 0f);
                    }
#if GAIA_PRESENT
                    GaiaSettings[] gaiaSettingsArrayHD = Resources.FindObjectsOfTypeAll<GaiaSettings>();
                    if (gaiaSettingsArrayHD != null)
                    {
                        foreach (GaiaSettings settings in gaiaSettingsArrayHD)
                        {
                            settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.HighDefinition2018x;
                            EditorUtility.SetDirty(settings);
                        }
                    }
#endif

#if !CTS_PRESENT
                if (terrainsHD != null)
                {
                    foreach (Terrain activeTerrains in terrainsHD)
                    {
                        activeTerrains.materialType = Terrain.MaterialType.Custom;
                        activeTerrains.materialTemplate = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath(terrainMaterial));
                    }

                    Material material = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath(terrainMaterial));
                    if (material != null)
                    {
                        material.shader = Shader.Find(terrainShader);
                        material.enableInstancing = true;
#if UNITY_2018_3_OR_NEWER
                        material.SetFloat("_EnableHeightBlend", 1f);
#endif
                    }
                }
#else
                    if (terrainsHD != null && Object.FindObjectOfType<CompleteTerrainShader>() != null)
                    {
                        CompleteTerrainShader[] ctsShader = Object.FindObjectsOfType<CompleteTerrainShader>();
                        if (ctsShader != null)
                        {
                            foreach (CompleteTerrainShader ctsSettings in ctsShader)
                            {
                                CTSTerrainManager.Instance.BroadcastProfileUpdate(ctsSettings.Profile);
                            }
                        }
                    }
                    else
                    {
                        foreach (Terrain activeTerrains in terrainsHD)
                        {
                            activeTerrains.materialType = Terrain.MaterialType.Custom;
                            activeTerrains.materialTemplate = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath(terrainMaterial));
                        }

                        Material material = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath(terrainMaterial));
                        if (material != null)
                        {
                            material.shader = Shader.Find(terrainShader);
                            material.enableInstancing = true;
#if UNITY_2018_3_OR_NEWER
                            material.SetFloat("_EnableHeightBlend", 1f);
#endif
                        }
                    }
#endif

#if HDPipeline
                    Camera camera = Object.FindObjectOfType<Camera>();
                    if (camera != null)
                    {
                        if (camera.GetComponent<HDAdditionalCameraData>() == null)
                        {
                            camera.gameObject.AddComponent<HDAdditionalCameraData>();
                        }
                    }

                    GameObject light = GetMainDirectionalLight();
                    if (light != null)
                    {
                        if (light.GetComponent<AdditionalShadowData>() == null)
                        {
                            light.AddComponent<AdditionalShadowData>();
                        }

                        if (light.GetComponent<HDAdditionalLightData>() == null)
                        {
                            light.AddComponent<HDAdditionalLightData>();
                        }
                    }

                    //Get relevant information
                    GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
                    GameObject parentObject = GetOrCreateEnvironmentParent();
                    Terrain activeTerrain = GetActiveTerrain();
                    GameObject underwaterProbeHD = GameObject.Find("Underwater Reflection Probe");

                    bool useUnderwaterProbe = false;
                    if (useUnderwaterProbe)
                    {
                        if (underwaterProbeHD == null)
                        {
                            underwaterProbeHD = new GameObject("Underwater Reflection Probe");
                            underwaterProbeHD.transform.SetParent(parentObject.transform);
                            underwaterProbeHD.transform.position = new Vector3(0f, sceneInfo.m_seaLevel - 1f, 0f);

                            ReflectionProbe probe = underwaterProbeHD.AddComponent<ReflectionProbe>();
                            probe.mode = ReflectionProbeMode.Baked;

                            HDAdditionalReflectionData hdProbe = underwaterProbeHD.AddComponent<HDAdditionalReflectionData>();
                            if (activeTerrain != null)
                            {
                                hdProbe.influenceVolume.boxSize = (new Vector3(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z));
                            }
                            else
                            {
                                hdProbe.influenceVolume.boxSize = (new Vector3(2000f, 200f, 2000f));
                            }

                            hdProbe.weight = 0.7f;
                            hdProbe.multiplier = 0.4f;

                            BakeGlobalReflectionProbe(false);
                        }
                        else
                        {
                            underwaterProbeHD.transform.position = new Vector3(0f, sceneInfo.m_seaLevel - 1f, 0f);

                            ReflectionProbe probe = underwaterProbeHD.GetComponent<ReflectionProbe>();
                            if (probe != null)
                            {
                                probe.mode = ReflectionProbeMode.Baked;
                            }

                            HDAdditionalReflectionData hdProbe = underwaterProbeHD.GetComponent<HDAdditionalReflectionData>();
                            if (hdProbe != null)
                            {
                                if (activeTerrain != null)
                                {
                                    hdProbe.influenceVolume.boxSize = (new Vector3(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z));
                                }
                                else
                                {
                                    hdProbe.influenceVolume.boxSize = (new Vector3(2000f, 200f, 2000f));
                                }

                                hdProbe.weight = 0.7f;
                                hdProbe.multiplier = 0.4f;
#if !UNITY_2019_1_OR_NEWER
                                hdProbe.mode = ReflectionProbeMode.Baked;
#else
                                hdProbe.mode = ProbeSettings.Mode.Baked;
#endif

                                BakeGlobalReflectionProbe(false);
                            }
                        }
                    }
                    else
                    {
                        if (underwaterProbeHD != null)
                        {
                            Object.DestroyImmediate(underwaterProbeHD);
                        }
                    }
#endif

                    BakeGlobalReflectionProbe(false);

                    if (finaliseEnvironment)
                    {
                        SetupHDEnvironmentalVolume(true, true, false, true, false, false, false, renderPipelineSettings, "High Definition Wind", "High Definition Environment Volume", 1.5f, 2f, "Density Volume", "FFFFFF", 300f, new Vector3(0.1f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleDay", "FFF4D3", 110f, 2.05f, 1200f, -25f);
#if UNITY_2019_1_OR_NEWER
                        WaterSetup(renderPipelineSettings, true);
#else
                        WaterSetup(renderPipelineSettings, false);
#endif
                        FixPostProcessingV2(renderPipelineSettings);
                        DestroyParent("Ambient Skies Environment");
                    }

                    RenderPipelineAsset pipelineAssetHD = GraphicsSettings.renderPipelineAsset;
                    if (pipelineAssetHD != null)
                    {
                        EditorUtility.SetDirty(pipelineAssetHD);
                        AssetDatabase.SaveAssets();
                        SceneView.RepaintAll();
                    }

                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }

                    #endregion
                    break;
            }
        }

        /// <summary>
        /// Applies and creates the volume settings
        /// </summary>
        /// <param name="hdEnabled"></param>
        /// <param name="switchingPipeline"></param>
        /// <param name="setMorning"></param>
        /// <param name="setDay"></param>
        /// <param name="setEvening"></param>
        /// <param name="setNight"></param>
        /// <param name="setDefault"></param>
        /// <param name="renderPipelineSettings"></param>
        /// <param name="hdWind"></param>
        /// <param name="volumeName"></param>
        /// <param name="ambientLightAmount"></param>
        /// <param name="skyExposure"></param>
        /// <param name="densityVolumeName"></param>
        /// <param name="densityAlbedoColor"></param>
        /// <param name="densityFogDistance"></param>
        /// <param name="densityScrollSpeed"></param>
        /// <param name="hdVolumeProfile"></param>
        /// <param name="hdriSkyTexture"></param>
        /// <param name="sunColor"></param>
        /// <param name="sunRotation"></param>
        /// <param name="sunIntensity"></param>
        /// <param name="fogFarDistance"></param>
        /// <param name="fogNearDistance"></param>
        public static void SetupHDEnvironmentalVolume(bool hdEnabled, bool switchingPipeline, bool setMorning, bool setDay, bool setEvening, bool setNight, bool setDefault, GaiaConstants.EnvironmentRenderer renderPipelineSettings, string hdWind, string volumeName, float ambientLightAmount, float skyExposure, string densityVolumeName, string densityAlbedoColor, float densityFogDistance, Vector3 densityScrollSpeed, string hdVolumeProfile, string hdriSkyTexture, string sunColor, float sunRotation, float sunIntensity, float fogFarDistance, float fogNearDistance)
        {
            //High Definition
            if (renderPipelineSettings == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                if (hdEnabled)
                {
                    #if HDPipeline
                    //Get parent object
                    GameObject parentObject = GetOrCreateParentObject("Gaia Environment");

                    //Get the main directional light
                    GameObject lightObj = GetMainDirectionalLight();
                    if (lightObj != null)
                    {
                        Vector3 sunRotationAngle = new Vector3(0f, 0f, 0f);
                        sunRotationAngle.x += sunRotation;
                        lightObj.transform.SetPositionAndRotation(new Vector3(0f, 0f, 0f), Quaternion.Euler(sunRotationAngle));

                        Light light = lightObj.GetComponent<Light>();
                        if (light != null)
                        {
                            light.color = GetColorFromHTML(sunColor);
                        }

                        HDAdditionalLightData lightData = lightObj.GetComponent<HDAdditionalLightData>();
                        if (lightData != null)
                        {
                            lightData.useVolumetric = true;
                            lightData.lightUnit = LightUnit.Lux;
                            lightData.intensity = sunIntensity * 3.14f;
                        }

                        AdditionalShadowData additionalShadowData = lightObj.GetComponent<AdditionalShadowData>();
                        if (additionalShadowData != null)
                        {
                            additionalShadowData.contactShadows = true;
                            additionalShadowData.edgeLeakFixup = true;
                            additionalShadowData.shadowResolution = 2048;
                        }
                    }

                    //Hd Pipeline Volume Setup
                    GameObject volumeObject = GameObject.Find(volumeName);
                    if (volumeObject == null)
                    {
                        volumeObject = new GameObject(volumeName);
                        volumeObject.layer = LayerMask.NameToLayer("TransparentFX");
                        volumeObject.transform.SetParent(parentObject.transform);
                    }

                    Volume volumeSettings = volumeObject.GetComponent<Volume>();
                    if (volumeSettings == null)
                    {
                        volumeSettings = volumeObject.AddComponent<Volume>();
                        volumeSettings.isGlobal = true;
                        volumeSettings.blendDistance = 0f;
                        volumeSettings.weight = 1f;
                        volumeSettings.priority = 1f;
                    }
                    else
                    {
                        volumeSettings.isGlobal = true;
                        volumeSettings.blendDistance = 0f;
                        volumeSettings.weight = 1f;
                        volumeSettings.priority = 1f;
                    }

                    volumeSettings.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(hdVolumeProfile));
                    VolumeProfile volumeProfile = volumeSettings.sharedProfile;

                    if (!setDefault)
                    {
                        if (!string.IsNullOrEmpty(hdVolumeProfile))
                        {
                            if (volumeProfile != null)
                            {
                                //Marks the volume to be saved
                                EditorUtility.SetDirty(volumeProfile);

                                //Visual Enviro
                                VisualEnvironment visualEnvironmentSettings;
                                if (volumeProfile.TryGet(out visualEnvironmentSettings))
                                {
                                    visualEnvironmentSettings.skyType.value = 1;
                                    visualEnvironmentSettings.fogType.value = FogType.Volumetric;
                                }

                                HDRISky hDRISky;
                                if (volumeProfile.TryGet(out hDRISky))
                                {
                                    hDRISky.active = true;
                                    hDRISky.hdriSky.value = AssetDatabase.LoadAssetAtPath<Cubemap>(GetAssetPath(hdriSkyTexture));
                                    hDRISky.skyIntensityMode.value = SkyIntensityMode.Exposure;
                                    hDRISky.exposure.value = skyExposure;
                                    hDRISky.multiplier.value = 1f;
                                    hDRISky.rotation.value = 0f;

                                    hDRISky.updateMode.value = EnvironementUpdateMode.OnChanged;
                                }

                                HDShadowSettings hDShadowSettings;
                                if (volumeProfile.TryGet(out hDShadowSettings))
                                {
                                    hDShadowSettings.cascadeShadowSplitCount.value = 4;
                                    hDShadowSettings.maxShadowDistance.value = 700f;
                                }

                                LinearFog linearFog;
                                if (volumeProfile.TryGet(out linearFog))
                                {
                                    linearFog.active = false;
                                    linearFog.density.value = 1f;
                                    linearFog.fogStart.value = fogNearDistance;
                                    linearFog.fogEnd.value = fogFarDistance;
                                    linearFog.fogHeightStart.value = 100f;
                                    linearFog.fogHeightEnd.value = 800f;
                                    linearFog.maxFogDistance.value = 500f;
                                    linearFog.mipFogNear.value = 0f;
                                    linearFog.mipFogFar.value = 1000f;
                                    linearFog.mipFogMaxMip.value = 0.5f; ;
                                }

                                VolumetricFog volumetricFog;
                                if (volumeProfile.TryGet(out volumetricFog))
                                {
                                    volumetricFog.active = true;
                                    volumetricFog.albedo.value = new Color32(255, 255, 255, 255);
                                    volumetricFog.meanFreePath.value = fogFarDistance / 1.7f;
                                    volumetricFog.baseHeight.value = 80f;
                                    volumetricFog.meanHeight.value = 200f;
                                    volumetricFog.anisotropy.value = 0.75f;
                                    volumetricFog.globalLightProbeDimmer.value = 0.25f;
                                    volumetricFog.maxFogDistance.value = 1500f;
                                }

                                ExponentialFog exponentialFog;
                                if (volumeProfile.TryGet(out exponentialFog))
                                {
                                    exponentialFog.active = false;
                                }

                                ProceduralSky proceduralSky;
                                if (volumeProfile.TryGet(out proceduralSky))
                                {
                                    proceduralSky.active = false;
                                }

                                GradientSky gradientSky;
                                if (volumeProfile.TryGet(out gradientSky))
                                {
                                    gradientSky.active = false;
                                }

                                VolumetricLightingController volumetricLightingController;
                                if (volumeProfile.TryGet(out volumetricLightingController))
                                {
                                    volumetricLightingController.active = true;
                                    volumetricLightingController.depthExtent.value = 200f;
                                    volumetricLightingController.sliceDistributionUniformity.value = 0.5f;
                                }

                                IndirectLightingController indirectLightingController;
                                if (volumeProfile.TryGet(out indirectLightingController))
                                {
                                    indirectLightingController.active = true;
                                    indirectLightingController.indirectDiffuseIntensity.value = ambientLightAmount;
                                }

#if UNITY_2018_3_OR_NEWER
                                MicroShadowing microShadowing;
                                if (volumeProfile.TryGet(out microShadowing))
                                {
                                    microShadowing.active = true;
                                }
#endif
                            }
                        }
                    }
                    else
                    {
                        //Marks the volume to be saved
                        EditorUtility.SetDirty(volumeProfile);

                        //Visual Enviro
                        VisualEnvironment visualEnvironmentSettings;
                        if (volumeProfile.TryGet(out visualEnvironmentSettings))
                        {
                            visualEnvironmentSettings.skyType.value = 2;
                            visualEnvironmentSettings.fogType.value = FogType.Volumetric;
                        }

                        HDRISky hDRISky;
                        if (volumeProfile.TryGet(out hDRISky))
                        {
                            hDRISky.active = false;

                            hDRISky.updateMode.value = EnvironementUpdateMode.OnChanged;
                        }

                        HDShadowSettings hDShadowSettings;
                        if (volumeProfile.TryGet(out hDShadowSettings))
                        {
                            hDShadowSettings.cascadeShadowSplitCount.value = 4;
                            hDShadowSettings.maxShadowDistance.value = 700f;
                        }

                        LinearFog linearFog;
                        if (volumeProfile.TryGet(out linearFog))
                        {
                            linearFog.active = false;
                        }

                        VolumetricFog volumetricFog;
                        if (volumeProfile.TryGet(out volumetricFog))
                        {
                            volumetricFog.active = true;
                            volumetricFog.meanFreePath.value = fogFarDistance / 2.7f;
                            volumetricFog.baseHeight.value = 40f;
                            volumetricFog.meanHeight.value = 100f;
                            volumetricFog.anisotropy.value = 0.75f;
                            volumetricFog.globalLightProbeDimmer.value = 0.8f;
                            volumetricFog.maxFogDistance.value = 1500f;
                        }

                        ExponentialFog exponentialFog;
                        if (volumeProfile.TryGet(out exponentialFog))
                        {
                            exponentialFog.active = false;
                        }

                        ProceduralSky proceduralSky;
                        if (volumeProfile.TryGet(out proceduralSky))
                        {
                            proceduralSky.active = true;
                            proceduralSky.exposure.value = skyExposure;
                            proceduralSky.sunSize.value = 0.01f;
                            proceduralSky.sunSizeConvergence.value = 10f;
                        }

                        GradientSky gradientSky;
                        if (volumeProfile.TryGet(out gradientSky))
                        {
                            gradientSky.active = false;
                        }

                        VolumetricLightingController volumetricLightingController;
                        if (volumeProfile.TryGet(out volumetricLightingController))
                        {
                            volumetricLightingController.active = true;
                            volumetricLightingController.depthExtent.value = 200f;
                            volumetricLightingController.sliceDistributionUniformity.value = 0.5f;
                        }

                        IndirectLightingController indirectLightingController;
                        if (volumeProfile.TryGet(out indirectLightingController))
                        {
                            indirectLightingController.active = true;
                            indirectLightingController.indirectDiffuseIntensity.value = 1f;
                        }

#if UNITY_2018_3_OR_NEWER
                        MicroShadowing microShadowing;
                        if (volumeProfile.TryGet(out microShadowing))
                        {
                            microShadowing.active = true;
                        }
#endif
                    }

                    GameObject densityVolume = GameObject.Find(densityVolumeName);
                    if (densityVolume == null)
                    {
                        densityVolume = new GameObject(densityVolumeName);
                        densityVolume.transform.position = new Vector3(0f, 50f, 0f);
                        densityVolume.AddComponent<DensityVolume>();
                        densityVolume.transform.SetParent(parentObject.transform);
                    }

                    Terrain terrain = GetActiveTerrain();
                    DensityVolume densitySettings = densityVolume.GetComponent<DensityVolume>();
                    if (densitySettings == null)
                    {
                        densitySettings = densityVolume.AddComponent<DensityVolume>();
                        densitySettings.parameters.albedo = GetColorFromHTML(densityAlbedoColor);
                        densitySettings.parameters.meanFreePath = densityFogDistance;
                        if (terrain != null)
                        {
                            densitySettings.parameters.size = new Vector3(terrain.terrainData.size.x, 150f, terrain.terrainData.size.z);
                        }
                        else
                        {
                            densitySettings.parameters.size = new Vector3(2000f, 150f, 2000f);
                        }

                        densitySettings.parameters.volumeMask = AssetDatabase.LoadAssetAtPath<Texture3D>(GetAssetPath("Fog Noise Texture 3D"));
                        densitySettings.parameters.textureScrollingSpeed = densityScrollSpeed;
                        densitySettings.parameters.textureTiling = new Vector3(10f, 1f, 10f);
                    }
                    else
                    {
                        densitySettings.parameters.albedo = GetColorFromHTML(densityAlbedoColor);
                        densitySettings.parameters.meanFreePath = densityFogDistance;
                        if (terrain != null)
                        {
                            densitySettings.parameters.size = new Vector3(terrain.terrainData.size.x, 150f, terrain.terrainData.size.z);
                        }
                        else
                        {
                            densitySettings.parameters.size = new Vector3(2000f, 150f, 2000f);
                        }

                        densitySettings.parameters.volumeMask = AssetDatabase.LoadAssetAtPath<Texture3D>(GetAssetPath("Fog Noise Texture 3D"));
                        densitySettings.parameters.textureScrollingSpeed = densityScrollSpeed;
                        densitySettings.parameters.textureTiling = new Vector3(10f, 1f, 10f);
                    }

                    //Baking Sky Setup
#if !UNITY_2019_1_OR_NEWER
                    BakingSky bakingSkySettings = volumeObject.GetComponent<BakingSky>();
                    if (bakingSkySettings == null)
                    {
                        bakingSkySettings = volumeObject.AddComponent<BakingSky>();
                        if (!setDefault)
                        {
                            bakingSkySettings.bakingSkyUniqueID = 1;
                        }
                        else
                        {
                            bakingSkySettings.bakingSkyUniqueID = 2;
                        }
                    }
                    else
                    {
                        bakingSkySettings.profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(hdVolumeProfile));

                        if (!setDefault)
                        {
                            bakingSkySettings.bakingSkyUniqueID = 1;
                        }
                        else
                        {
                            bakingSkySettings.bakingSkyUniqueID = 2;
                        }
                    }
#else
                    StaticLightingSky bakingSkySettings = volumeObject.GetComponent<StaticLightingSky>();
                    if (bakingSkySettings == null)
                    {
                        bakingSkySettings = volumeObject.AddComponent<StaticLightingSky>();
                        if (!setDefault)
                        {
                            bakingSkySettings.staticLightingSkyUniqueID = 1;
                        }
                        else
                        {
                            bakingSkySettings.staticLightingSkyUniqueID = 2;
                        }
                    }
                    else
                    {
                        bakingSkySettings.profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(hdVolumeProfile));

                        if (!setDefault)
                        {
                            bakingSkySettings.staticLightingSkyUniqueID = 1;
                        }
                        else
                        {
                            bakingSkySettings.staticLightingSkyUniqueID = 2;
                        }
                    }
#endif

                    GameObject hdrspWind = GameObject.Find(hdWind);
                    if (hdrspWind == null)
                    {
                        hdrspWind = new GameObject(hdWind);
                        hdrspWind.transform.SetParent(parentObject.transform);
                        ShaderWindSettings windSettings = hdrspWind.AddComponent<ShaderWindSettings>();
                        windSettings.WindSpeed = 40f;
                        windSettings.Turbulence = 0.25f;
                        windSettings.NoiseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetAssetPath("FogNoise"));
                        windSettings.FlexNoiseWorldSize = 150f;
                        windSettings.ShiverNoiseWorldSize = 15f;
                        windSettings.GustMaskTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetAssetPath("GustNoise"));
                        windSettings.GustWorldSize = 500f;
                        windSettings.GustSpeed = 20f;
                        windSettings.GustScale = 1f;
                    }
                    else
                    {
                        ShaderWindSettings windSettings = hdrspWind.GetComponent<ShaderWindSettings>();
                        if (windSettings != null)
                        {
                            windSettings.WindSpeed = 40f;
                            windSettings.Turbulence = 0.25f;
                            windSettings.NoiseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetAssetPath("FogNoise"));
                            windSettings.FlexNoiseWorldSize = 150f;
                            windSettings.ShiverNoiseWorldSize = 15f;
                            windSettings.GustMaskTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetAssetPath("GustNoise"));
                            windSettings.GustWorldSize = 500f;
                            windSettings.GustSpeed = 20f;
                            windSettings.GustScale = 1f;
                        }
                        else
                        {
                            windSettings = hdrspWind.AddComponent<ShaderWindSettings>();
                            windSettings.WindSpeed = 40f;
                            windSettings.Turbulence = 0.25f;
                            windSettings.NoiseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetAssetPath("FogNoise"));
                            windSettings.FlexNoiseWorldSize = 150f;
                            windSettings.ShiverNoiseWorldSize = 15f;
                            windSettings.GustMaskTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetAssetPath("GustNoise"));
                            windSettings.GustWorldSize = 500f;
                            windSettings.GustSpeed = 20f;
                            windSettings.GustScale = 1f;
                        }
                    }
#endif
                }
                else if (renderPipelineSettings == GaiaConstants.EnvironmentRenderer.HighDefinition2018x && switchingPipeline && setDay)
                {
                    SetupHDEnvironmentalVolume(true, false, false, true, false, false, false, renderPipelineSettings, "High Definition Wind", "High Definition Environment Volume", 1.5f, 2f, "Density Volume", "FFFFFF", 300f, new Vector3(0.1f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleDay", "FFF4D3", 110f, 2.05f, 1200f, -25f);
                }
                else
                {
                    GameObject volumeObject = GameObject.Find(volumeName);
                    if (volumeObject != null)
                    {
                        Object.DestroyImmediate(volumeObject);
                    }

                    GameObject densityVolume = GameObject.Find(densityVolumeName);
                    if (densityVolume != null)
                    {
                        Object.DestroyImmediate(densityVolume);
                    }          
                }

#if UNITY_2019_1_OR_NEWER
                Apply2019HDRPPostFX("Gaia HDRP Post Processing", "Gaia HDRP Post Processing Profile");
#endif
            }
            else
            {
                GameObject environmentVolume = GameObject.Find(volumeName);
                if (environmentVolume != null)
                {
                    Object.DestroyImmediate(environmentVolume);
                }

                GameObject densityVolume = GameObject.Find(densityVolumeName);
                if (densityVolume != null)
                {
                    Object.DestroyImmediate(densityVolume);
                }

                GameObject hdWindSettings = GameObject.Find(hdWind);
                if (hdWindSettings != null)
                {
                    Object.DestroyImmediate(hdWindSettings);
                }

                GameObject hdrpPostFX = GameObject.Find("Gaia HDRP Post Processing");
                if (hdrpPostFX != null)
                {
                    Object.DestroyImmediate(hdrpPostFX);
                }

                if (switchingPipeline && setDay)
                {                   
                    if (renderPipelineSettings != GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
                    {
#if UNITY_2018_1_OR_NEWER
                        SetHDRISky(new Vector3(70f, 180f, 0f), "FFF6E9FF", 2.2f, "Ambient Skies Sample Sky", "AmbientSkiesSampleDay", "5A5A5AFF", 1.6f, 0f, "CEE2E9", -50f, 1.6f, "B7C1C9", "A9A395", "8E8166");
#else
                        SetHDRISky(new Vector3(70f, 180f, 0f), "FFF6E9FF", 1.2f, "Ambient Skies Sample Sky", "AmbientSkiesSampleDay", "5A5A5AFF", 1.6f, 0f, "CEE2E9", -50f, 1f, "D4DBEC", "EADCBC", "DFD9D9");
#endif
                    }
                }
            }

            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        /// <summary>
        /// Apply 2019 HDRP Post Processing
        /// </summary>
        /// <param name="postProcessVolumeObject"></param>
        /// <param name="postProcessVolumeProfile"></param>
        public static void Apply2019HDRPPostFX(string postProcessVolumeObject, string postProcessVolumeProfile)
        {
#if HDPipeline
            GameObject processObject = GameObject.Find(postProcessVolumeObject);
            GameObject parentObject = GetOrCreateEnvironmentParent();
            if (processObject == null)
            {
                processObject = new GameObject(postProcessVolumeObject);
                processObject.layer = LayerMask.NameToLayer("TransparentFX");
                processObject.transform.SetParent(parentObject.transform);
            }
            else
            {
                processObject.layer = LayerMask.NameToLayer("TransparentFX");
                processObject.transform.SetParent(parentObject.transform);
            }

            Volume processVolume = processObject.GetComponent<Volume>();
            if (processVolume == null)
            {
                processVolume = processObject.AddComponent<Volume>();
                processVolume.isGlobal = true;
                processVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(postProcessVolumeProfile));
            }
            else
            {
                processVolume.isGlobal = true;
                processVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(postProcessVolumeProfile));
            }

            GameObject postprocessVolumeOld = GameObject.Find("Global Post Processing");
            if (postprocessVolumeOld != null)
            {
                Object.DestroyImmediate(postprocessVolumeOld);
            }
            #if UNITY_POST_PROCESSING_STACK_V2

            PostProcessLayer processLayer = Object.FindObjectOfType<PostProcessLayer>();
            if (processLayer != null)
            {
                Object.DestroyImmediate(processLayer);
            }
            #endif
#endif
        }

        /// <summary>
        /// Apply Resources to HD pipeline
        /// </summary>
        /// <param name="renderPipelineSettings"></param>
        /// <param name="renderPipelineAsset"></param>
        public static void ApplyHDPipelineResources(GaiaConstants.EnvironmentRenderer renderPipelineSettings, string renderPipelineAsset)
        {
            RenderPipelineAsset hdRenderPipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(GetAssetPath(renderPipelineAsset));
            if (hdRenderPipelineAsset != null)
            {
#if HDPipeline
                HDRenderPipelineAsset pipelineSettings = AssetDatabase.LoadAssetAtPath<HDRenderPipelineAsset>(GetAssetPath(renderPipelineAsset));
                if (pipelineSettings != null)
                {
                    if (pipelineSettings.renderPipelineResources == null)
                    {
                        pipelineSettings.renderPipelineResources = AssetDatabase.LoadAssetAtPath<RenderPipelineResources>(GetAssetPath("HDRenderPipelineResources"));
                    }
                    if (pipelineSettings.renderPipelineEditorResources == null)
                    {
                        pipelineSettings.renderPipelineEditorResources = AssetDatabase.LoadAssetAtPath<HDRenderPipelineEditorResources>(GetAssetPath("HDRenderPipelineEditorResources"));
                    }
#if UNITY_2019_1_OR_NEWER
                    if (pipelineSettings.diffusionProfileSettingsList == null)
                    {
                        string assetPath = GetAssetPath("Procedural Worlds Diffusion Profile Settings");
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            pipelineSettings.diffusionProfileSettingsList[0] = AssetDatabase.LoadAssetAtPath<DiffusionProfileSettings>(GetAssetPath(assetPath));
                        }
                    }
#elif UNITY_2018_3_OR_NEWER
                    if (pipelineSettings.diffusionProfileSettings == null)
                    {
                        pipelineSettings.diffusionProfileSettings = AssetDatabase.LoadAssetAtPath<DiffusionProfileSettings>(GetAssetPath("Procedural Worlds Diffusion Profile Settings"));
                    } 
#endif
                }
            EditorUtility.SetDirty(pipelineSettings);
#endif
            }
        }

        /// <summary>
        /// Sets up the water correctly to the pipeline required
        /// </summary>
        /// <param name="renderPipelineSettings"></param>
        public static void WaterSetup(GaiaConstants.EnvironmentRenderer renderPipelineSettings, bool planarReflections)
        {
            //Get relevant information
#if HDPipeline
            GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
            GameObject parentObject = GetOrCreateEnvironmentParent();
            Terrain activeTerrain = GetActiveTerrain();
#endif
            if (renderPipelineSettings == GaiaConstants.EnvironmentRenderer.HighDefinition2018x && planarReflections)
            {
#if HDPipeline
                GameObject planarReflectionProbeObject = GameObject.Find("Water Planar Reflections");
                if (planarReflectionProbeObject == null)
                {
                    planarReflectionProbeObject = new GameObject("Water Planar Reflections");
                    planarReflectionProbeObject.transform.position = new Vector3(0f, sceneInfo.m_seaLevel + 0.1f, 0f);
                    planarReflectionProbeObject.transform.SetParent(parentObject.transform);

                    PlanarReflectionProbe planar = planarReflectionProbeObject.AddComponent<PlanarReflectionProbe>();
                    if (activeTerrain != null)
                    {
                        planar.influenceVolume.boxSize = new Vector3(activeTerrain.terrainData.size.x * 10f, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z * 10f);
                    }
                    else
                    {
                        planar.influenceVolume.boxSize = new Vector3(2000f, 1000f, 2000f);
                    }

                    GaiaHDPlanarReflections gaiaHDPlanar = Object.FindObjectOfType<GaiaHDPlanarReflections>();
                    if (gaiaHDPlanar == null)
                    {
                        GameObject waterPlanar = GameObject.Find("Water Planar Reflections");
                        if (waterPlanar != null)
                        {
                            gaiaHDPlanar = waterPlanar.AddComponent<GaiaHDPlanarReflections>();
                        }
                    }
                }
                else
                {
                    planarReflectionProbeObject.transform.position = new Vector3(0f, sceneInfo.m_seaLevel + 0.1f, 0f);
                    PlanarReflectionProbe planar = planarReflectionProbeObject.GetComponent<PlanarReflectionProbe>();
                    if (planar == null)
                    {
                        planar = planarReflectionProbeObject.AddComponent<PlanarReflectionProbe>();
                        if (activeTerrain != null)
                        {
                            planar.influenceVolume.boxSize = new Vector3(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z);
                        }
                        else
                        {
                            planar.influenceVolume.boxSize = new Vector3(2000f, 1000f, 2000f);
                        }
                    }
                    else
                    {
                        if (activeTerrain != null)
                        {
                            planar.influenceVolume.boxSize = new Vector3(activeTerrain.terrainData.size.x * 10f, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z * 10f);
                        }
                        else
                        {
                            planar.influenceVolume.boxSize = new Vector3(2500f, 1000f, 2000f);
                        }
                    }
                }

                GameObject theReflectionProbeObject = GameObject.Find("Camera Reflection Probe");
                if (theReflectionProbeObject != null)
                {
                    Object.DestroyImmediate(theReflectionProbeObject);
                }

                GameObject globalProbe = GameObject.Find("Global Reflection Probe");
                if (globalProbe != null)
                {
                    Object.DestroyImmediate(globalProbe);
                }
#endif
            }
            else if (renderPipelineSettings == GaiaConstants.EnvironmentRenderer.HighDefinition2018x && !planarReflections)
            {
                GameObject globalProbe = GameObject.Find("Global Reflection Probe");
                if (globalProbe == null)
                {
                    GetOrCreateReflectionProbe("Global Reflection Probe");
                }

                GameObject planarReflectionProbeObject = GameObject.Find("Water Planar Reflections");
                if (planarReflectionProbeObject != null)
                {
                    Object.DestroyImmediate(planarReflectionProbeObject);
                }
            }
            else
            {
                bool useCameraReflectionProbe = false;
                GameObject theReflectionProbeObject = GameObject.Find("Camera Reflection Probe");
                if (useCameraReflectionProbe)
                {
                    //Adds reflection probe updater script if missing               
                    if (theReflectionProbeObject == null)
                    {
                        GetOrCreateReflectionProbe("Camera Reflection Probe");

                        GameObject reflectionProbeObject = GameObject.Find("Camera Reflection Probe");
                        if (reflectionProbeObject != null)
                        {
                            GaiaReflectionProbeUpdate probeObject = reflectionProbeObject.AddComponent<GaiaReflectionProbeUpdate>();
                            probeObject.m_followCamera = true;
                            probeObject.SetProbeSettings();
                        }
                    }
                    else
                    {
                        if (theReflectionProbeObject.GetComponent<GaiaReflectionProbeUpdate>() == null)
                        {
                            GaiaReflectionProbeUpdate probeObject = theReflectionProbeObject.AddComponent<GaiaReflectionProbeUpdate>();
                            probeObject.m_followCamera = true;
                            probeObject.SetProbeSettings();
                        }
                    }
                }
                else
                {
                    if (theReflectionProbeObject != null)
                    {
                        Object.DestroyImmediate(theReflectionProbeObject);
                    }
                }
               
                GameObject globalProbe = GameObject.Find("Global Reflection Probe");
                if (globalProbe == null)
                {
                    GetOrCreateReflectionProbe("Global Reflection Probe");
                }

                GameObject planarReflectionProbeObject = GameObject.Find("Water Planar Reflections");
                if (planarReflectionProbeObject != null)
                {
                    Object.DestroyImmediate(planarReflectionProbeObject);
                }
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get or create the main scene camera
        /// </summary>
        /// <returns>The gameobject camera</returns>
        public static GameObject GetOrCreateMainCamera()
        {
            GameObject mainCameraObject = GameObject.Find("Main Camera");
            if (mainCameraObject != null)
            {
                return mainCameraObject;
            }

            mainCameraObject = GameObject.Find("Camera");
            if (mainCameraObject != null)
            {
                return mainCameraObject;
            }

            mainCameraObject = GameObject.Find("FirstPersonCharacter");
            if (mainCameraObject != null)
            {
                return mainCameraObject;
            }

            mainCameraObject = GameObject.Find("FlyCam");
            if (mainCameraObject != null)
            {
                return mainCameraObject;
            }

            if (Camera.main != null)
            {
                mainCameraObject = Camera.main.gameObject;
                return mainCameraObject;
            }

            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (var camera in cameras)
            {
                return camera.gameObject;
            }

            //Create new camera
            mainCameraObject = new GameObject("Main Camera");
            mainCameraObject.AddComponent<Camera>();
#if !UNITY_2017_1_OR_NEWER
            mainCameraObject.AddComponent<GUILayer>();
#endif
            mainCameraObject.AddComponent<FlareLayer>();
            mainCameraObject.AddComponent<AudioListener>();
            mainCameraObject.tag = "MainCamera";

            return mainCameraObject;
        }

        /// <summary>
        /// Sets up the post processing profiles and layers to the appropiate pipeline setup
        /// </summary>
        /// <param name="renderPipelineSettings"></param>
        public static void FixPostProcessingV2(GaiaConstants.EnvironmentRenderer renderPipelineSettings)
        {
            #if UNITY_POST_PROCESSING_STACK_V2
            PostProcessProfile[] processProfiles = Resources.FindObjectsOfTypeAll<PostProcessProfile>();
            if (processProfiles != null)
            {
                if (renderPipelineSettings == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                {
                    foreach (PostProcessProfile profiles in processProfiles)
                    {
                        UnityEngine.Rendering.PostProcessing.MotionBlur motionBlur;

                        if (profiles.TryGetSettings(out motionBlur))
                        {
                            motionBlur.active = false;
                        }
                    }
                }
                else
                {
                    foreach (PostProcessProfile profiles in processProfiles)
                    {
                        UnityEngine.Rendering.PostProcessing.MotionBlur motionBlur;
                        if (profiles.TryGetSettings(out motionBlur))
                        {
                            motionBlur.active = true;
                        }
                    }
                }

                PostProcessLayer[] processLayers = Object.FindObjectsOfType<PostProcessLayer>();
                if (processLayers  != null)
                {
                    Camera[] camera = Object.FindObjectsOfType<Camera>();
                    if (renderPipelineSettings == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                    {
                        foreach (PostProcessLayer layers in processLayers)
                        {
                            layers.antialiasingMode = PostProcessLayer.Antialiasing.None;
                        }
                        if (camera != null)
                        {
                            foreach (Camera mainCams in camera)
                            {
                                mainCams.allowMSAA = true;
                                mainCams.allowHDR = false;
                            }
                        }
                    }
                    else
                    {
                        foreach (PostProcessLayer layers in processLayers)
                        {
                            layers.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                        }
                        if (camera != null)
                        {
                            foreach (Camera mainCams in camera)
                            {
                                mainCams.allowMSAA = false;
                                mainCams.allowHDR = true;
                            }
                        }
                    }

                }
            }
#endif
        }

        /// <summary>
        /// Gets or creates a reflection probe
        /// </summary>
        private static void GetOrCreateReflectionProbe(string probeName)
        {
            GameObject theParentObject = GetOrCreateEnvironmentParent();
            GameObject reflectionProbeObject = GameObject.Find(probeName);
            if (reflectionProbeObject == null)
            {
                reflectionProbeObject = new GameObject(probeName);
                reflectionProbeObject.transform.parent = theParentObject.transform;

                ReflectionProbe probe = reflectionProbeObject.AddComponent<ReflectionProbe>();
                probe.importance = 1;
                probe.intensity = 1f;
                probe.blendDistance = 0f;
                probe.resolution = 64;
                probe.shadowDistance = 50f;
                probe.clearFlags = ReflectionProbeClearFlags.Skybox;
                probe.mode = ReflectionProbeMode.Realtime;
                probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
                probe.refreshMode = ReflectionProbeRefreshMode.OnAwake;
                probe.cullingMask = 0;

                Terrain theTerrain = GetActiveTerrain();
                if (theTerrain != null)
                {
                    probe.size = new Vector3(theTerrain.terrainData.size.x, theTerrain.terrainData.size.y, theTerrain.terrainData.size.z);
                    probe.farClipPlane = theTerrain.terrainData.size.x;
                    Vector3 probeLocation = new Vector3(0f, 0f, 0f);
                    probeLocation.y = theTerrain.SampleHeight(probeLocation) + 50f;
                    reflectionProbeObject.transform.localPosition = probeLocation;
                }
                else
                {
                    probe.size = new Vector3(3000f, 1500f, 3000f);
                    probe.farClipPlane = 3000f;
                    reflectionProbeObject.transform.localPosition = new Vector3(0f, 250f, 0f);
                }
            }
        }


        /// <summary>
        /// Gets and return water material
        /// </summary>
        /// <returns></returns>
        public static Material GetWaterMaterial()
        {
            Material waterMat = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath("Ambient Water Sample Material"));
            if (waterMat == null)
            {
                waterMat = GameObject.Find("Ambient Water Sample").GetComponent<MeshRenderer>().sharedMaterial;
                if (waterMat != null)
                {
                    return waterMat;
                }
                else
                {
                    Material[] materials = Object.FindObjectsOfType<Material>();
                    foreach (Material mat in materials)
                    {
                        if (Shader.Find("Simple Water Sample"))
                        {
                            waterMat = mat;
                            return waterMat;
                        }
                        else if (Shader.Find("Simple Water Sample LW"))
                        {
                            waterMat = mat;
                            return waterMat;
                        }
                        else if (Shader.Find("Simple Water Sample HD"))
                        {
                            waterMat = mat;
                            return waterMat;
                        }

                        return null;
                    }
                }
            }            
            else
            {
                return waterMat;
            }

            return null;
        }

        /// <summary>
        /// Return the first scriptable that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the prefab or null</returns>
        public static ScriptableObject GetAssetScriptableObject(string name)
        {
            string path = GetAssetPath(name, "asset");
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }
            return null;
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <param name="name">Type to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string name, string type)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            string[] file;
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                //Make sure its an exact match
                file = Path.GetFileName(path).Split('.');
                if (file.GetLength(0) != 2)
                {
                    continue;
                }
                if (file[0] != name)
                {
                    continue;
                }
                if (file[1] != type)
                {
                    continue;
                }
                return path;
            }
            return "";
        }

        /// <summary>
        /// Get the currently active terrain - or any terrain
        /// </summary>
        /// <returns>A terrain if there is one</returns>
        public static Terrain[] GetActiveTerrains()
        {
            //Grab active terrains if we can
            Terrain[] terrain = Terrain.activeTerrains;
            if (terrain != null)
            {
                return terrain;
            }

            return null;
        }

        /// <summary>
        /// Bake our global reflection probe - if no lighting baked yet and requested then a global bake is kicked off
        /// </summary>
        /// <param name="doGlobalBakeIfNecessary">If no previous bake has been done then a global bake will be kicked off</param>
        public static void BakeGlobalReflectionProbe(bool doGlobalBakeIfNecessary)
        {
            if (Lightmapping.isRunning)
            {
                return;
            }

            //Get global reflection probe
            ReflectionProbe[] reflectionProbes = Object.FindObjectsOfType<ReflectionProbe>();
            if (reflectionProbes == null || reflectionProbes.Length == 0)
            {
                return;
            }

            GameObject reflectionProbeObject = GameObject.Find("Global Reflection Probe");
            if (reflectionProbeObject != null)
            {
                var probe = reflectionProbeObject.GetComponent<ReflectionProbe>();
                if (probe.mode == ReflectionProbeMode.Baked)
                {

                    if (probe.bakedTexture == null)
                    {
                        if (doGlobalBakeIfNecessary)
                        {
                            BakeGlobalLighting();
                        }
                        return;
                    }

                    Lightmapping.BakeReflectionProbe(probe, AssetDatabase.GetAssetPath(probe.bakedTexture));
                }
                else
                {
                    probe.RenderProbe();
                }
            }
        }

        /// <summary>
        /// Bakes global lighting
        /// </summary>
        public static void BakeGlobalLighting()
        {
            //Bakes the lightmaps
            if (!Application.isPlaying && !Lightmapping.isRunning)
            {
                Lightmapping.BakeAsync();
            }
        }

        /// <summary>
        /// Get the currently active terrain - or any terrain
        /// </summary>
        /// <returns>A terrain if there is one</returns>
        public static Terrain GetActiveTerrain()
        {
            //Grab active terrains if we can
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                return terrain;
            }

            return null;
        }

        /// <summary>
        /// Set linear deffered lighting (the best for outdoor scenes)
        /// </summary>
        public static void SetLinearDeferredLighting()
        {
            PlayerSettings.colorSpace = ColorSpace.Linear;
#if UNITY_5_5_OR_NEWER
            var tier1 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1);
            tier1.renderingPath = RenderingPath.DeferredShading;
            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1, tier1);
            var tier2 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2);
            tier2.renderingPath = RenderingPath.DeferredShading;
            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2, tier2);
            var tier3 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3);
            tier3.renderingPath = RenderingPath.DeferredShading;
            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3, tier3);
#else
            PlayerSettings.renderingPath = RenderingPath.DeferredShading;
#endif
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns>The path or null</returns>
        public static string GetAssetPath(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            return null;
        }

        /// <summary>
        /// Find parent object and destroys it if it's empty
        /// </summary>
        /// <param name="parentGameObject"></param>
        public static void DestroyParent(string parentGameObject)
        {
            //If string isn't empty
            if (!string.IsNullOrEmpty(parentGameObject))
            {
                //If string doesn't = Ambient Skies Environment
                if (parentGameObject != "Gaia Environment")
                {
                    //Sets the paramater to Ambient Skies Environment
                    parentGameObject = "Gaia Environment";
                }

                //Find parent object
                GameObject parentObject = GameObject.Find(parentGameObject);
                if (parentObject != null)
                {
                    //Find parents in parent object
                    Transform[] parentChilds = parentObject.GetComponentsInChildren<Transform>();
                    if (parentChilds.Length == 1)
                    {
                        //Destroy object if object is empty
                        Object.DestroyImmediate(parentObject);
                    }
                }
            }
        }

        /// <summary>
        /// Get the main directional light in the scene
        /// </summary>
        /// <returns>Main light or null</returns>
        public static GameObject GetMainDirectionalLight()
        {
            GameObject lightObj = GameObject.Find("Directional Light");
            if (lightObj == null)
            {
                //Grab the first directional light we can find
                Light[] lights = Object.FindObjectsOfType<Light>();
                foreach (var light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        lightObj = light.gameObject;
                        break;
                    }
                }
            }
            return lightObj;
        }

        /// <summary>
        /// Get or create a parent object
        /// </summary>
        /// <param name="parentGameObject">Name of the parent object to get or create</param>
        /// <returns>Parent objet</returns>
        public static GameObject GetOrCreateParentObject(string parentGameObject)
        {
            //Get the parent object
            GameObject theParentGo = GameObject.Find(parentGameObject);

            if (theParentGo == null)
            {
                theParentGo = GameObject.Find("Ambient Skies Environment");

                if (theParentGo == null)
                {
                    theParentGo = new GameObject(parentGameObject);
                }
            }

            return theParentGo;
        }

        /// Get a color from a html string
        /// </summary>
        /// <param name="htmlString">Color in RRGGBB or RRGGBBBAA or #RRGGBB or #RRGGBBAA format.</param>
        /// <returns>Color or white if unable to parse it.</returns>
        public static Color GetColorFromHTML(string htmlString)
        {
            Color color = Color.white;
            if (!htmlString.StartsWith("#"))
            {
                htmlString = "#" + htmlString;
            }
            if (!ColorUtility.TryParseHtmlString(htmlString, out color))
            {
                color = Color.white;
            }
            return color;
        }

        /// <summary>
        /// Setup the sky settings using trilight ambient and skybox ambient
        /// </summary>
        /// <param name="sunRotation"></param>
        /// <param name="sunColor"></param>
        /// <param name="sunIntensity"></param>
        /// <param name="skyMaterial"></param>
        /// <param name="hdrSkyTexture"></param>
        /// <param name="skyTint"></param>
        /// <param name="skyExposure"></param>
        /// <param name="skyRotation"></param>
        /// <param name="fogColor"></param>
        /// <param name="fogStartDistance"></param>
        /// <param name="skyGroundIntensity"></param>
        /// <param name="skyColor"></param>
        /// <param name="equatorColor"></param>
        /// <param name="groundColor"></param>
        private static void SetHDRISky(Vector3 sunRotation, string sunColor, float sunIntensity, string skyMaterial, string hdrSkyTexture, string skyTint, float skyExposure, float skyRotation, string fogColor, float fogStartDistance, float skyGroundIntensity, string skyColor, string equatorColor, string groundColor)
        {
            GameObject parentObject = GetOrCreateEnvironmentParent();
            GameObject lightObject = GetOrCreateDirectionalLight();

            lightObject.transform.parent = parentObject.transform;
            lightObject.transform.localRotation = Quaternion.Euler(sunRotation);
            Light light = lightObject.GetComponent<Light>();
            if (light != null)
            {
                light.color = GetColorFromHTML(sunColor);
                light.intensity = sunIntensity;
                RenderSettings.sun = light;
            }

            //Set the skybox material
            string skyMatPath = GetAssetPath(skyMaterial);
            string hdrSkyPath = GetAssetPath(hdrSkyTexture);
            if (!string.IsNullOrEmpty(skyMatPath) && !string.IsNullOrEmpty(hdrSkyPath))
            {
                Material hdrSkyMaterial = AssetDatabase.LoadAssetAtPath<Material>(skyMatPath);
                hdrSkyMaterial.SetColor("_Tint", GetColorFromHTML(skyTint));
                hdrSkyMaterial.SetFloat("_Exposure", skyExposure);
                hdrSkyMaterial.SetFloat("_Rotation", skyRotation);
                hdrSkyMaterial.SetTexture("_Tex", AssetDatabase.LoadAssetAtPath<Texture>(hdrSkyPath));
                RenderSettings.skybox = hdrSkyMaterial;
            }

            //Set render settings
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = GetColorFromHTML(skyColor);
            RenderSettings.ambientEquatorColor = GetColorFromHTML(equatorColor);
            RenderSettings.ambientGroundColor = GetColorFromHTML(groundColor);
            RenderSettings.fog = true;
            RenderSettings.fogColor = GetColorFromHTML(fogColor);
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = -fogStartDistance;

            //Gets far clip plane distance from camera
            float fogDistance = GetOrCreateMainCamera().GetComponent<Camera>().farClipPlane;
            //If fogDistance is below the min clamp value it's default to 1500
            if (fogDistance < 250)
            {
                RenderSettings.fogEndDistance = 1500f;
            }
            //If fogDistance great or equals to min clamp value it'll assign the value fogDistance and clamp it
            else
            {
                RenderSettings.fogEndDistance = Mathf.Clamp(fogDistance, 250f, 2300f);
            }

            GaiaReflectionProbeUpdate[] rpuArray = Object.FindObjectsOfType<GaiaReflectionProbeUpdate>();
            foreach (GaiaReflectionProbeUpdate rpu in rpuArray)
            {
                if (rpu.gameObject.name == "Camera Reflection Probe")
                {
                    rpu.SetProbeSettings();
                }
            }

            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        /// <summary>
        /// Get or create a directional light
        /// </summary>
        /// <returns>First directional light it finds, or a new one</returns>
        private static GameObject GetOrCreateDirectionalLight()
        {
            GameObject lightObject = GameObject.Find("Directional Light");
            if (lightObject == null)
            {
                //Check to see if we have one in the scene
                Light[] lights = GameObject.FindObjectsOfType<Light>();
                foreach (var light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        return light.gameObject;
                    }
                }

                //Create a new one
                lightObject = new GameObject("Directional Light");
                Light newLight = lightObject.AddComponent<Light>();
                newLight.type = LightType.Directional;
                newLight.shadows = LightShadows.Soft;
            }
            return lightObject;
        }

        /// <summary>
        /// Get or create the environment parent object - all environment stuff added to this
        /// </summary>
        /// <returns>Existing or new environment parent</returns>
        private static GameObject GetOrCreateEnvironmentParent()
        {
            GameObject parent = GameObject.Find("Gaia Environment");
            if (parent == null)
            {
                parent = new GameObject("Gaia Environment");
            }
            return parent;
        }
        #endregion
    }
}
#endif