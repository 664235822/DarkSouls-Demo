#if !AMBIENT_WATER
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
    public class AmbientWaterSamples : MonoBehaviour
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
            return "Ambient Water Samples";
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

        //Adds new gaia water to your scene
        public static void GX_Water_AddWater()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

#if UNITY_2018_3_OR_NEWER && HDPipeline
            bool planarReflections = false;
#endif
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing fromy our project, please make sure Gaia settings is in your project.");
                return;
            }

            if (Application.isPlaying)
            {
                Debug.LogWarning("Can only add water when application is not playing!");
                return;
            }

            //Get relevant information
            GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
            GameObject parentObject = GetOrCreateEnvironmentParent();
            Terrain activeTerrain = GetActiveTerrain();
            Material waterMaterial = GetWaterMaterial("Ambient Water Sample Material");

            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                bool useUnderWaterProbe = false;
                GameObject underwaterProbe = GameObject.Find("Underwater Reflection Probe");
                if (useUnderWaterProbe)
                {
                    if (underwaterProbe == null)
                    {
                        underwaterProbe = new GameObject("Underwater Reflection Probe");
                        underwaterProbe.transform.SetParent(parentObject.transform);
                        underwaterProbe.transform.position = new Vector3(0f, sceneInfo.m_seaLevel - 1f, 0f);

                        ReflectionProbe probe = underwaterProbe.AddComponent<ReflectionProbe>();
                        probe.mode = ReflectionProbeMode.Baked;
#if HDPipeline
                        HDAdditionalReflectionData hdProbe = underwaterProbe.AddComponent<HDAdditionalReflectionData>();
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

                        if (!planarReflections)
                        {
                            GameObject planar = GameObject.Find("Water Planar Reflections");
                            if (planar != null)
                            {
                                DestroyImmediate(planar);
                            }
                        }
#endif
                    }
                    else
                    {
                        underwaterProbe.transform.position = new Vector3(0f, sceneInfo.m_seaLevel - 1f, 0f);

                        ReflectionProbe probe = underwaterProbe.GetComponent<ReflectionProbe>();
                        if (probe != null)
                        {
                            probe.mode = ReflectionProbeMode.Baked;
                        }
#if HDPipeline
                        HDAdditionalReflectionData hdProbe = underwaterProbe.GetComponent<HDAdditionalReflectionData>();
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

                        if (!planarReflections)
                        {
                            GameObject planar = GameObject.Find("Water Planar Reflections");
                            if (planar != null)
                            {
                                DestroyImmediate(planar);
                            }
                        }
#endif
                    }
                }
                else
                {
                    if (underwaterProbe != null)
                    {
                        DestroyImmediate(underwaterProbe);
                    }
                }
            }
            else
            {
                GameObject underwaterProbe = GameObject.Find("Underwater Reflection Probe");
                if (underwaterProbe != null)
                {
                    DestroyImmediate(underwaterProbe);
                }
            }

            //Get main directional light and make sure its set up properly
            var lightObject = GetOrCreateDirectionalLight();
            var effectsSettings = lightObject.GetComponent<GaiaUnderWaterEffects>();
            if (lightObject.GetComponent<GaiaUnderWaterEffects>() == null)
            {
                effectsSettings = lightObject.AddComponent<GaiaUnderWaterEffects>();
            }
            effectsSettings.m_causticsSize = 5;
            effectsSettings.m_followPlayer = false;
            effectsSettings.m_framesPerSecond = 25f;
            effectsSettings.m_sealevel = sceneInfo.m_seaLevel;
            effectsSettings.m_underWaterFogColor = new Color32(76, 112, 142, 255);
            effectsSettings.m_underWaterFogDistance = 65f;
            effectsSettings.LoadCaustics();
#if UNITY_EDITOR
            effectsSettings.player = effectsSettings.GetThePlayer();
#endif

