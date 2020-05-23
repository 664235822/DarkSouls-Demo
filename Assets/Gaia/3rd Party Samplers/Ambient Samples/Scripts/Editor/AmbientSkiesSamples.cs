#if !AMBIENT_SKIES
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.Rendering;
using System;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
#if HDPipeline
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif
using UnityEngine.Experimental.Rendering;
using UnityEditor.SceneManagement;

namespace Gaia.GX.ProceduralWorlds
{
    public class AmbientSkiesSamples : MonoBehaviour
    {
        #region Generic informational methods

        /// <summary>
        /// Returns the publisher name if provided. 
        /// This will override the publisher name in the namespace ie Gaia.GX.PublisherName
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Procedural Worlds";
        }

        /// <summary>
        /// Returns the package name if provided
        /// This will override the package name in the class name ie public class PackageName.
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Ambient Skies Samples";
        }

#endregion

        #region Methods exposed by Gaia as buttons must be prefixed with GX_

        //Says what the product is about
        public static void GX_About()
        {
            EditorUtility.DisplayDialog("About Ambient Skies Samples", "Ambient Skies Samples allows you to give your scene a quick makeover. Using HDRI skyboxes and quick time of day setups allows you to view your scene in different lighting. Also enjoy our own simple water included with gaia, a very lightweight shader allows you to view your scene and experiment with water. Lastly we offer post processing v2 support this system allows you to use the new stacks v2 setup and it's automatically sets this up for you in your scene and auto sets for the time of day. Note: Post processing only works in 5.6.1f1 or higher and MSVO Ambient Occlusion only works in 2017.1 or higher", "OK");
        }

        //Links to full version of the product
        public static void GX_GetFullVersion()
        {
            EditorUtility.DisplayDialog("Ambient Skies Full Version", "Ambient Skies full version coming soon!", "OK");
            //Application.OpenURL("LinkGoesHere");
        }

        //Sets the time of day to morning
        public static void GX_Skies_Morning()
        {
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GaiaPipelineUtils.SetupHDEnvironmentalVolume(true, false, true, false, false, false, false, m_gaiaSettings.m_currentRenderer, "High Definition Wind", "High Definition Environment Volume", 1f, 1f, "Density Volume", "FFFFFF", 150f, new Vector3(0.1f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleMorningAndEvening", "FFC39E", 30f, 0.9f, 1050f, 5f);
            }
            else
            {
#if UNITY_2018_1_OR_NEWER
                SetHDRISky(new Vector3(165f, 0f, 0f), "FFDFCA00", 1.1f, "Ambient Skies Sample Sky", "AmbientSkiesSampleMorningAndEvening", "ABA49900", 0.85f, 0f, "AE937A", -15f, 1.1f, "E9DECC", "E0DAC7", "D1BFBF");
#else
                SetHDRISky(new Vector3(165f, 0f, 0f), "FFDFCA00", 1.2f, "Ambient Skies Sample Sky", "AmbientSkiesSampleMorningAndEvening", "ABA49900", 0.85f, 0f, "AE937A", -15f, 0.85f, "80786C", "63615B", "9A8D8D");
#endif
            }

#if HDPipeline && UNITY_2019_1_OR_NEWER
 
#else
            SetPostProcessingStyle("Ambient Sample Default Morning Post Processing");
#endif
            SetAmbientAudio("Gaia Ambient Audio Morning");

#if UNITY_2019_1_OR_NEWER
            if (m_gaiaSettings.m_currentRenderer != GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GetOrCreateReflectionProbe("Global Reflection Probe");
                BakeGlobalReflectionProbe(false);
            }
            else
            {
                RemoveGlobalProbe("Global Reflection Probe");
            }
#else
            GetOrCreateReflectionProbe("Global Reflection Probe");
            BakeGlobalReflectionProbe(false);
#endif
        }

        //Sets the time of day to day
        public static void GX_Skies_Day()
        {
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GaiaPipelineUtils.SetupHDEnvironmentalVolume(true, false, false, true, false, false, false, m_gaiaSettings.m_currentRenderer, "High Definition Wind", "High Definition Environment Volume", 0.8f, 2f, "Density Volume", "FFFFFF", 300f, new Vector3(0.1f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleDay", "FFF4D3", 110f, 2.05f, 1200f, -25f);
            }
            else
            {
#if UNITY_2018_1_OR_NEWER
                SetHDRISky(new Vector3(70f, 180f, 0f), "FFF6E9FF", 4f, "Ambient Skies Sample Sky", "AmbientSkiesSampleDay", "5A5A5AFF", 1.6f, 0f, "CEE2E9", 50f, 1.2f, "B7C1C9", "A9A395", "8E8166");
#else
                SetHDRISky(new Vector3(70f, 180f, 0f), "FFF6E9FF", 2f, "Ambient Skies Sample Sky", "AmbientSkiesSampleDay", "5A5A5AFF", 1.6f, 0f, "CEE2E9", 50f, 1f, "D4DBEC", "EADCBC", "DFD9D9");
#endif
            }

#if HDPipeline && UNITY_2019_1_OR_NEWER

#else
            SetPostProcessingStyle("Ambient Sample Default Day Post Processing");
#endif
            SetAmbientAudio("Gaia Ambient Audio Day");

#if UNITY_2019_1_OR_NEWER
            if (m_gaiaSettings.m_currentRenderer != GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GetOrCreateReflectionProbe("Global Reflection Probe");
                BakeGlobalReflectionProbe(false);
            }
            else
            {
                RemoveGlobalProbe("Global Reflection Probe");
            }
#else
            GetOrCreateReflectionProbe("Global Reflection Probe");
            BakeGlobalReflectionProbe(false);
#endif
        }

        //Sets the time of day to morning
        public static void GX_Skies_Evening()
        {
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GaiaPipelineUtils.SetupHDEnvironmentalVolume(true, false, false, false, true, false, false, m_gaiaSettings.m_currentRenderer, "High Definition Wind", "High Definition Environment Volume", 0.9f, 0.75f, "Density Volume", "FFFFFF", 200f, new Vector3(0.1f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleMorningAndEvening", "FFD0C2", 150f, 0.81f, 1200f, 5f);
            }
            else
            {
#if UNITY_2018_1_OR_NEWER
                SetHDRISky(new Vector3(15f, 0f, 0f), "FFDFCA00", 1.3f, "Ambient Skies Sample Sky", "AmbientSkiesSampleMorningAndEvening", "B4B4B400", 0.75f, 180f, "A69180", -15f, 1.3f, "F6EED7", "A8A393", "AD9C9C");
#else
                SetHDRISky(new Vector3(15f, 0f, 0f), "FFDFCA00", 0.9f, "Ambient Skies Sample Sky", "AmbientSkiesSampleMorningAndEvening", "B4B4B400", 0.75f, 180f, "A69180", -15f, 1f, "676253", "706E64", "9F8A8A");
#endif
            }

#if HDPipeline && UNITY_2019_1_OR_NEWER

#else
            SetPostProcessingStyle("Ambient Sample Default Evening Post Processing");
#endif
            SetAmbientAudio("Gaia Ambient Audio Evening");

#if UNITY_2019_1_OR_NEWER
            if (m_gaiaSettings.m_currentRenderer != GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GetOrCreateReflectionProbe("Global Reflection Probe");
                BakeGlobalReflectionProbe(false);
            }
            else
            {
                RemoveGlobalProbe("Global Reflection Probe");
            }
#else
            GetOrCreateReflectionProbe("Global Reflection Probe");
            BakeGlobalReflectionProbe(false);
#endif
        }

        //Sets the time of day to night
        public static void GX_Skies_Night()
        {
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GaiaPipelineUtils.SetupHDEnvironmentalVolume(true, false, false, false, false, true, false, m_gaiaSettings.m_currentRenderer, "High Definition Wind", "High Definition Environment Volume", 1f, 0.5f, "Density Volume", "4B4B4B", 300f, new Vector3(0.1f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleNight", "9EC0FF", 135f, 0.57f, 800f, 50f);
            }
            else
            {
#if UNITY_2018_1_OR_NEWER
                SetHDRISky(new Vector3(135f, 0f, 0f), "ABC8FFFF", 1.3f, "Ambient Skies Sample Sky", "AmbientSkiesSampleNight", "404040FF", 1f, 0f, "1A1F2FFF", 50f, 2f, "2A303A", "4D4E51", "4B4B4B");
#else
                SetHDRISky(new Vector3(135f, 0f, 0f), "ABC8FFFF", 0.8f, "Ambient Skies Sample Sky", "AmbientSkiesSampleNight", "404040FF", 1f, 0f, "1A1F2FFF", 50f, 1f, "2A303A", "777A83", "4B4B4B");
#endif
            }

#if HDPipeline && UNITY_2019_1_OR_NEWER

#else
            SetPostProcessingStyle("Ambient Sample Default Night Post Processing");
#endif
            SetAmbientAudio("Gaia Ambient Audio Night");

#if UNITY_2019_1_OR_NEWER
            if (m_gaiaSettings.m_currentRenderer != GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GetOrCreateReflectionProbe("Global Reflection Probe");
                BakeGlobalReflectionProbe(false);
            }
            else
            {
                RemoveGlobalProbe("Global Reflection Probe");
            }
#else
            GetOrCreateReflectionProbe("Global Reflection Probe");
            BakeGlobalReflectionProbe(false);
#endif
        }

        //Sets the time of day to default procedural skybox
        public static void GX_Skies_DefaultProcedural()
        {
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GaiaPipelineUtils.SetupHDEnvironmentalVolume(true, false, false, true, false, false, true, m_gaiaSettings.m_currentRenderer, "High Definition Wind", "High Definition Environment Volume", 1.5f, 2.5f, "Density Volume", "FFFFFF", 300f, new Vector3(0.1f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleDay", "FFF6E9FF", 110f, 1.95f, 1200f, -25f);
            }
            else
            {
                GameObject parentObject = GetOrCreateEnvironmentParent();
                GameObject lightObject = GetOrCreateDirectionalLight();
                lightObject.transform.parent = parentObject.transform;
                lightObject.transform.localRotation = Quaternion.Euler(50f, -30f, 0f);
                Light light = lightObject.GetComponent<Light>();
                if (light != null)
                {
                    light.color = GetColorFromHTML("FFF4D6");
                    light.intensity = 1.4f;
                    RenderSettings.sun = light;
                }

                //Set the skybox material
                string skyMatPath = GetAssetPath("Ambient Skies Default Sky");
                if (!string.IsNullOrEmpty(skyMatPath))
                {
#if UNITY_EDITOR
                    RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>(skyMatPath);
#endif
                }

                //Set render settings
                if (!Application.isPlaying)
                {
                    RenderSettings.ambientMode = AmbientMode.Skybox;
                }
                else
                {
                    RenderSettings.ambientMode = AmbientMode.Trilight;
                    RenderSettings.ambientSkyColor = GetColorFromHTML("B7C1C9");
                    RenderSettings.ambientEquatorColor = GetColorFromHTML("A9A395");
                    RenderSettings.ambientGroundColor = GetColorFromHTML("8E8166");
                }

                //Set the fog 
                RenderSettings.fog = true;
                RenderSettings.fogColor = GetColorFromHTML("B1CEE6");
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = 50f;
                Terrain theTerrain = GetActiveTerrain();
                if (theTerrain != null)
                {
                    RenderSettings.fogEndDistance = theTerrain.terrainData.size.y / 1.25f;
                }
                else
                {
                    RenderSettings.fogEndDistance = 1000f / 1.25f;
                }
            }

#if HDPipeline && UNITY_2019_1_OR_NEWER

#else
            SetPostProcessingStyle("Ambient Sample Default Day Post Processing");
#endif
            SetAmbientAudio("Gaia Ambient Audio Day");

#if UNITY_2019_1_OR_NEWER
            if (m_gaiaSettings.m_currentRenderer != GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GetOrCreateReflectionProbe("Global Reflection Probe");
                BakeGlobalReflectionProbe(false);
            }
            else
            {
                RemoveGlobalProbe("Global Reflection Probe");
            }
#else
            GetOrCreateReflectionProbe("Global Reflection Probe");
            BakeGlobalReflectionProbe(false);
#endif
        }

        //Allows you to remove ambient skies samples from your scene
        public static void GX_Skies_RemoveAmbientSkies()
        {
            if (EditorUtility.DisplayDialog("Remove Ambient Skies Sample", "You're about to remove Ambient Skies Samples from your scene. This will remove Water and effects, post processing and set the skybox to default procedural. Would you like to remove it?", "Yes", "Cancel"))
            {
                RemoveAmbientSkiesSample();
            }
        }

        //Adds global reflection probe to your scene
        public static void GX_Skies_AddGlobalReflectionProbe()
        {
            GetOrCreateReflectionProbe("Global Reflection Probe");
        }

        //Removes the global reflection probe from your scene
        public static void GX_Skies_RemoveGlobalReflectionProbe()
        {
            RemoveGlobalProbe("Global Reflection Probe");
            RemoveGlobalProbe("Camera Reflection Probe");
        }

        //Bakes the lighting for your current open scene
        public static void GX_Skies_BakeLighting()
        {
            if (!Lightmapping.isRunning)
            {
                RenderSettings.ambientMode = AmbientMode.Skybox;
                Lightmapping.bakedGI = true;
                Lightmapping.realtimeGI = true;
#if UNITY_2018_2_OR_NEWER
                LightmapEditorSettings.directSampleCount = 32;
                LightmapEditorSettings.indirectSampleCount = 500;
                LightmapEditorSettings.bounces = 3;
                LightmapEditorSettings.filteringMode = LightmapEditorSettings.FilterMode.Auto;
                LightmapEditorSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
#endif
                LightmapEditorSettings.realtimeResolution = 2;
                LightmapEditorSettings.bakeResolution = 16;
                LightmapEditorSettings.padding = 2;
                LightmapEditorSettings.textureCompression = false;
                LightmapEditorSettings.enableAmbientOcclusion = true;
                LightmapEditorSettings.aoMaxDistance = 1f;
                LightmapEditorSettings.aoExponentIndirect = 1f;
                LightmapEditorSettings.aoExponentDirect = 1f;
                Lightmapping.BakeAsync();
            }
        }

        //Adds occlusion culling volume
        public static void GX_Occlusion_AddOcclusionCullingVolume()
        {
            OcclusionCulling(true, false, false);
        }

        //Removes occlusion volume
        public static void GX_Occlusion_RemoveOcclusionCullingVolume()
        {
            OcclusionCulling(false, false, false);
        }

        //Bakes and adds occlusion volume
        public static void GX_Occlusion_BakeOcclusionCulling()
        {
            OcclusionCulling(true, true, false);
        }

        //Cancels occlusion bake
        public static void GX_Occlusion_CancelOcclusionCulling()
        {
            OcclusionCulling(true, false, false);
        }

        //Clears occlusion data
        public static void GX_Occlusion_ClearOcclusionCulling()
        {
            OcclusionCulling(true, false, true);
        }

        //Sets postprocessing to blockbuster1
        public static void GX_PostProcessing_DefaultMorning()
        {
#if HDPipeline && UNITY_2019_1_OR_NEWER
           
#else
            SetPostProcessingStyle("Ambient Sample Default Morning Post Processing");
#endif
        }

        //Sets postprocessing to default
        public static void GX_PostProcessing_DefaultDay()
        {
#if HDPipeline && UNITY_2019_1_OR_NEWER
            
#else
            SetPostProcessingStyle("Ambient Sample Default Day Post Processing");
#endif
        }

        //Sets postprocessing to real low contrast
        public static void GX_PostProcessing_DefaultEvening()
        {
#if HDPipeline && UNITY_2019_1_OR_NEWER
           
#else
            SetPostProcessingStyle("Ambient Sample Default Evening Post Processing");
#endif
        }

        //Sets postprocessing to vibrant1
        public static void GX_PostProcessing_DefaultNight()
        {
#if HDPipeline && UNITY_2019_1_OR_NEWER
           
#else
            SetPostProcessingStyle("Ambient Sample Default Night Post Processing");
#endif
        }

        //Removes post processing v2 from your scene
        public static void GX_PostProcessing_RemovePostProcessing()
        {
            RemovePostProcessingV2();
        }

#endregion

        #region Helper methods
        /// <summary>
        /// Sets up and bakes occlusion culling
        /// </summary>
        /// <param name="occlusionCullingEnabled"></param>
        /// <param name="bakeOcclusionCulling"></param>
        /// <param name="clearBakedData"></param>
        public static void OcclusionCulling(bool occlusionCullingEnabled, bool bakeOcclusionCulling, bool clearBakedData)
        {
            GameObject parentObject = GetOrCreateEnvironmentParent();
            if (occlusionCullingEnabled)
            {
                Terrain terrain = GetActiveTerrain();

                GameObject occlusionCullObject = GameObject.Find("Occlusion Culling Volume");
                if (occlusionCullObject == null)
                {
                    occlusionCullObject = new GameObject("Occlusion Culling Volume");
                    occlusionCullObject.transform.SetParent(parentObject.transform);
                    OcclusionArea occlusionArea = occlusionCullObject.AddComponent<OcclusionArea>();
                    if (terrain != null)
                    {
                        occlusionArea.size = new Vector3(terrain.terrainData.size.x, terrain.terrainData.size.y, terrain.terrainData.size.z);
                    }
                    else
                    {
                        occlusionArea.size = new Vector3(2000f, 1000f, 2000f);
                    }

                    StaticOcclusionCulling.smallestOccluder = 4f;
                    StaticOcclusionCulling.smallestHole = 0.2f;
                    StaticOcclusionCulling.backfaceThreshold = 15f;
                }
                else
                {
                    OcclusionArea occlusionArea = occlusionCullObject.GetComponent<OcclusionArea>();
                    if (occlusionArea != null)
                    {
                        if (terrain != null)
                        {
                            occlusionArea.size = new Vector3(terrain.terrainData.size.x, terrain.terrainData.size.y, terrain.terrainData.size.z);
                        }
                        else
                        {
                            occlusionArea.size = new Vector3(2000f, 1000f, 2000f);
                        }

                        StaticOcclusionCulling.smallestOccluder = 4f;
                        StaticOcclusionCulling.smallestHole = 0.2f;
                        StaticOcclusionCulling.backfaceThreshold = 15f;
                    }
                }
            }
            else
            {
                GameObject occlusionObject = GameObject.Find("Occlusion Culling Volume");
                if (occlusionObject != null)
                {
                    DestroyImmediate(occlusionObject);
                }
            }

            if (bakeOcclusionCulling)
            {
                StaticOcclusionCulling.GenerateInBackground();
            }
            else
            {
                StaticOcclusionCulling.Cancel();
            }

            if (clearBakedData)
            {
                StaticOcclusionCulling.Clear();
            }
        }

        /// <summary>
        /// Removes ambient skies samples
        /// </summary>
        private static void RemoveAmbientSkiesSample()
        {
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

            RemoveGlobalProbe("Global Reflection Probe");
            RemovePostProcessingV2();

            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
#if HDPipeline
                Volume volume = FindObjectOfType<Volume>();
                if (volume != null)
                {
                    VolumeProfile profile = volume.sharedProfile;
                    if (profile != null)
                    {
                        VisualEnvironment environment;
                        if (profile.TryGet(out environment))
                        {
                            environment.skyType.value = 2;
                        }

                        HDRISky hDRISky;
                        if (profile.TryGet(out hDRISky))
                        {
                            hDRISky.active = false;
                        }

                        ProceduralSky sky;
                        if (profile.TryGet(out sky))
                        {
                            sky.active = true;
                        }
                    }
                }
#endif
            }
            else
            {
                Material skyMaterial = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath("Ambient Skies Default Sky"));
                if (skyMaterial != null)
                {
                    RenderSettings.skybox = skyMaterial;
                }

                GameObject hdSky = GameObject.Find("High Definition Environment Volume");
                if (hdSky != null)
                {
                    DestroyImmediate(hdSky);
                }

                GameObject cameraProbe = GameObject.Find("Camera Reflection Probe");
                if (cameraProbe != null)
                {
                    DestroyImmediate(cameraProbe);
                }
            }

            GameObject parentObject = GetOrCreateEnvironmentParent();
            GameObject lightObject = GetOrCreateDirectionalLight();
            lightObject.transform.parent = parentObject.transform;
            lightObject.transform.localRotation = Quaternion.Euler(50f, -30f, 0f);
            Light light = lightObject.GetComponent<Light>();
            if (light != null && m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
#if HDPipeline
                HDAdditionalLightData lightData = FindObjectOfType<HDAdditionalLightData>();
                if (lightData != null)
                {
                    light.color = new Color32(255, 244, 214, 255);
                    lightData.intensity = 3.14f;
                }
#endif
            }
            else if (light != null)
            {
                light.color = new Color32(255, 244, 214, 255);
                light.intensity = 1f;
                RenderSettings.sun = light;
            }

            GameObject ambientAudio = GameObject.Find("Ambient Audio");
            if (ambientAudio != null)
            {
                DestroyImmediate(ambientAudio);
            }
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns></returns>
        private static string GetAssetPath(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            return null;
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
        /// Adds ambiance audio to the scene 
        /// </summary>
        /// <param name="audioName"></param>
        public static void SetAmbientAudio(string audioName)
        {
            string audioFile = GetAssetPath(audioName);
            if (string.IsNullOrEmpty(audioFile))
            {
                Debug.LogWarning("Audio " + audioFile + " is missing");
                return;
            }

            GameObject audioSource = GameObject.Find("Ambient Audio");
            if (audioSource == null)
            {
                audioSource = new GameObject("Ambient Audio");
                audioSource.transform.parent = GetOrCreateEnvironmentParent().transform;
                audioSource.AddComponent<AudioSource>();
            }

            AudioSource theAudioSource = audioSource.GetComponent<AudioSource>();
            theAudioSource.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioFile);
            theAudioSource.volume = 0.225f;
            theAudioSource.loop = true;

            Terrain terrain = GetActiveTerrain();
            if (terrain != null)
            {
                theAudioSource.maxDistance = GetActiveTerrain().terrainData.size.x * 1.1f;
            }
            else
            {
                theAudioSource.maxDistance = 3000f;
            }

            if (Application.isPlaying)
            {
                theAudioSource.Play();
            }
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
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

            GameObject parentObject = GetOrCreateEnvironmentParent();
            GameObject lightObject = GetOrCreateDirectionalLight();

            lightObject.transform.parent = parentObject.transform;
            lightObject.transform.localRotation = Quaternion.Euler(sunRotation);
            Light light = lightObject.GetComponent<Light>();
            if (light != null)
            {
                light.color = GetColorFromHTML(sunColor);
                if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
                {
                    light.intensity = sunIntensity;
                }
                else
                {
                    light.intensity = sunIntensity / 2f;
                }
                RenderSettings.sun = light;
            }

            //Set the skybox material
#if UNITY_EDITOR
            string skyMatPath = GetAssetPath(skyMaterial);
            string hdrSkyPath = GetAssetPath(hdrSkyTexture);
            if (!string.IsNullOrEmpty(skyMatPath) && !string.IsNullOrEmpty(hdrSkyPath))
            {
                Material hdrSkyMaterial = AssetDatabase.LoadAssetAtPath<Material>(skyMatPath);

                if (hdrSkyMaterial.shader != Shader.Find("Skybox/Cubemap"))
                {
                    hdrSkyMaterial.shader = Shader.Find("Skybox/Cubemap");
                }

                hdrSkyMaterial.SetColor("_Tint", GetColorFromHTML(skyTint));
                hdrSkyMaterial.SetFloat("_Exposure", skyExposure);
                hdrSkyMaterial.SetFloat("_Rotation", skyRotation);
                hdrSkyMaterial.SetTexture("_Tex", AssetDatabase.LoadAssetAtPath<Texture>(hdrSkyPath));
                RenderSettings.skybox = hdrSkyMaterial;
            }
#endif

            //Set render settings
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = GetColorFromHTML(skyColor);
            RenderSettings.ambientEquatorColor = GetColorFromHTML(equatorColor);
            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
            {
                RenderSettings.ambientIntensity = skyGroundIntensity - 0.2f;
            }
            else
            {
                RenderSettings.ambientIntensity = skyGroundIntensity;
            }
            RenderSettings.ambientGroundColor = GetColorFromHTML(groundColor);
            RenderSettings.fog = true;
            RenderSettings.fogColor = GetColorFromHTML(fogColor);
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = fogStartDistance;

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
                RenderSettings.fogEndDistance = Mathf.Clamp(fogDistance / 2f, 250f, 2300f);
            }

            GaiaReflectionProbeUpdate[] rpuArray = FindObjectsOfType<GaiaReflectionProbeUpdate>();
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
        /// Set the post processing in the scene
        /// </summary>
        public static void SetPostProcessingStyle(string ppName)
        {
            //Hack to not set PP on ultralight and mobile - because it is too expensive
            GaiaSettings settings = GaiaUtils.GetGaiaSettings();
            if (settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.UltraLight || settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.MobileAndVR)
            {
                return;
            }

#if UNITY_5_6_0
                return;
#endif

#if UNITY_POST_PROCESSING_STACK_V2

            RemovePostProcessingV1();
            GameObject theParentObject = GetOrCreateEnvironmentParent();
            GameObject postProcessingVolumeObject = GameObject.Find("Global Post Processing");
            GameObject mainCameraObject = GetOrCreateMainCamera();

            //If the post processing volume is null it creates one
            if (postProcessingVolumeObject == null)
            {
                postProcessingVolumeObject = new GameObject("Global Post Processing");
                postProcessingVolumeObject.transform.parent = theParentObject.transform;
                postProcessingVolumeObject.layer = LayerMask.NameToLayer("TransparentFX");

                var ppVol = postProcessingVolumeObject.AddComponent<PostProcessVolume>();
                ppVol.isGlobal = true;
                ppVol.priority = 0f;
                ppVol.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(ppName));
                ppVol.weight = 1f;
                ppVol.blendDistance = 0f;
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(ppVol, false);
            }
            else
            {
                var ppVol = postProcessingVolumeObject.GetComponent<PostProcessVolume>();
                ppVol.isGlobal = true;
                ppVol.priority = 0f;
                ppVol.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(ppName));
                ppVol.weight = 1f;
                ppVol.blendDistance = 0f;
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(ppVol, false);
            }

            PostProcessProfile profile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(ppName));
            if (profile != null)
            {
                UnityEngine.Rendering.PostProcessing.AmbientOcclusion ambientOcclusion;
#if UNITY_2017_3_OR_NEWER
                if (profile.TryGetSettings(out ambientOcclusion))
                {
                    ambientOcclusion.mode.value = AmbientOcclusionMode.MultiScaleVolumetricObscurance;
                }
#else
                if (profile.TryGetSettings(out ambientOcclusion))
                {
                    ambientOcclusion.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
                }
#endif
                GaiaPipelineUtils.FixPostProcessingV2(settings.m_currentRenderer);
            }

            //Set the main camera to support post processing
            mainCameraObject.GetComponent<Camera>().renderingPath = RenderingPath.UsePlayerSettings;

            var ppLayer = mainCameraObject.GetComponent<PostProcessLayer>();
            if (ppLayer == null)
            {
                ppLayer = mainCameraObject.AddComponent<PostProcessLayer>();
                ppLayer.volumeTrigger = mainCameraObject.transform;
                ppLayer.volumeLayer = 2;

                Camera cameraComponent = mainCameraObject.GetComponent<Camera>();
                if (settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                {
                    ppLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                    cameraComponent.allowHDR = false;
                    cameraComponent.allowMSAA = true;
                }
                else
                {
#if !UNITY_2017_1_OR_NEWER
                ppLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
#else
                    ppLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
#endif
                    ppLayer.fog.excludeSkybox = true;
                    ppLayer.fog.enabled = true;
                    ppLayer.stopNaNPropagation = true;

                    cameraComponent.allowHDR = true;

                    var tier1 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1);
                    var tier2 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2);
                    var tier3 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3);
                    if (tier1.renderingPath == RenderingPath.DeferredShading || tier2.renderingPath == RenderingPath.DeferredShading || tier3.renderingPath == RenderingPath.DeferredShading)
                    {
                        cameraComponent.allowMSAA = false;
                    }
                    else
                    {
                        cameraComponent.allowMSAA = true;
                    }
                    if (settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                    {

                    }
                    else
                    {
                        
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Removes Gaia post processing V2
        /// </summary>
        private static void RemovePostProcessingV2()
        {
#if UNITY_POST_PROCESSING_STACK_V2
            GameObject mainCameraObject = GetOrCreateMainCamera();
            PostProcessLayer ppLayer = mainCameraObject.GetComponent<PostProcessLayer>();
            if (ppLayer != null)
            {
                DestroyImmediate(ppLayer);
            }

            GameObject postProcessVolumeObject = GameObject.Find("Global Post Processing");
            if (postProcessVolumeObject != null)
            {
                DestroyImmediate(postProcessVolumeObject);
            }
#endif
        }

        /// <summary>
        /// Removes Gaia post processing V1
        /// </summary>
        private static void RemovePostProcessingV1()
        {
#if UNITY_5_5_OR_NEWER
            //If post processing volume not in project exit.
            Type postProcessingBehaviourType = GaiaCommon1.Utils.GetType("UnityEngine.PostProcessing.PostProcessingBehaviour");
            if (postProcessingBehaviourType == null)
            {
                return;
            }

            //Find camera
            GameObject cameraObject = GetOrCreateMainCamera();
            var postProcessingBehaviour = cameraObject.GetComponent(postProcessingBehaviourType);
            if (postProcessingBehaviour != null)
            {
                DestroyImmediate(postProcessingBehaviour);
            }
#endif
        }

        /// <summary>
        /// Removes the probeName from the scene
        /// </summary>
        /// <param name="probeName"></param>
        private static void RemoveGlobalProbe(string probeName)
        {
            GameObject probeAsset = GameObject.Find(probeName);

            if (probeAsset != null)
            {
                DestroyImmediate(probeAsset);
            }
        }

        /// <summary>
        /// Bake SphericalHarmonicsL2 values
        /// </summary>
        /// <param name="probePosition"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        /// <param name="shL2"></param>
        public static void SHAddPointLight(Vector3 probePosition, Vector3 position, float range, Color color, float intensity, ref SphericalHarmonicsL2 sphericalHarmonicsL2)
        {
            Vector3 probeToLight = position - probePosition;
            float attenuation = 1.0F / (1.0F + 25.0F * probeToLight.sqrMagnitude / (range * range));
            sphericalHarmonicsL2.AddDirectionalLight(probeToLight.normalized, color, intensity * attenuation);
        }

        /// <summary>
        /// Bakes ambient lighting
        /// </summary>
        public static void BakeAmbientLight()
        {
            if (Lightmapping.lightingDataAsset == null)
            {
                Debug.Log("No baked probes found, have you baked your lighting?");
                return;
            }
            else
            {
                GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
                if (m_gaiaSettings == null)
                {
                    Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                    return;
                }
                else
                {
                    if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
                    {
                        return;
                    }
                    else
                    {
                        Color Ambient = RenderSettings.ambientSkyColor;
                        Light[] Lights = FindObjectsOfType<Light>();

                        SphericalHarmonicsL2[] bakedProbes = LightmapSettings.lightProbes.bakedProbes;
                        Vector3[] probePositions = LightmapSettings.lightProbes.positions;
                        int probeCount = LightmapSettings.lightProbes.count;

                        // Clear all probes
                        for (int i = 0; i < probeCount; i++)
                        {
                            if (i < 0)
                            {
                                return;
                            }
                        }

                        for (int i = 0; i < probeCount; i++)
                        {
                            if (i > 15)
                            {
                                bakedProbes[i].Clear();
                            }
                        }

                        // Add ambient light to all probes
                        for (int i = 0; i < probeCount; i++)
                        {
                            if (i > 15)
                            {
                                bakedProbes[i].AddAmbientLight(Ambient);
                            }
                        }

                        // Add directional and point lights' contribution to all probes
                        foreach (Light allLights in Lights)
                        {
                            if (allLights.type == LightType.Directional)
                            {
                                for (int dirLight = 0; dirLight < probeCount; dirLight++)
                                {
                                    if (dirLight > 15)
                                    {
                                        bakedProbes[dirLight].AddDirectionalLight(-allLights.transform.forward, allLights.color, allLights.intensity);
                                    }
                                }
                            }
                            else if (allLights.type == LightType.Point)
                            {
                                for (int poiLight = 0; poiLight < probeCount; poiLight++)
                                {
                                    if (poiLight > 15)
                                    {
                                        SHAddPointLight(probePositions[poiLight], allLights.transform.position, allLights.range, allLights.color, allLights.intensity, ref bakedProbes[poiLight]);
                                    }
                                }
                            }
                        }

                        //Set the bake probes settings to new setup
                        if (LightmapSettings.lightProbes == null)
                        {
                            LightmapSettings.lightProbes.bakedProbes = bakedProbes;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or creates a reflection probe
        /// </summary>
        private static void GetOrCreateReflectionProbe(string probeName)
        {
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

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
                probe.mode = ReflectionProbeMode.Baked;
                probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
                probe.refreshMode = ReflectionProbeRefreshMode.OnAwake;

                Terrain theTerrain = GetActiveTerrain();
                if (theTerrain != null)
                {
                    probe.size = new Vector3(theTerrain.terrainData.size.x * 10, theTerrain.terrainData.size.y, theTerrain.terrainData.size.z * 10);
                    probe.farClipPlane = theTerrain.terrainData.size.x;
                    Vector3 probeLocation = new Vector3(0f, 0f, 0f);
                    probeLocation.y = theTerrain.SampleHeight(probeLocation) + 50f;
                    reflectionProbeObject.transform.localPosition = probeLocation;
                }
                else
                {
                    probe.size = new Vector3(2560f, 1500f, 2560f);
                    probe.farClipPlane = 3000f;
                    reflectionProbeObject.transform.localPosition = new Vector3(0f, 250f, 0f);
                }
            }
            else
            {
                ReflectionProbe probe = reflectionProbeObject.GetComponent<ReflectionProbe>();
                if (probe != null)
                {
                    probe.importance = 1;
                    probe.intensity = 1f;
                    probe.blendDistance = 0f;
                    probe.resolution = 64;
                    probe.shadowDistance = 50f;
                    probe.clearFlags = ReflectionProbeClearFlags.Skybox;
                    probe.mode = ReflectionProbeMode.Baked;
                    probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
                    probe.refreshMode = ReflectionProbeRefreshMode.OnAwake;

                    Terrain theTerrain = GetActiveTerrain();
                    if (theTerrain != null)
                    {
                        if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
                        {
#if HDPipeline
                            HDAdditionalReflectionData data = reflectionProbeObject.GetComponent<HDAdditionalReflectionData>();
                            if (data != null)
                            {
                                data.influenceVolume.boxSize = new Vector3(theTerrain.terrainData.size.x * 10, theTerrain.terrainData.size.y, theTerrain.terrainData.size.z * 10);
#if !UNITY_2019_1_OR_NEWER
                                data.captureSettings.farClipPlane = theTerrain.terrainData.size.x;
#endif
                            }
#endif
                            }
                        else
                        {
                            probe.size = new Vector3(theTerrain.terrainData.size.x * 10, theTerrain.terrainData.size.y, theTerrain.terrainData.size.z * 10);
                            probe.farClipPlane = theTerrain.terrainData.size.x;
                        }

                        Vector3 probeLocation = new Vector3(0f, 0f, 0f);
                        probeLocation.y = theTerrain.SampleHeight(probeLocation) + 50f;
                        reflectionProbeObject.transform.localPosition = probeLocation;
                    }
                    else
                    {
                        if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
                        {
#if HDPipeline
                            HDAdditionalReflectionData data = reflectionProbeObject.GetComponent<HDAdditionalReflectionData>();
                            if (data != null)
                            {
                                data.influenceVolume.boxSize = new Vector3(2560f, 1500f, 2560f);
#if !UNITY_2019_1_OR_NEWER
                                data.captureSettings.farClipPlane = 2750f;
#endif
                            }
#endif
                            }
                        else
                        {
                            probe.size = new Vector3(2560f, 1500f, 2560f);
                            probe.farClipPlane = 3000f;
                        }
                        reflectionProbeObject.transform.localPosition = new Vector3(0f, 250f, 0f);
                    }
                }
            }

            BakeGlobalReflectionProbe(false);
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
            ReflectionProbe[] reflectionProbes = FindObjectsOfType<ReflectionProbe>();
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

                    BakeAmbientLight();
                    Lightmapping.BakeReflectionProbe(probe, AssetDatabase.GetAssetPath(probe.bakedTexture));
                }
                else
                {
                    probe.RenderProbe();
                }
            }
        }

        /// <summary>
        /// Get the asset prefab if we can find it in the project
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject GetAssetPrefab(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (path.Contains(".prefab"))
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }
            return null;
        }

        /// <summary>
        /// Get the currently active terrain - or any terrain
        /// </summary>
        /// <returns>A terrain if there is one</returns>
        public static Terrain GetActiveTerrain()
        {
            //Grab active terrain if we can
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null && terrain.isActiveAndEnabled)
            {
                return terrain;
            }

            //Then check rest of terrains
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                if (terrain != null && terrain.isActiveAndEnabled)
                {
                    return terrain;
                }
            }
            return null;
        }

        /// <summary>
        /// Get or create the main scene camera
        /// </summary>
        /// <returns>The gameobject camera</returns>
        private static GameObject GetOrCreateMainCamera()
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
            GameObject parent = GameObject.Find("Ambient Skies Samples");
            if (parent == null)
            {
                parent = new GameObject("Ambient Skies Samples");
            }

            GameObject parentGaia = GameObject.Find("Gaia Environment");
            if (parentGaia == null)
            {
                parentGaia = new GameObject("Gaia Environment");
            }

            parent.transform.SetParent(parentGaia.transform);

            return parent;
        }

        /// <summary>
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

            #endregion
    }
}
#else

using UnityEngine;
using AmbientSkies;
using UnityEditor;

namespace Gaia.GX.ProceduralWorlds
{
    public class AmbientSkiesSamples : MonoBehaviour
    {
        #region Generic informational methods

        /// <summary>
        /// Returns the publisher name if provided. 
        /// This will override the publisher name in the namespace ie Gaia.GX.PublisherName
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Procedural Worlds";
        }

        /// <summary>
        /// Returns the package name if provided
        /// This will override the package name in the class name ie public class PackageName.
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Ambient Skies Samples";
        }

        #endregion

        #region Methods exposed by Gaia as buttons must be prefixed with GX_

        //Opens Ambient Skies window when pressed
        public static void GX_OpenAmbientSkies()
        {
            OpenAmbientSkiesWindow();
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Function to open the ambient skies window
        /// </summary>
        public static void OpenAmbientSkiesWindow()
        {
            //Ambient Skies Editor Window
            var mainWindow = EditorWindow.GetWindow<AmbientSkiesEditorWindow>(false, "Ambient Skies");
            //Check to see if it's there before opening
            if (mainWindow != null)
            {
                //Show the ambient skies window
                mainWindow.Show();
            }
        }

        #endregion
    }
}
#endif