#if UNITY_POST_PROCESSING_STACK_V2
            var underwaterPostFxObject = GameObject.Find("Underwater PostFX");
            if (underwaterPostFxObject == null)
            {
                underwaterPostFxObject = new GameObject("Underwater PostFX");
                underwaterPostFxObject.transform.position = new Vector3(0f, sceneInfo.m_seaLevel - 506.5f, 0f);
                underwaterPostFxObject.transform.parent = parentObject.transform;
                underwaterPostFxObject.layer = LayerMask.NameToLayer("TransparentFX");

                //Add the pp volume
                var ppVol = underwaterPostFxObject.AddComponent<PostProcessVolume>();
                ppVol.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath("Ambient Sample Underwater Post Processing"));
                ppVol.blendDistance = 4f;
                ppVol.priority = 1;
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(ppVol, false);

                PostProcessProfile profile = ppVol.sharedProfile;
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
                var colliderSettings = underwaterPostFxObject.AddComponent<BoxCollider>();
                if (activeTerrain != null)
                {
                    colliderSettings.size = new Vector3(activeTerrain.terrainData.size.x * 1.5f, 1000f, activeTerrain.terrainData.size.z * 1.5f);
                }
                else
                {
                    colliderSettings.size = new Vector3(2560f, 1000f, 2560f);
                }
                colliderSettings.isTrigger = true;
            }

            var underwaterTransitionFXObject = GameObject.Find("Underwater Transition PostFX");
            if (underwaterTransitionFXObject == null)
            {
                underwaterTransitionFXObject = new GameObject("Underwater Transition PostFX");
                underwaterTransitionFXObject.transform.position = new Vector3(0f, sceneInfo.m_seaLevel, 0f);
                underwaterTransitionFXObject.transform.parent = parentObject.transform;
                underwaterTransitionFXObject.layer = LayerMask.NameToLayer("TransparentFX");

                PostProcessVolume ppVol = underwaterTransitionFXObject.GetComponent<PostProcessVolume>();
                if (ppVol == null)
                {
                    ppVol = underwaterTransitionFXObject.AddComponent<PostProcessVolume>();
                    ppVol.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath("Ambient Sample Underwater Transaction Post Processing"));
                    ppVol.blendDistance = 0.15f;
                    ppVol.priority = 2;
                    UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(ppVol, false);
                }
                else
                {
                    ppVol.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath("Ambient Sample Underwater Transaction Post Processing"));
                    ppVol.blendDistance = 0.15f;
                    ppVol.priority = 2;
                    UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(ppVol, false);
                }


                PostProcessProfile profile = ppVol.sharedProfile;
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
                var colliderSettings = underwaterTransitionFXObject.AddComponent<BoxCollider>();
                if (activeTerrain != null)
                {
                    colliderSettings.size = new Vector3(activeTerrain.terrainData.size.x * 1.5f, 0.1f, activeTerrain.terrainData.size.z * 1.5f);
                }
                else
                {
                    colliderSettings.size = new Vector3(2560f, 0.1f, 2560f);
                }
                colliderSettings.isTrigger = true;
            }

            effectsSettings.transitionPostFX = GameObject.Find("Underwater Transition PostFX").GetComponent<PostProcessVolume>();
            effectsSettings.underwaterPostFX = GameObject.Find("Underwater PostFX").GetComponent<PostProcessVolume>();
            effectsSettings.transitionPostFX.enabled = false;
            effectsSettings.underwaterPostFX.enabled = false;
#endif

            var underwaterAudioFXObject = GameObject.Find("Underwater SoundFX");
            if (underwaterAudioFXObject == null)
            {
                underwaterAudioFXObject = new GameObject("Underwater SoundFX");
                underwaterAudioFXObject.transform.parent = parentObject.transform;
                var audio = underwaterAudioFXObject.AddComponent<AudioSource>();
                audio.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(GetAssetPath("Gaia Ambient Underwater Sound Effect"));
                audio.volume = 0f;
                audio.loop = true;

                if (activeTerrain != null)
                {
                    audio.maxDistance = activeTerrain.terrainData.size.x * 1.5f;
                }
                else
                {
                    audio.maxDistance = 3000f;
                }
            }

            //Grab or create the water
            GameObject theWaterObject = GameObject.Find("Ambient Water Sample");
            if (theWaterObject == null)
            {
                theWaterObject = Instantiate(GetAssetPrefab("Ambient Water Sample"));
                theWaterObject.name = "Ambient Water Sample";
                theWaterObject.transform.parent = parentObject.transform;
            }

            //And update it
            Vector3 waterPosition = sceneInfo.m_centrePointOnTerrain;
            waterPosition.y = sceneInfo.m_seaLevel;
            theWaterObject.transform.position = waterPosition;
            if (activeTerrain != null)
            {
                theWaterObject.transform.localScale = new Vector3(sceneInfo.m_sceneBounds.size.x, 1f, sceneInfo.m_sceneBounds.size.z);
            }
            else
            {
                theWaterObject.transform.localScale = new Vector3(256f, 1f, 256f);
            }

            //Update water material
            if (waterMaterial != null)
            {
                if (activeTerrain != null)
                {
                    waterMaterial.SetFloat("_GlobalTiling", sceneInfo.m_sceneBounds.size.x);
                }
                else
                {
                    waterMaterial.SetFloat("_GlobalTiling", 128f);
                }
            }

            if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
#if HDPipeline
                if (planarReflections)
                {
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
                                planar.influenceVolume.boxSize = new Vector3(activeTerrain.terrainData.size.x * 10f, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z * 10f);
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
                                planar.influenceVolume.boxSize = new Vector3(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z);
                            }
                            else
                            {
                                planar.influenceVolume.boxSize = new Vector3(2500f, 1000f, 2000f);
                            }
                        }

                        HDAdditionalReflectionData reflectionData = planarReflectionProbeObject.GetComponent<HDAdditionalReflectionData>();
                        if (reflectionData == null)
                        {
                            reflectionData = planarReflectionProbeObject.AddComponent<HDAdditionalReflectionData>();
#if !UNITY_2019_1_OR_NEWER
                            reflectionData.mode = ReflectionProbeMode.Custom;
#else
                            reflectionData.mode = ProbeSettings.Mode.Custom;
#endif

                        }
                        else
                        {
#if !UNITY_2019_1_OR_NEWER
                            reflectionData.mode = ReflectionProbeMode.Custom;
#else
                            reflectionData.mode = ProbeSettings.Mode.Custom;
#endif
                        }

                        ReflectionProbe probe = planarReflectionProbeObject.GetComponent<ReflectionProbe>();
                        if (probe != null)
                        {
#if !UNITY_2019_1_OR_NEWER
                            probe.mode = ReflectionProbeMode.Custom;
#else
                            probe.mode = ReflectionProbeMode.Custom;
#endif
                        }
                    }

                    GameObject theReflectionProbeObject = GameObject.Find("Camera Reflection Probe");
                    if (theReflectionProbeObject != null)
                    {
                        DestroyImmediate(theReflectionProbeObject);
                    }

                    GameObject globalProbe = GameObject.Find("Global Reflection Probe");
                    if (globalProbe != null)
                    {
                        DestroyImmediate(globalProbe);
                    }
                }
                else
                {
                    GX_Reflection_AddGlobalReflectionProbe();

                    ReflectionProbe probe = FindObjectOfType<ReflectionProbe>();
                    if (probe != null)
                    {
                        probe.mode = ReflectionProbeMode.Baked;

                        HDAdditionalReflectionData reflectionData = probe.gameObject.GetComponent<HDAdditionalReflectionData>();
                        if (reflectionData != null)
                        {
#if !UNITY_2019_1_OR_NEWER
                            reflectionData.mode = ReflectionProbeMode.Baked;
#else
                            reflectionData.mode = ProbeSettings.Mode.Baked;
#endif
                        }

                        BakeGlobalReflectionProbe(false);
                    }

                    GameObject theReflectionProbeObject = GameObject.Find("Camera Reflection Probe");
                    if (theReflectionProbeObject != null)
                    {
                        DestroyImmediate(theReflectionProbeObject);
                    }
                }
#endif
            }
            else
            {
                if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                {
                    GaiaPipelineUtils.FixPostProcessingV2(m_gaiaSettings.m_currentRenderer);
                }

                //Adds reflection probe updater script if missing
                GetOrCreateReflectionProbe("Camera Reflection Probe");
                GameObject theReflectionProbeObject = GameObject.Find("Camera Reflection Probe");
                if (theReflectionProbeObject != null)
                {
                    ReflectionProbe probe = theReflectionProbeObject.GetComponent<ReflectionProbe>();
                    if (probe != null)
                    {
                        Vector3 newProbeSize = probe.size;
                        newProbeSize.y += 3000f;

                        probe.size = newProbeSize;
                    }

                    if (theReflectionProbeObject.GetComponent<GaiaReflectionProbeUpdate>() == null)
                    {
                        GaiaReflectionProbeUpdate probeObject = theReflectionProbeObject.AddComponent<GaiaReflectionProbeUpdate>();
                        probeObject.m_followCamera = true;
                        probeObject.SetProbeSettings();
                    }
                }

                GameObject planarReflectionProbeObject = GameObject.Find("Water Planar Reflections");
                if (planarReflectionProbeObject != null)
                {
                    DestroyImmediate(planarReflectionProbeObject);
                }
            }
        }

        //Sets the water settings to deep blue
        public static void GX_Water_DeepBlueStyle()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            SetWater(0.97f, 0.15f, "4C708EFF", 0.31f, 1f, 0.92f, "878787FF", 0.4f);
            SetTheUnderwaterFogColor("4C708EFF");
        }

        //Sets the water settings to clear blue
        public static void GX_Water_ClearBlueStyle()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            SetWater(0.97f, 0.15f, "4781B2FF", 0.5f, 1f, 0.92f, "878787FF", 0.4f);
            SetTheUnderwaterFogColor("7C97AEFF");
        }

        //Sets the water settings to toxic green
        public static void GX_Water_ToxicGreenStyle()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            SetWater(0.985f, 0.15f, "257C31FF", 0.31f, 1f, 0.92f, "878787FF", 0.4f);
            SetTheUnderwaterFogColor("6C9E77FF");
        }

        //Sets the water settings to cyan
        public static void GX_Water_CyanStyle()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            SetWater(0.945f, 0.15f, "4BA3A7FF", 0.3f, 1f, 0.92f, "878787FF", 0.4f);
            SetTheUnderwaterFogColor("6C9E94FF");
        }

        //Removes new gaia water from your scene
        public static void GX_Water_RemoveWater()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                GameObject theWaterObject = GameObject.Find("Ambient Water Sample");
                if (theWaterObject != null)
                {
                    DestroyImmediate(theWaterObject);
                }

                GameObject underwaterFX = GameObject.Find("Underwater PostFX");
                if (underwaterFX != null)
                {
                    DestroyImmediate(underwaterFX);
                }

                GameObject underwaterProbe = GameObject.Find("Underwater Reflection Probe");
                if (underwaterProbe != null)
                {
                    DestroyImmediate(underwaterProbe);
                }

                GaiaUnderWaterEffects underwaterFXScript = FindObjectOfType<GaiaUnderWaterEffects>();
                if (underwaterFXScript != null)
                {
                    DestroyImmediate(underwaterFXScript);
                }

                GameObject underwaterAudioFX = GameObject.Find("Underwater SoundFX");
                if (underwaterAudioFX != null)
                {
                    DestroyImmediate(underwaterAudioFX);
                }

                GameObject underwaterTransactionPostFX = GameObject.Find("Underwater Transition PostFX");
                if (underwaterTransactionPostFX != null)
                {
                    DestroyImmediate(underwaterTransactionPostFX);
                }

                GaiaReflectionProbeUpdate reflectionProbeUpdater = FindObjectOfType<GaiaReflectionProbeUpdate>();
                if (reflectionProbeUpdater != null)
                {
                    DestroyImmediate(reflectionProbeUpdater);
                }

                GameObject hdWaterReflections = GameObject.Find("Water Planar Reflections");
                if (hdWaterReflections != null)
                {
                    DestroyImmediate(hdWaterReflections);
                }

                GameObject cameraProbe = GameObject.Find("Camera Reflection Probe");
                if (cameraProbe != null)
                {
                    DestroyImmediate(cameraProbe);
                }
            }
        }

        //Removes new gaia underwater FX from your scene
        public static void GX_Water_RemoveUnderwaterFx()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (!Application.isPlaying)
            {
                GameObject underwaterFX = GameObject.Find("Underwater PostFX");
                if (underwaterFX != null)
                {
                    DestroyImmediate(underwaterFX);
                }

                GaiaUnderWaterEffects underwaterFXScript = FindObjectOfType<GaiaUnderWaterEffects>();
                if (underwaterFXScript != null)
                {
                    DestroyImmediate(underwaterFXScript);
                }

                GameObject underwaterAudioFX = GameObject.Find("Underwater SoundFX");
                if (underwaterAudioFX != null)
                {
                    DestroyImmediate(underwaterAudioFX);
                }

                GameObject underwaterTransactionPostFX = GameObject.Find("Underwater Transition PostFX");
                if (underwaterTransactionPostFX != null)
                {
                    DestroyImmediate(underwaterTransactionPostFX);
                }

                GaiaReflectionProbeUpdate reflectionProbeUpdater = FindObjectOfType<GaiaReflectionProbeUpdate>();
                if (reflectionProbeUpdater != null)
                {
                    DestroyImmediate(reflectionProbeUpdater);
                }
            }
        }

        //Adds global reflection probe to your scene
        public static void GX_Reflection_AddGlobalReflectionProbe()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            GetOrCreateReflectionProbe("Global Reflection Probe");
        }

        //Removes the global reflection probe from your scene
        public static void GX_Reflection_RemoveGlobalReflectionProbe()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            RemoveGlobalProbe("Global Reflection Probe");
            RemoveGlobalProbe("Camera Reflection Probe");
        }

        //Bakes the lighting for your current open scene
        public static void GX_Reflection_BakeLighting()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

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

        #endregion

        #region Helper methods

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
        /// Sets the fog settings for underwater sotred settings
        /// </summary>
        /// <param name="fogColor"></param>
        /// <param name="fogEndDistance"></param>
        public static void SetUnderwaterFogSettings(Color32 fogColor, float fogEndDistance)
        {
            GaiaUnderWaterEffects underWaterFXSettings = FindObjectOfType<GaiaUnderWaterEffects>();
            if (underWaterFXSettings != null)
            {
                underWaterFXSettings.storedFogColor = fogColor;
                underWaterFXSettings.storedFogDistance = fogEndDistance;
            }
        }

        /// <summary>
        /// Sets sets the underwater fogColor
        /// </summary>
        /// <param name="fogColor"></param>
        public static void SetTheUnderwaterFogColor (string fogColor)
        {
            GaiaUnderWaterEffects underWaterFXSettings = FindObjectOfType<GaiaUnderWaterEffects>();
            if (underWaterFXSettings != null)
            {
                underWaterFXSettings.m_underWaterFogColor = GetColorFromHTML(fogColor);
            }
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
                    if (probeName == "Camera Reflection Probe")
                    {
                        Vector3 newProbeSize = probe.size;
                        newProbeSize.y += 3000f;

                        probe.size = newProbeSize;
                    }
                    else
                    {
                        probe.size = new Vector3(theTerrain.terrainData.size.x * 10, theTerrain.terrainData.size.y, theTerrain.terrainData.size.z * 10);
                        probe.farClipPlane = theTerrain.terrainData.size.x;
                        Vector3 probeLocation = new Vector3(0f, 0f, 0f);
                        probeLocation.y = theTerrain.SampleHeight(probeLocation) + 50f;
                        reflectionProbeObject.transform.localPosition = probeLocation;
                    }
                }
                else
                {
                    if (probeName == "Camera Reflection Probe")
                    {
                        Vector3 newProbeSize = probe.size;
                        newProbeSize.y = 3000f;

                        probe.size = newProbeSize;
                    }
                    else
                    {
                        probe.size = new Vector3(2560f, 1500f, 2560f);
                        probe.farClipPlane = 3000f;
                        reflectionProbeObject.transform.localPosition = new Vector3(0f, 250f, 0f);
                    }
                }
            }
            else
            {
                ReflectionProbe probe = reflectionProbeObject.GetComponent<ReflectionProbe>();
                if (probe != null)
                {
                    if (probeName == "Camera Reflection Probe")
                    {
                        Vector3 newProbeSize = probe.size;
                        newProbeSize.y += 3000f;

                        probe.size = newProbeSize;
                    }
                    else
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
            GameObject parent = GameObject.Find("Ambient Water Samples");
            if (parent == null)
            {
                parent = new GameObject("Ambient Water Samples");
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

        /// <summary>
        /// Sets the water paramaters
        /// </summary>
        /// <param name="surfaceOpacity"></param>
        /// <param name="normalScale"></param>
        /// <param name="surfaceColor"></param>
        /// <param name="surfaceColorBlend"></param>
        /// <param name="waterSpecular"></param>
        /// <param name="waterSmoothness"></param>
        /// <param name="foamTint"></param>
        /// <param name="foamOpacity"></param>
        private static void SetWater(float surfaceOpacity,float normalScale, string surfaceColor, float surfaceColorBlend, float waterSpecular, float waterSmoothness, string foamTint, float foamOpacity)
        {
            Material waterMat = GetWaterMaterial("Ambient Water Sample Material");
            if (waterMat == null)
            {
                if (Shader.Find("Procedural Worlds/Simple Water") != null)
                {
                    Material[] materials = Resources.LoadAll<Material>(GetAssetPath("Ambient Skies Samples"));
                    if (Shader.Find("Procedural Worlds/Simple Water") != null)
                    {
                        foreach (Material mats in materials)
                        {
                            if (mats.shader == Shader.Find("Procedural Worlds/Simple Water"))
                            {
                                waterMat = mats;
                            }
                            else
                            {
                                waterMat = null;
                            }
                        }
                    }
                    else if (Shader.Find("Procedural Worlds/Simple Water LW") != null)
                    {
                        foreach (Material mats in materials)
                        {
                            if (mats.shader == Shader.Find("Procedural Worlds/Simple Water LW"))
                            {
                                waterMat = mats;
                            }
                            else
                            {
                                waterMat = null;
                            }
                        }
                    }
                    else
                    {
                        foreach (Material mats in materials)
                        {
                            if (mats.shader == Shader.Find("Procedural Worlds/Simple Water HD"))
                            {
                                waterMat = mats;
                            }
                            else
                            {
                                waterMat = null;
                            }
                        }
                    }
                }
            }
            if (waterMat != null)
            {
                waterMat.SetFloat("_SurfaceOpacity", surfaceOpacity);
                waterMat.SetFloat("NormalScale", normalScale);
                waterMat.SetColor("_SurfaceColor", GetColorFromHTML(surfaceColor));
                waterMat.SetFloat("_SurfaceColorBlend", surfaceColorBlend);
                waterMat.SetFloat("_WaterSpecular", waterSpecular);
                waterMat.SetFloat("_WaterSmoothness", waterSmoothness);
                waterMat.SetColor("_FoamTint", GetColorFromHTML(foamTint));
                waterMat.SetFloat("_FoamOpacity", foamOpacity);
            }
        }

        /// <summary>
        /// Gets our own water shader if in the scene
        /// </summary>
        /// <returns>The water material if there is one</returns>
        public static Material GetWaterMaterial(string waterName)
        {
            string gaiaWater = GetAssetPath(waterName);
            if (!string.IsNullOrEmpty(gaiaWater))
            {
                return AssetDatabase.LoadAssetAtPath<Material>(gaiaWater);
            }
            return null;
        }
        #endregion
    }
}
#endif