using Gaia.Internal;
using GaiaCommon1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.PackageManager;
using UnityEngine.Experimental.Rendering;
#endif
#if UNITY_2018_3_OR_NEWER
using UnityEngine.Networking;
#endif
using UnityEditor.SceneManagement;

namespace Gaia
{
    /// <summary>
    /// Handy helper for all things Gaia
    /// </summary>
    public class GaiaManagerEditor : EditorWindow, IPWEditor
    {
        #region Variables, Properties
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private GUIStyle m_titleStyle;
        private GUIStyle m_headingStyle;
        private GUIStyle m_bodyStyle;
        private GUIStyle m_linkStyle;
        private GaiaSettings m_settings;
        private IEnumerator m_updateCoroutine;
        private EditorUtils m_editorUtils;

        private TabSet m_mainTabs;
        private TabSet m_moreTabs;

        //Extension manager
        bool m_needsScan = true;
        GaiaExtensionManager m_extensionMgr = new GaiaExtensionManager();
        private bool m_foldoutSession = false;
        private bool m_foldoutTerrain = false;
        private bool m_foldoutSpawners = false;
        private bool m_foldoutCharacters = false;
        private bool m_foldoutUtils = false;

        // Icon tests
        private Texture2D m_stdIcon;
        private Texture2D m_advIcon;
        private Texture2D m_gxIcon;
        private Texture2D m_moreIcon;

        [SerializeField]
#if !UNITY_2018_1_OR_NEWER
        private string m_dependsInstalled;
#endif
        private string m_standardAssets;
        private string m_speedTreesB;
        private string m_speedTreesC;
        private string m_speedTreesP;
        public bool PositionChecked { get; set; }
        #endregion

        #region Gaia Menu Items
        /// <summary>
        /// Show Gaia Manager editor window
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/Gaia/Show Gaia Manager... %g", false, 40)]
        public static void ShowGaiaManager()
        {
            var manager = EditorWindow.GetWindow<Gaia.GaiaManagerEditor>(false, "Gaia Manager");
            //Manager can be null if the dependency package installation is started upon opening the manager window.
            if (manager != null)
            {
                manager.Show();
            }
        }

        ///// <summary>
        ///// Show the forum
        ///// </summary>
        //[MenuItem("Window/Gaia/Show Forum...", false, 60)]
        //public static void ShowForum()
        //{
        //    Application.OpenURL(
        //        "http://www.procedural-worlds.com/forum/gaia/");
        //}

        /// <summary>
        /// Show documentation
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/Gaia/Show Extensions...", false, 65)]
        public static void ShowExtensions()
        {
            Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=gaia-extensions");
        }
        #endregion

        #region Constructors destructors and related delegates

        /// <summary>
        /// Setup on destroy
        /// </summary>
        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        /// <summary>
        /// See if we can preload the manager with existing settings
        /// </summary>
        void OnEnable()
        {
            if (EditorGUIUtility.isProSkin)
            {
                if (m_stdIcon == null)
                {
                    m_stdIcon = Resources.Load("gstdIco_p") as Texture2D;
                }
                if (m_advIcon == null)
                {
                    m_advIcon = Resources.Load("gadvIco_p") as Texture2D;
                }
                if (m_gxIcon == null)
                {
                    m_gxIcon = Resources.Load("ggxIco_p") as Texture2D;
                }
                if (m_moreIcon == null)
                {
                    m_moreIcon = Resources.Load("gmoreIco_p") as Texture2D;
                }
            }
            else
            {
                if (m_stdIcon == null)
                {
                    m_stdIcon = Resources.Load("gstdIco") as Texture2D;
                }
                if (m_advIcon == null)
                {
                    m_advIcon = Resources.Load("gadvIco") as Texture2D;
                }
                if (m_gxIcon == null)
                {
                    m_gxIcon = Resources.Load("ggxIco") as Texture2D;
                }
                if (m_moreIcon == null)
                {
                    m_moreIcon = Resources.Load("gmoreIco") as Texture2D;
                }
            }

            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            var mainTabs = new Tab[] {
                new Tab ("Standard", m_stdIcon, StandardTab),
                new Tab ("Advanced", m_advIcon, AdvancedTab),
                new Tab ("GX", m_gxIcon, ExtensionsTab),
                new Tab ("More...", m_moreIcon, MoreTab),
            };
            var moreTabs = new Tab[] {
                new Tab ("Tutorials & Support", TutorialsAndSupportTab),
                new Tab ("Partners & Extensions", MoreOnProceduralWorldsTab),
            };

            m_mainTabs = new TabSet(m_editorUtils, mainTabs);
            m_moreTabs = new TabSet(m_editorUtils, moreTabs);

            //Signal we need a scan
            m_needsScan = true;

            //Set the Gaia directories up
            GaiaUtils.CreateGaiaAssetDirectories();

            //Get or create existing settings object
            if (m_settings == null)
            {
                m_settings = (GaiaSettings)GaiaCommon1.AssetUtils.GetAssetScriptableObject("GaiaSettings");
                if (m_settings == null)
                {
                    m_settings = CreateSettingsAsset();
                }
            }

            //Make sure we have defaults
            if (m_settings.m_currentDefaults == null)
            {
                m_settings.m_currentDefaults = (GaiaDefaults)GaiaCommon1.AssetUtils.GetAssetScriptableObject("GaiaDefaults");
                EditorUtility.SetDirty(m_settings);
            }

            //Grab first resource we can find
            if (m_settings.m_currentResources == null)
            {
                m_settings.m_currentResources = (GaiaResource)GaiaCommon1.AssetUtils.GetAssetScriptableObject("GaiaResources");
                EditorUtility.SetDirty(m_settings);
            }

            //Grab first game object resource we can find
            if (m_settings.m_currentGameObjectResources == null)
            {
                m_settings.m_currentGameObjectResources = m_settings.m_currentResources;
                EditorUtility.SetDirty(m_settings);
            }

            if (!Application.isPlaying)
            {
                StartEditorUpdates();
                m_updateCoroutine = GetNewsUpdate();
            }

            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.Log("Gaia Settings are missing from our project, please make sure Gaia settings is in your project.");
                return;
            }

            //Sets up the render to the correct pipeline
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                m_gaiaSettings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
            }
            else if (GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("HDRenderPipelineAsset"))
            {
                m_gaiaSettings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.HighDefinition2018x;
            }
            else
            {
                m_gaiaSettings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.LightWeight2018x;
            }

#if !UNITY_POST_PROCESSING_STACK_V2 && UNITY_2018_1_OR_NEWER
            if (EditorUtility.DisplayDialog("Missing Post Processing V2", "We're about to import post processing v2 from the package manager. This process may take a few minutes and will setup your current scenes environment.", "OK"))
            {
                GaiaPipelineUtilsEditor.ShowGaiaPipelineUtilsEditor(m_gaiaSettings.m_currentRenderer, m_gaiaSettings.m_currentRenderer, false, this, false);               

                return;
            }
#endif

#if UNITY_POST_PROCESSING_STACK_V2 && !UNITY_2018_1_OR_NEWER
            m_dependsInstalled = "PostProcessing-2";
#endif

#if !UNITY_2018_1_OR_NEWER
            if (string.IsNullOrEmpty(m_dependsInstalled))
            {
                ImportDependsOnGaiaStartUp();    
                return;
            }
#else
            m_standardAssets = GetAssetPath("Characters");
            m_speedTreesB = GetAssetPath("Broadleaf");
            m_speedTreesC = GetAssetPath("Conifer");
            m_speedTreesP = GetAssetPath("Palm");

            if (string.IsNullOrEmpty(m_standardAssets) || string.IsNullOrEmpty(m_speedTreesB) || string.IsNullOrEmpty(m_speedTreesC) || string.IsNullOrEmpty(m_speedTreesP))
            {
                ImportDependsOnGaiaStartUp();
                return;
            }
#endif
        }

        /// <summary>
        /// Settings up settings on disable
        /// </summary>
        void OnDisable()
        {
            StopEditorUpdates();
        }

        /// <summary>
        /// Creates a new Gaia settings asset
        /// </summary>
        /// <returns>New gaia settings asset</returns>
        public static GaiaSettings CreateSettingsAsset()
        {
            GaiaSettings settings = ScriptableObject.CreateInstance<Gaia.GaiaSettings>();
            AssetDatabase.CreateAsset(settings, "Assets/Gaia/Data/GaiaSettings.asset");
            AssetDatabase.SaveAssets();
            return settings;
        }

        #endregion

        #region Tabs
        /// <summary>
        /// Draw the brief editor
        /// </summary>
        void StandardTab()
        {
            EditorGUI.indentLevel++;

            if (m_editorUtils.ClickableText("Follow the workflow to create your scene. Click here for tutorials."))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=tutorials");
            }
            GUILayout.Space(5f);

            //Add in a check for linear deferred lighting
            if (m_settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.Desktop ||
                m_settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.PowerfulDesktop)
            {
                var tier1 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1);
                var tier2 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2);
                var tier3 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3);
                if (PlayerSettings.colorSpace != ColorSpace.Linear || tier1.renderingPath != RenderingPath.DeferredShading)
                {
                    if (m_editorUtils.ButtonAutoIndent("0. Set Linear Deferred"))
                    {
                        var manager = GetWindow<GaiaManagerEditor>();

                        if (EditorUtility.DisplayDialog(
                        m_editorUtils.GetTextValue("SettingLinearDeferred"),
                        m_editorUtils.GetTextValue("SetLinearDeferred"),
                        m_editorUtils.GetTextValue("Yes"), m_editorUtils.GetTextValue("Cancel")))
                        {
                            manager.Close();

                            PlayerSettings.colorSpace = ColorSpace.Linear;

                            tier1.renderingPath = RenderingPath.DeferredShading;
                            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1, tier1);

                            tier2.renderingPath = RenderingPath.DeferredShading;
                            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2, tier2);

                            tier3.renderingPath = RenderingPath.DeferredShading;
                            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3, tier3);

#if UNITY_2018_1_OR_NEWER && !UNITY_2019_1_OR_NEWER
                            LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.ProgressiveCPU;
#elif UNITY_2019_1_OR_NEWER
                            LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.ProgressiveGPU;
#endif

#if UNITY_2018_1_OR_NEWER
                            Lightmapping.realtimeGI = true;
                            Lightmapping.bakedGI = true;
                            LightmapEditorSettings.realtimeResolution = 2f;
                            LightmapEditorSettings.bakeResolution = 40f;
                            Lightmapping.indirectOutputScale = 2f;
                            RenderSettings.defaultReflectionResolution = 256;
                            if (QualitySettings.shadowDistance < 350f)
                            {
                                QualitySettings.shadowDistance = 350f;
                            }
#else
                            if (QualitySettings.shadowDistance < 250f)
                            {
                                QualitySettings.shadowDistance = 250f;
                            }
#endif
                            if (Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.Iterative)
                            {
                                Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
                            }

                            if (GameObject.Find("Directional light") != null)
                            {
                                RenderSettings.sun = GameObject.Find("Directional light").GetComponent<Light>();
                            }
                            else if (GameObject.Find("Directional Light") != null)
                            {
                                RenderSettings.sun = GameObject.Find("Directional Light").GetComponent<Light>();
                            }
                        }
                    }
                }
            }

            if (m_editorUtils.ButtonAutoIndent("1. Create Terrain & Show Stamper"))
            {
                ShowSessionManager();
                CreateTerrain();
                ShowStamper();
            }

            EditorGUI.indentLevel++;
            if (m_editorUtils.ButtonAutoIndent("1A. Enhance Terrain"))
            {
                ShowTerrainUtilties();
            }
            EditorGUI.indentLevel--;

            if (m_editorUtils.ButtonAutoIndent("2. Create Spawners"))
            {
                //Only do this if we have 1 terrain
                if (!DisplayErrorIfInvalidTerrainCount(1))
                {
                    Spawner spawner;
                    //Create the spawners
                    spawner = CreateTextureSpawner().GetComponent<Spawner>();
                    if (spawner.m_activeRuleCnt == 0)
                    {
                        DestroyImmediate(spawner.gameObject);
                    }
                    spawner = CreateCoverageGameObjectSpawner().GetComponent<Spawner>();
                    if (spawner.m_activeRuleCnt == 0)
                    {
                        DestroyImmediate(spawner.gameObject);
                    }
                    spawner = CreateClusteredTreeSpawnerFromTerrainTrees().GetComponent<Spawner>();
                    if (spawner.m_activeRuleCnt == 0)
                    {
                        DestroyImmediate(spawner.gameObject);
                    }
                    spawner = CreateClusteredTreeSpawnerFromGameObjects().GetComponent<Spawner>();
                    if (spawner.m_activeRuleCnt == 0)
                    {
                        DestroyImmediate(spawner.gameObject);
                    }
                    spawner = CreateCoverageTreeSpawner().GetComponent<Spawner>();
                    if (spawner.m_activeRuleCnt == 0)
                    {
                        DestroyImmediate(spawner.gameObject);
                    }
                    spawner = CreateCoverageTreeSpawnerFromGameObjects().GetComponent<Spawner>();
                    if (spawner.m_activeRuleCnt == 0)
                    {
                        DestroyImmediate(spawner.gameObject);
                    }
                    spawner = CreateDetailSpawner().GetComponent<Spawner>();
                    if (spawner.m_activeRuleCnt == 0)
                    {
                        DestroyImmediate(spawner.gameObject);
                    }
                }
            }

            string buttonLabel;
            if (m_settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.UltraLight ||
                m_settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.MobileAndVR)
            {
                buttonLabel = "3. Create Player, Screenshotter, Skies, Water";
            }
            else
            {
                buttonLabel = "3. Create Player, Post FX, Screenshotter, Skies, Water & Wind";
            }
            if (m_editorUtils.ButtonAutoIndent(buttonLabel))
            {
                //Only do this if we have 1 terrain
                if (DisplayErrorIfInvalidTerrainCount(1))
                {
                    return;
                }

                if (EditorUtility.DisplayDialog("Adding Ambient Skies Samples", "You're about to add HDRI sky, Post Processing, Ambient Skies Sample Water and Player. Would you like to proceed?", "Yes", "No"))
                {

                    CreatePlayer();

                    if (m_settings.m_currentEnvironment != GaiaConstants.EnvironmentTarget.UltraLight && m_settings.m_currentEnvironment != GaiaConstants.EnvironmentTarget.MobileAndVR)
                    {
                        CreateSky();
                        CreateWater();
                        CreateScreenShotter();
                        CreateWindZone();
                    }
                    else
                    {
                        CreateSky();
                        CreateWater();
                        CreateScreenShotter();
                    }
                }
            }

            if (Lightmapping.isRunning)
            {
                if (m_editorUtils.ButtonAutoIndent("4. Cancel Bake"))
                {
                    Lightmapping.Cancel();
                    Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
                }
            }
            else
            {
                if (m_editorUtils.ButtonAutoIndent("4. Bake Lighting"))
                {
                    if (EditorUtility.DisplayDialog(
                        m_editorUtils.GetTextValue("BakingLightmaps!"),
                        m_editorUtils.GetTextValue("BakingLightmapsInfo"),
                        m_editorUtils.GetTextValue("Bake"), m_editorUtils.GetTextValue("Cancel")))
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
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draw the detailed editor
        /// </summary>
        void AdvancedTab()
        {
            EditorGUI.indentLevel++;

            //            if (DrawLinkHeaderText("Advanced Workflow"))
            //            {
            //                Application.OpenURL("http://www.procedural-worlds.com/gaia/tutorials/import-real-world-terrain/");
            //            }

            if (m_editorUtils.ClickableText("Pick and choose your tasks. Click here for tutorials."))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=tutorials");
            }

            GUILayout.Space(5f);

            if (m_foldoutSession = m_editorUtils.Foldout(m_foldoutSession, "1. Create Session Manager..."))
            {
                EditorGUI.indentLevel++;
                if (m_editorUtils.ButtonAutoIndent("Show Session Manager"))
                {
                    ShowSessionManager();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            if (m_foldoutTerrain = m_editorUtils.Foldout(m_foldoutTerrain, "2. Create your Terrain..."))
            {
                EditorGUI.indentLevel++;
                if (m_editorUtils.ButtonAutoIndent("Create Terrain"))
                {
                    CreateTerrain();
                }
                if (m_editorUtils.ButtonAutoIndent("Show Stamper"))
                {
                    ShowStamper();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            if (m_foldoutSpawners = m_editorUtils.Foldout(m_foldoutSpawners, "3. Create and configure your Spawners..."))
            {
                EditorGUI.indentLevel++;
                //if (m_editorUtils.ButtonAutoIndent("Create Stamp Spawner"))
                //{
                //    Selection.activeObject = CreateStampSpawner();
                //}
                if (m_editorUtils.ButtonAutoIndent("Create Coverage Texture Spawner"))
                {
                    Selection.activeObject = CreateTextureSpawner();
                }
                if (m_editorUtils.ButtonAutoIndent("Create Clustered Grass Spawner"))
                {
                    Selection.activeObject = CreateClusteredDetailSpawner();
                }
                if (m_editorUtils.ButtonAutoIndent("Create Coverage Grass Spawner"))
                {
                    Selection.activeObject = CreateDetailSpawner();
                }
                if (m_editorUtils.ButtonAutoIndent("Create Clustered Terrain Tree Spawner"))
                {
                    Selection.activeObject = CreateClusteredTreeSpawnerFromTerrainTrees();
                }
                if (m_editorUtils.ButtonAutoIndent("Create Clustered Prefab Tree Spawner"))
                {
                    Selection.activeObject = CreateClusteredTreeSpawnerFromGameObjects();
                }
                if (m_editorUtils.ButtonAutoIndent("Create Coverage Terrain Tree Spawner"))
                {
                    Selection.activeObject = CreateCoverageTreeSpawner();
                }
                if (m_editorUtils.ButtonAutoIndent("Create Coverage Prefab Tree Spawner"))
                {
                    Selection.activeObject = CreateCoverageTreeSpawnerFromGameObjects();
                }
                if (m_editorUtils.ButtonAutoIndent("Create Clustered Prefab Spawner"))
                {
                    Selection.activeObject = CreateClusteredGameObjectSpawner();
                }
                if (m_editorUtils.ButtonAutoIndent("Create Coverage Prefab Spawner"))
                {
                    Selection.activeObject = CreateCoverageGameObjectSpawner();
                }
                //if (m_editorUtils.ButtonAutoIndent("Create Group Spawner"))
                //{
                //    Selection.activeObject = FindOrCreateGroupSpawner();
                //}
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            if (m_foldoutCharacters = m_editorUtils.Foldout(m_foldoutCharacters, "4. Add common Game Objects..."))
            {
                EditorGUI.indentLevel++;
                if (m_editorUtils.ButtonAutoIndent("Add Character"))
                {
                    
                        Selection.activeGameObject = CreatePlayer();

//#if GAIA_PRESENT
//                    GameObject underwaterFX = GameObject.Find("Directional Light");
//                    GaiaReflectionProbeUpdate theProbeUpdater = FindObjectOfType<GaiaReflectionProbeUpdate>();
//                    GaiaUnderWaterEffects effectsSettings = underwaterFX.GetComponent<GaiaUnderWaterEffects>();
//                    if (theProbeUpdater != null && effectsSettings != null)
//                    {
//#if UNITY_EDITOR
//                        effectsSettings.player = effectsSettings.GetThePlayer();
//#endif
//                    }
//#endif
                }
                if (m_editorUtils.ButtonAutoIndent("Add Wind Zone"))
                {
                    Selection.activeGameObject = CreateWindZone();
                }
                if (m_editorUtils.ButtonAutoIndent("Add Water"))
                {
                    Selection.activeGameObject = CreateWater();
                }
                if (m_editorUtils.ButtonAutoIndent("Add Screen Shotter"))
                {
                    Selection.activeGameObject = CreateScreenShotter();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            if (m_foldoutUtils = m_editorUtils.Foldout(m_foldoutUtils, "5. Handy Utilities..."))
            {
                EditorGUI.indentLevel++;
                if (m_editorUtils.ButtonAutoIndent("Show Scanner"))
                {
                    Selection.activeGameObject = CreateScanner();
                }
                if (m_editorUtils.ButtonAutoIndent("Show Visualiser"))
                {
                    Selection.activeGameObject = ShowVisualiser();
                }
                if (m_editorUtils.ButtonAutoIndent("Show Terrain Utilities"))
                {
                    ShowTerrainUtilties();
                }
                if (m_editorUtils.ButtonAutoIndent("Show Splatmap Exporter"))
                {
                    ShowTexureMaskExporter();
                }
                if (m_editorUtils.ButtonAutoIndent("Show Grass Exporter"))
                {
                    ShowGrassMaskExporter();
                }
                if (m_editorUtils.ButtonAutoIndent("Show Mesh Exporter"))
                {
                    ShowTerrainObjExporter();
                }
                if (m_editorUtils.ButtonAutoIndent("Show Shore Exporter"))
                {
                    ExportShoremaskAsPNG();
                }
                if (m_editorUtils.ButtonAutoIndent("Show Extension Exporter"))
                {
                    ShowExtensionExporterEditor();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            m_editorUtils.LabelField(m_editorUtils.GetTextValue("Celebrate!"), m_wrapStyle);
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draw the extension editor
        /// </summary>
        void ExtensionsTab()
        {
            EditorGUI.indentLevel++;

            if (m_editorUtils.ClickableText(
                "Gaia eXtensions accelerate and simplify development by integrating quality assets. This tab shows the extensions for the products you've installed. Click here to see more extensions.")
            )
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=gaia-extensions");
            }
            GUILayout.Space(5f);

            //And scan if something has changed
            if (m_needsScan)
            {
                m_extensionMgr.ScanForExtensions();
                if (m_extensionMgr.GetInstalledExtensionCount() != 0)
                {
                    m_needsScan = false;
                }
            }

            int methodIdx = 0;
            string cmdName;
            string currFoldoutName = "";
            string prevFoldoutName = "";
            MethodInfo command;
            string[] cmdBreakOut = new string[0];
            List<GaiaCompatiblePackage> packages;
            List<GaiaCompatiblePublisher> publishers = m_extensionMgr.GetPublishers();

            foreach (GaiaCompatiblePublisher publisher in publishers)
            {
                if (publisher.InstalledPackages() > 0)
                {
                    if (publisher.m_installedFoldedOut = m_editorUtils.Foldout(publisher.m_installedFoldedOut, new GUIContent(publisher.m_publisherName)))
                    {
                        EditorGUI.indentLevel++;

                        packages = publisher.GetPackages();
                        foreach (GaiaCompatiblePackage package in packages)
                        {
                            if (package.m_isInstalled)
                            {
                                if (package.m_installedFoldedOut = m_editorUtils.Foldout(package.m_installedFoldedOut, new GUIContent(package.m_packageName)))
                                {
                                    EditorGUI.indentLevel++;

                                    //Now loop thru and process
                                    while (methodIdx < package.m_methods.Count)
                                    {
                                        command = package.m_methods[methodIdx];
                                        cmdBreakOut = command.Name.Split('_');

                                        //Ignore if we are not a valid thing
                                        if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                        {
                                            methodIdx++;
                                            continue;
                                        }

                                        //Get foldout and command name
                                        if (cmdBreakOut.GetLength(0) == 2)
                                        {
                                            currFoldoutName = "";
                                        }
                                        else
                                        {
                                            currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                        }
                                        cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                        if (currFoldoutName == "")
                                        {
                                            methodIdx++;
                                            if (m_editorUtils.ButtonAutoIndent(new GUIContent(cmdName)))
                                            {
                                                command.Invoke(null, null);
                                            }
                                        }
                                        else
                                        {
                                            prevFoldoutName = currFoldoutName;

                                            //Make sure we have it in our dictionary
                                            if (!package.m_methodGroupFoldouts.ContainsKey(currFoldoutName))
                                            {
                                                package.m_methodGroupFoldouts.Add(currFoldoutName, false);
                                            }

                                            if (package.m_methodGroupFoldouts[currFoldoutName] = m_editorUtils.Foldout(package.m_methodGroupFoldouts[currFoldoutName], new GUIContent(currFoldoutName)))
                                            {
                                                EditorGUI.indentLevel++;

                                                while (methodIdx < package.m_methods.Count && currFoldoutName == prevFoldoutName)
                                                {
                                                    command = package.m_methods[methodIdx];
                                                    cmdBreakOut = command.Name.Split('_');

                                                    //Drop out if we are not a valid thing
                                                    if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                                    {
                                                        methodIdx++;
                                                        continue;
                                                    }

                                                    //Get foldout and command name
                                                    if (cmdBreakOut.GetLength(0) == 2)
                                                    {
                                                        currFoldoutName = "";
                                                    }
                                                    else
                                                    {
                                                        currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                                    }
                                                    cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                                    if (currFoldoutName != prevFoldoutName)
                                                    {
                                                        continue;
                                                    }

                                                    if (m_editorUtils.ButtonAutoIndent(new GUIContent(cmdName)))
                                                    {
                                                        command.Invoke(null, null);
                                                    }

                                                    methodIdx++;
                                                }

                                                EditorGUI.indentLevel--;
                                            }
                                            else
                                            {
                                                while (methodIdx < package.m_methods.Count && currFoldoutName == prevFoldoutName)
                                                {
                                                    command = package.m_methods[methodIdx];
                                                    cmdBreakOut = command.Name.Split('_');

                                                    //Drop out if we are not a valid thing
                                                    if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                                    {
                                                        methodIdx++;
                                                        continue;
                                                    }

                                                    //Get foldout and command name
                                                    if (cmdBreakOut.GetLength(0) == 2)
                                                    {
                                                        currFoldoutName = "";
                                                    }
                                                    else
                                                    {
                                                        currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                                    }
                                                    cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                                    if (currFoldoutName != prevFoldoutName)
                                                    {
                                                        continue;
                                                    }

                                                    methodIdx++;
                                                }
                                            }
                                        }
                                    }

                                    /*
                                    foreach (MethodInfo command in package.m_methods)
                                    {
                                        cmdBreakOut = command.Name.Split('_');

                                        if ((cmdBreakOut.GetLength(0) == 2 || cmdBreakOut.GetLength(0) == 3) && cmdBreakOut[0] == "GX")
                                        {
                                            if (cmdBreakOut.GetLength(0) == 2)
                                            {
                                                currFoldoutName = "";
                                            }
                                            else
                                            {
                                                currFoldoutName = cmdBreakOut[1];
                                                Debug.Log(currFoldoutName);
                                            }

                                            cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");
                                            if (m_editorUtils.ButtonAutoIndent(new GUIContent(cmdName)))
                                            {
                                                command.Invoke(null, null);
                                            }
                                        }
                                    }
                                        */

                                    EditorGUI.indentLevel--;
                                }
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }

            EditorGUI.indentLevel--;
        }


        /// <summary>
        /// Draw the show more editor
        /// </summary>
        void MoreTab()
        {
            m_editorUtils.Tabs(m_moreTabs);
        }

        void TutorialsAndSupportTab()
        {
            EditorGUI.indentLevel++;
            m_editorUtils.Text("Review the QuickStart guide and other product documentation in the Gaia / Documentation directory.");
            GUILayout.Space(5f);

            if (m_settings.m_hideHeroMessage)
            {
                if (m_editorUtils.ClickableHeadingNonLocalized(m_settings.m_latestNewsTitle))
                {
                    Application.OpenURL(m_settings.m_latestNewsUrl);
                }

                m_editorUtils.TextNonLocalized(m_settings.m_latestNewsBody);
                GUILayout.Space(5f);
            }

            if (m_editorUtils.ClickableHeading("Video Tutorials"))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=tutorials");
            }
            m_editorUtils.Text("With over 45 video tutorials we cover everything you need to become an expert.");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Join Our Community"))
            {
                Application.OpenURL("https://discord.gg/rtKn8rw");
            }
            m_editorUtils.Text("Whether you need an answer now or feel like a chat our friendly discord community is a great place to learn!");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Ticketed Support"))
            {
                Application.OpenURL("https://proceduralworlds.freshdesk.com/support/home");
            }
            m_editorUtils.Text("Don't let your question get lost in the noise. All ticketed requests are answered, and usually within 48 hours.");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Help us Grow - Rate & Review!"))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/42618?aid=1101lSqC");
            }
            m_editorUtils.Text("Quality products are a huge investment to create & support. Please take a moment to show your appreciation by leaving a rating & review.");
            GUILayout.Space(5f);

            if (m_settings.m_hideHeroMessage)
            {
                if (m_editorUtils.ClickableHeading("Show Hero Message"))
                {
                    m_settings.m_hideHeroMessage = false;
                    EditorUtility.SetDirty(m_settings);
                }
                m_editorUtils.Text("Show latest news and hero messages in Gaia.");
                GUILayout.Space(5f);
            }
            EditorGUI.indentLevel--;
        }

        void MoreOnProceduralWorldsTab()
        {
            EditorGUI.indentLevel++;
            m_editorUtils.Text("Super charge your development with our amazing partners & extensions.");
            GUILayout.Space(5f);

            if (m_settings.m_hideHeroMessage)
            {
                if (m_editorUtils.ClickableHeadingNonLocalized(m_settings.m_latestNewsTitle))
                {
                    Application.OpenURL(m_settings.m_latestNewsUrl);
                }

                m_editorUtils.TextNonLocalized(m_settings.m_latestNewsBody);
                GUILayout.Space(5f);
            }

            if (m_editorUtils.ClickableHeading("Our Partners"))
            {
                Application.OpenURL("http://www.procedural-worlds.com/partners/");
            }
            m_editorUtils.Text("The content included with Gaia is an awesome starting point for your game, but that's just the tip of the iceberg. Learn more about how these talented publishers can help you to create amazing environments in Unity.");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Gaia eXtensions (GX)"))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=gaia-extensions");
            }
            m_editorUtils.Text("Gaia eXtensions accelerate and simplify your development by automating asset setup in your scene. Check out the quality assets we have integrated for you!");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Help Us to Grow - Spread The Word!"))
            {
                Application.OpenURL("https://www.facebook.com/proceduralworlds/");
            }
            m_editorUtils.Text("Get regular news updates and help us to grow by liking and sharing our Facebook page!");
            GUILayout.Space(5f);

            if (m_settings.m_hideHeroMessage)
            {
                if (m_editorUtils.ClickableHeading("Show Hero Message"))
                {
                    m_settings.m_hideHeroMessage = false;
                    EditorUtility.SetDirty(m_settings);
                }
                m_editorUtils.Text("Show latest news and hero messages in Gaia.");
                GUILayout.Space(5f);
            }
            EditorGUI.indentLevel--;
        }
#endregion

#region On GUI
        void OnGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.fontStyle = FontStyle.Normal;
                m_wrapStyle.wordWrap = true;
            }

            if (m_bodyStyle == null)
            {
                m_bodyStyle = new GUIStyle(GUI.skin.label);
                m_bodyStyle.fontStyle = FontStyle.Normal;
                m_bodyStyle.wordWrap = true;
            }

            if (m_titleStyle == null)
            {
                m_titleStyle = new GUIStyle(m_bodyStyle);
                m_titleStyle.fontStyle = FontStyle.Bold;
                m_titleStyle.fontSize = 20;
            }

            if (m_headingStyle == null)
            {
                m_headingStyle = new GUIStyle(m_bodyStyle);
                m_headingStyle.fontStyle = FontStyle.Bold;
            }

            if (m_linkStyle == null)
            {
                m_linkStyle = new GUIStyle(m_bodyStyle);
                m_linkStyle.wordWrap = false;
                m_linkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
                m_linkStyle.stretchWidth = false;
            }

            //Check for state of compiler
            if (EditorApplication.isCompiling)
            {
                m_needsScan = true;
            }

            m_editorUtils.GUIHeader();

            EditorGUILayout.BeginVertical(m_boxStyle);

            GaiaConstants.EnvironmentControllerType targetControllerType = (GaiaConstants.EnvironmentControllerType)m_editorUtils.EnumPopup("Controller", m_settings.m_currentController);
            GaiaConstants.EnvironmentTarget targetEnv = (GaiaConstants.EnvironmentTarget)m_editorUtils.EnumPopup("Environment", m_settings.m_currentEnvironment);
            GaiaConstants.EnvironmentRenderer targetRenderer = (GaiaConstants.EnvironmentRenderer)m_editorUtils.EnumPopup("Renderer", m_settings.m_currentRenderer);
            GaiaConstants.EnvironmentSize targetSize = (GaiaConstants.EnvironmentSize)m_editorUtils.EnumPopup("Terrain Size", m_settings.m_currentSize);

            if (targetRenderer != m_settings.m_currentRenderer)
            {
                if (Application.isPlaying)
                {
                    Debug.LogWarning("Can't switch render pipelines in play mode.");
                    targetRenderer = m_settings.m_currentRenderer;
                }
                else
                {
#if !UNITY_2018_3_OR_NEWER
                    EditorUtility.DisplayDialog("Pipeline change not supported", "Lightweight and High Definition is only supported in 2018.3 or higher. To use Gaia selected pipeline, please install and upgrade your project to 2018.3.x. Switching back to Built-In Pipeline.", "OK");
                    targetRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
#else
                    if (EditorUtility.DisplayDialog("CHANGE RENDER PIPELINE",
                                "You are about to install a new render pipeline!" +
                                "\nPlease BACKUP your project first!" +
                                "\nAre you sure?",
                                "Yes", "No"))
                    {

                        bool upgradeMaterials = false;
                        if (EditorUtility.DisplayDialog("UPGRADE MATERIALS",
                            "Upgrade materials to the " + targetRenderer.ToString() + " pipeline?" +
                            "\nWARNING: THIS PROCESS CAN NOT BE UNDONE!" +
                            "\nSay NO and change pipeline back if unsure!",
                            "Yes", "No"))
                        {
                            upgradeMaterials = true;
                        }

                        /*
                        bool finalizeEnvironment = false;
                        if (EditorUtility.DisplayDialog("FINALIZE ENVIRONMENT",
                            "Finalizing the environment will configure your scenes lighting and setup post processing. This will overwrite your current lighting and skybox settings. Would you like to finalize your environment?",
                            "Yes", "No"))
                        {
                            finalizeEnvironment = true;
                        }
                        */

                        GaiaPipelineUtilsEditor.ShowGaiaPipelineUtilsEditor(m_settings.m_currentRenderer, targetRenderer, upgradeMaterials, this, true);

                       
                    }
#endif
                }
            }

            bool needsUpdate = false;
            if (targetEnv != m_settings.m_currentEnvironment)
            {
                switch (targetEnv)
                {
                    case GaiaConstants.EnvironmentTarget.UltraLight:
                        m_settings.m_currentDefaults = m_settings.m_ultraLightDefaults;
                        m_settings.m_currentResources = m_settings.m_ultraLightResources;
                        m_settings.m_currentGameObjectResources = m_settings.m_ultraLightGameObjectResources;
                        m_settings.m_currentWaterPrefabName = m_settings.m_waterMobilePrefabName;
                        targetSize = GaiaConstants.EnvironmentSize.Is512MetersSq;
                        break;
                    case GaiaConstants.EnvironmentTarget.MobileAndVR:
                        m_settings.m_currentDefaults = m_settings.m_mobileDefaults;
                        m_settings.m_currentResources = m_settings.m_mobileResources;
                        m_settings.m_currentGameObjectResources = m_settings.m_mobileGameObjectResources;
                        m_settings.m_currentWaterPrefabName = m_settings.m_waterMobilePrefabName;
                        targetSize = GaiaConstants.EnvironmentSize.Is1024MetersSq;
                        break;
                    case GaiaConstants.EnvironmentTarget.Desktop:
                        m_settings.m_currentDefaults = m_settings.m_desktopDefaults;
                        m_settings.m_currentResources = m_settings.m_desktopResources;
                        m_settings.m_currentGameObjectResources = m_settings.m_desktopGameObjectResources;
                        m_settings.m_currentWaterPrefabName = m_settings.m_waterPrefabName;
                        targetSize = GaiaConstants.EnvironmentSize.Is2048MetersSq;
                        break;
                    case GaiaConstants.EnvironmentTarget.PowerfulDesktop:
                        m_settings.m_currentDefaults = m_settings.m_powerDesktopDefaults;
                        m_settings.m_currentResources = m_settings.m_powerDesktopResources;
                        m_settings.m_currentGameObjectResources = m_settings.m_powerDesktopGameObjectResources;
                        m_settings.m_currentWaterPrefabName = m_settings.m_waterPrefabName;
                        targetSize = GaiaConstants.EnvironmentSize.Is2048MetersSq;
                        break;
                }
                EditorUtility.SetDirty(m_settings);
                needsUpdate = true;
            }

            if (targetControllerType != m_settings.m_currentController)
            {
                m_settings.m_currentController = targetControllerType;
                switch (targetControllerType)
                {
                    case GaiaConstants.EnvironmentControllerType.FirstPerson:
                        m_settings.m_currentPlayerPrefabName = m_settings.m_fpsPlayerPrefabName;
                        break;
                    case GaiaConstants.EnvironmentControllerType.ThirdPerson:
                        m_settings.m_currentPlayerPrefabName = m_settings.m_3pPlayerPrefabName;
                        break;
                    //case GaiaConstants.EnvironmentControllerType.Rollerball:
                    //m_settings.m_currentPlayerPrefabName = m_settings.m_rbPlayerPrefabName;
                    //break;
                    case GaiaConstants.EnvironmentControllerType.FlyingCamera:
                        m_settings.m_currentPlayerPrefabName = "Flycam";
                        break;
                }
                EditorUtility.SetDirty(m_settings);
            }

            if (targetEnv != m_settings.m_currentEnvironment)
            {
                m_settings.m_currentEnvironment = targetEnv;
                EditorUtility.SetDirty(m_settings);
            }

            if (needsUpdate || targetSize != m_settings.m_currentSize)
            {
                m_settings.m_currentSize = targetSize;
                switch (targetSize)
                {
                    case GaiaConstants.EnvironmentSize.Is256MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 256;
                        break;
                    case GaiaConstants.EnvironmentSize.Is512MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 512;
                        break;
                    case GaiaConstants.EnvironmentSize.Is1024MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 1024;
                        break;
                    case GaiaConstants.EnvironmentSize.Is2048MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 2048;
                        break;
                    case GaiaConstants.EnvironmentSize.Is4096MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 4096;
                        break;
                    case GaiaConstants.EnvironmentSize.Is8192MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 8192;
                        break;
                    case GaiaConstants.EnvironmentSize.Is16384MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 16384;
                        break;
                }

                switch (targetEnv)
                {
                    case GaiaConstants.EnvironmentTarget.UltraLight:
                        m_settings.m_currentDefaults.m_heightmapResolution = 33;
                        m_settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 8, 256, 512);
                        m_settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 8, 128, 512);
                        m_settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 8, 64, 512);
                        m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 8, 64, 512);
                        break;
                    case GaiaConstants.EnvironmentTarget.MobileAndVR:
                        m_settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 64, 512) + 1;
                        m_settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 256, 512);
                        m_settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                        m_settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                        m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                        break;
                    case GaiaConstants.EnvironmentTarget.Desktop:
                        m_settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 2048) + 1;
                        m_settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                        m_settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 4096);
                        m_settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                        m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                        break;
                    case GaiaConstants.EnvironmentTarget.PowerfulDesktop:
                        m_settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 2048) + 1;
                        m_settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 2048);
                        m_settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 4096);
                        m_settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 2048);
                        m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 2048);
                        break;
                    case GaiaConstants.EnvironmentTarget.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(Mathf.Clamp()  , m_settings.m_currentDefaults.m_size);


                EditorUtility.SetDirty(m_settings);
                EditorUtility.SetDirty(m_settings.m_currentDefaults);
            }

            GUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            m_settings.m_currentDefaults = (GaiaDefaults)m_editorUtils.ObjectField("Terrain Defaults", m_settings.m_currentDefaults, typeof(GaiaDefaults), false);
            if (m_editorUtils.Button("New", GUILayout.Width(45), GUILayout.Height(16f)))
            {
                m_settings.m_currentDefaults = CreateDefaultsAsset();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            m_settings.m_currentResources = (GaiaResource)m_editorUtils.ObjectField("Terrain Resources", m_settings.m_currentResources, typeof(GaiaResource), false);
            if (m_editorUtils.Button("New", GUILayout.Width(45), GUILayout.Height(16f)))
            {
                m_settings.m_currentResources = CreateResourcesAsset();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_settings.m_currentGameObjectResources = (GaiaResource)m_editorUtils.ObjectField("GameObject Resources", m_settings.m_currentGameObjectResources, typeof(GaiaResource), false);
            if (m_editorUtils.Button("New", GUILayout.Width(45), GUILayout.Height(16f)))
            {
                m_settings.m_currentGameObjectResources = CreateResourcesAsset();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            m_editorUtils.TabsNoBorder(m_mainTabs);

            //Bottom section
            if (!m_settings.m_hideHeroMessage)
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal(m_boxStyle);
                    {
                        //                GUILayout.BeginVertical();
                        //                GUILayout.Space(3f);
                        //                DrawImage(m_settings.m_latestNewsImage, 50f, 50f);
                        //                GUILayout.EndVertical();
                        GUILayout.BeginVertical();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (m_editorUtils.ClickableHeadingNonLocalized(m_settings.m_latestNewsTitle))
                                {
                                    Application.OpenURL(m_settings.m_latestNewsUrl);
                                }

                                if (m_editorUtils.ClickableHeading("Hide", GUILayout.Width(33f)))
                                {
                                    m_settings.m_hideHeroMessage = true;
                                    EditorUtility.SetDirty(m_settings);
                                }
                            }
                            GUILayout.EndHorizontal();
                            m_editorUtils.TextNonLocalized(m_settings.m_latestNewsBody);
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
        }
#endregion

#region Gaia Main Function Calls
        /// <summary>
        /// Create and returns a defaults asset
        /// </summary>
        /// <returns>New defaults asset</returns>
        public static GaiaDefaults CreateDefaultsAsset()
        {
            GaiaDefaults defaults = ScriptableObject.CreateInstance<Gaia.GaiaDefaults>();
            AssetDatabase.CreateAsset(defaults, string.Format("Assets/Gaia/Data/GD-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
            AssetDatabase.SaveAssets();
            return defaults;
        }

        /// <summary>
        /// Create and returns a resources asset
        /// </summary>
        /// <returns>New resources asset</returns>
        public static GaiaResource CreateResourcesAsset()
        {
            GaiaResource resources = ScriptableObject.CreateInstance<Gaia.GaiaResource>();
            AssetDatabase.CreateAsset(resources, string.Format("Assets/Gaia/Data/GR-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
            AssetDatabase.SaveAssets();
            return resources;
        }

        /// <summary>
        /// Set up the Gaia Present defines
        /// </summary>
        public static void SetGaiaDefinesStatic()
        {
            string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            //Check for and inject GAIA_PRESENT
            if (!currBuildSettings.Contains("GAIA_PRESENT"))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings + ";GAIA_PRESENT");
            }
        }

        /// <summary>
        /// Create the terrain
        /// </summary>
        void CreateTerrain()
        {
            //Only do this if we have < 1 terrain
            int actualTerrainCount = Gaia.TerrainHelper.GetActiveTerrainCount();
            if (actualTerrainCount != 0)
            {
                EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("OOPS!"), string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to use this feature you need {1}. Please add or remove terrains."), actualTerrainCount, 0), m_editorUtils.GetTextValue("OK"));
            }
            else
            {
                //Disable automatic light baking - this kills perf on most systems
                Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;

                //Create the terrain
                m_settings.m_currentDefaults.CreateTerrain(m_settings.m_currentResources);

                //Adjust the scene view so you can see the terrain
                if (SceneView.lastActiveSceneView != null)
                {
                    if (m_settings != null)
                    {
                        SceneView.lastActiveSceneView.LookAtDirect(new Vector3(0f, 300f, -1f * (m_settings.m_currentDefaults.m_terrainSize / 2f)), Quaternion.Euler(30f, 0f, 0f));
                        Repaint();
                    }
                }
            }
        }

        /// <summary>
        /// Create / show the session manager
        /// </summary>
        GameObject ShowSessionManager(bool pickupExistingTerrain = false)
        {
            GameObject mgrObj = GaiaSessionManager.GetSessionManager(pickupExistingTerrain).gameObject;
            Selection.activeGameObject = mgrObj;
            return mgrObj;
        }

        /// <summary>
        /// Select or create a stamper
        /// </summary>
        void ShowStamper()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            //Make sure we have a session manager
            //m_sessionManager = m_resources.CreateOrFindSessionManager().GetComponent<GaiaSessionManager>();

            //Make sure we have gaia object
            GameObject gaiaObj = m_settings.m_currentResources.CreateOrFindGaia();

            //Create or find the stamper
            GameObject stamperObj = GameObject.Find("Stamper");
            if (stamperObj == null)
            {
                stamperObj = new GameObject("Stamper");
                stamperObj.transform.parent = gaiaObj.transform;
                Stamper stamper = stamperObj.AddComponent<Stamper>();
                stamper.m_resources = m_settings.m_currentResources;
                stamper.FitToTerrain();
                stamperObj.transform.position = new Vector3(stamper.m_x, stamper.m_y, stamper.m_z);
            }
            Selection.activeGameObject = stamperObj;
        }

        /// <summary>
        /// Select or create a scanner
        /// </summary>
        GameObject CreateScanner()
        {
            GameObject gaiaObj = m_settings.m_currentResources.CreateOrFindGaia();
            GameObject scannerObj = GameObject.Find("Scanner");
            if (scannerObj == null)
            {
                scannerObj = new GameObject("Scanner");
                scannerObj.transform.parent = gaiaObj.transform;
                scannerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
                Scanner scanner = scannerObj.AddComponent<Scanner>();

                //Load the material to draw it
                string matPath = GetAssetPath("GaiaScannerMaterial");
                if (!string.IsNullOrEmpty(matPath))
                {
                    scanner.m_previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }
            }
            return scannerObj;
        }

        /// <summary>
        /// Create or select the existing visualiser
        /// </summary>
        /// <returns>New or exsiting visualiser - or null if no terrain</returns>
        GameObject ShowVisualiser()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            GameObject gaiaObj = m_settings.m_currentResources.CreateOrFindGaia();
            GameObject visualiserObj = GameObject.Find("Visualiser");
            if (visualiserObj == null)
            {
                visualiserObj = new GameObject("Visualiser");
                visualiserObj.AddComponent<ResourceVisualiser>();
                visualiserObj.transform.parent = gaiaObj.transform;

                //Center it on the terrain
                visualiserObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();
            }
            ResourceVisualiser visualiser = visualiserObj.GetComponent<ResourceVisualiser>();
            visualiser.m_resources = m_settings.m_currentResources;
            return visualiserObj;
        }

        /// <summary>
        /// Show a normal exporter
        /// </summary>
        void ShowNormalMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaNormalExporterEditor>(false, m_editorUtils.GetTextValue("Normalmap Exporter"));
            export.Show();
        }

        /// <summary>
        /// Show the terrain height adjuster
        /// </summary>
        void ShowTerrainHeightAdjuster()
        {
            var export = EditorWindow.GetWindow<GaiaTerrainHeightAdjuster>(false, m_editorUtils.GetTextValue("Height Adjuster"));
            export.Show();
        }

        /// <summary>
        /// Show the terrain explorer helper
        /// </summary>
        void ShowTerrainUtilties()
        {
            var export = EditorWindow.GetWindow<GaiaTerrainExplorerEditor>(false, m_editorUtils.GetTextValue("Terrain Utilities"));
            export.Show();
        }

        /// <summary>
        /// Show a texture mask exporter
        /// </summary>
        void ShowTexureMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaMaskExporterEditor>(false, m_editorUtils.GetTextValue("Splatmap Exporter"));
            export.Show();
        }

        /// <summary>
        /// Show a grass mask exporter
        /// </summary>
        void ShowGrassMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaGrassMaskExporterEditor>(false, m_editorUtils.GetTextValue("Grassmask Exporter"));
            export.Show();
        }

        /// <summary>
        /// Show flowmap exporter
        /// </summary>
        void ShowFlowMapMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaWaterflowMapEditor>(false, m_editorUtils.GetTextValue("Flowmap Exporter"));
            export.Show();
        }

        /// <summary>
        /// Show a terrain obj exporter
        /// </summary>
        void ShowTerrainObjExporter()
        {
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<ExportTerrain>(false, m_editorUtils.GetTextValue("Export Terrain"));
            export.Show();
        }

        /// <summary>
        /// Export the world as a PNG heightmap
        /// </summary>
        void ExportWorldAsHeightmapPNG()
        {
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                string path = "Assets/GaiaMasks/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, GaiaCommon1.Utils.FixFileName(string.Format("Terrain-Heightmap-{0:yyyyMMdd-HHmmss}", DateTime.Now)));
                mgr.ExportWorldAsPng(path);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog(
                    m_editorUtils.GetTextValue("Export complete"),
                    m_editorUtils.GetTextValue(" Your heightmap has been saved to : ") + path,
                    m_editorUtils.GetTextValue("OK"));
            }
        }

        /// <summary>
        /// Export the shore mask as a png file
        /// </summary>
        void ExportShoremaskAsPNG()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<ShorelineMaskerEditor>(false, m_editorUtils.GetTextValue("Export Shore"));
            export.m_seaLevel = m_settings.m_currentResources.m_seaLevel;
            export.Show();
        }

        /// <summary>
        /// Show the extension exporter
        /// </summary>
        void ShowExtensionExporterEditor()
        {
            var export = EditorWindow.GetWindow<GaiaExtensionExporterEditor>(false, m_editorUtils.GetTextValue("Export GX"));
            export.Show();
        }

        /// <summary>
        /// Display an error if there is not exactly one terrain
        /// </summary>
        /// <param name="requiredTerrainCount">The amount required</param>
        /// <param name="feature">The feature name</param>
        /// <returns>True if an error, false otherwise</returns>
        private bool DisplayErrorIfInvalidTerrainCount(int requiredTerrainCount, string feature = "")
        {
            int actualTerrainCount = Gaia.TerrainHelper.GetActiveTerrainCount();
            if (actualTerrainCount != requiredTerrainCount)
            {
                if (string.IsNullOrEmpty(feature))
                {
                    if (actualTerrainCount < requiredTerrainCount)
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use this feature you need {1}. Please create a terrain!"), actualTerrainCount, requiredTerrainCount),
                            m_editorUtils.GetTextValue("OK"));
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use this feature you need {1}. Please remove terrain!"), actualTerrainCount, requiredTerrainCount),
                            m_editorUtils.GetTextValue("OK"));
                    }
                }
                else
                {
                    if (actualTerrainCount < requiredTerrainCount)
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use {2} you need {1}. Please create terrain!"), actualTerrainCount, requiredTerrainCount, feature),
                            m_editorUtils.GetTextValue("OK"));
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use {2} you need {1}. Please remove terrain!"), actualTerrainCount, requiredTerrainCount, feature),
                            m_editorUtils.GetTextValue("OK"));
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the range from the terrain
        /// </summary>
        /// <returns></returns>
        private float GetRangeFromTerrain()
        {
            float range = (m_settings.m_currentDefaults.m_terrainSize / 2) * m_settings.m_currentDefaults.m_tilesX;
            Terrain t = Gaia.TerrainHelper.GetActiveTerrain();
            if (t != null)
            {
                range = (Mathf.Max(t.terrainData.size.x, t.terrainData.size.z) / 2f) * m_settings.m_currentDefaults.m_tilesX;
            }
            return range;
        }

        /// <summary>
        /// Create a texture spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateTextureSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateCoverageTextureSpawner(GetRangeFromTerrain(), Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / (float)m_settings.m_currentDefaults.m_controlTextureResolution, 0.2f, 100f));
        }

        /// <summary>
        /// Create a detail spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateDetailSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateCoverageDetailSpawner(GetRangeFromTerrain(), Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / (float)m_settings.m_currentDefaults.m_detailResolution, 0.2f, 100f));
        }

        /// <summary>
        /// Create a clustered detail spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredDetailSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateClusteredDetailSpawner(GetRangeFromTerrain(), Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / (float)m_settings.m_currentDefaults.m_detailResolution, 0.2f, 100f));
        }

        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredTreeSpawnerFromTerrainTrees()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateClusteredTreeSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a tree spawner from game objecxts
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredTreeSpawnerFromGameObjects()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentGameObjectResources.CreateClusteredGameObjectSpawnerForTrees(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateCoverageTreeSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateCoverageTreeSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateCoverageTreeSpawnerFromGameObjects()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentGameObjectResources.CreateCoverageGameObjectSpawnerForTrees(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a game object spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateCoverageGameObjectSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentGameObjectResources.CreateCoverageGameObjectSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a game object spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredGameObjectSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentGameObjectResources.CreateClusteredGameObjectSpawner(GetRangeFromTerrain());
        }
#endregion

#region Create Step 3 (Player, water, sky etc)
        /// <summary>
        /// Create a player
        /// </summary>
        GameObject CreatePlayer()
        {
            //Gaia Settings to check pipeline selected
            GaiaSettings m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (m_gaiaSettings == null)
            {
                Debug.LogWarning("Gaia Settings are missing from your project, please make sure Gaia settings is in your project.");
                return null;
            }

            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            GameObject playerObj = null;
           
            //If nothing selected then make the default the fly cam
            string playerPrefabName = m_settings.m_currentPlayerPrefabName;
            if (string.IsNullOrEmpty(playerPrefabName))
            {
                playerPrefabName = "FlyCam";
            }

            GameObject mainCam = GameObject.Find("Main Camera");
            if (mainCam == null)
            {
                mainCam = GameObject.Find("Camera");
            }
            GameObject firstPersonController = GameObject.Find("FPSController");
            GameObject thirdPersonController = GameObject.Find("ThirdPersonController");
            GameObject flyCamController = GameObject.Find("FlyCam");
            GameObject flyCamControllerUI = GameObject.Find("FlyCamera UI");

            if (mainCam != null)
            {
                DestroyImmediate(mainCam);
            }
            if (firstPersonController != null)
            {
                DestroyImmediate(firstPersonController);
            }
            if (thirdPersonController != null)
            {
                DestroyImmediate(thirdPersonController);
            }
            if (flyCamController != null)
            {
                DestroyImmediate(flyCamController);
            }
            if (flyCamControllerUI != null)
            {
                DestroyImmediate(flyCamControllerUI);
            }

            //Get the centre of world at game height plus a bit
            Vector3 location = Gaia.TerrainHelper.GetActiveTerrainCenter(true);

            //Get the suggested camera far distance based on terrain scale
            Terrain terrain = GetActiveTerrain();
            float cameraDistance = Mathf.Clamp(terrain.terrainData.size.x, 250f, 2048) + 200f;

            //Create the player
            if (playerPrefabName == "Flycam")
            {
                playerObj = new GameObject();
                playerObj.name = "FlyCam";
                playerObj.tag = "MainCamera";
                playerObj.AddComponent<FlareLayer>();
#if !UNITY_2017_1_OR_NEWER
                playerObj.AddComponent<GUILayer>();
#endif
                playerObj.AddComponent<AudioListener>();
                playerObj.AddComponent<FreeCamera>();

                Camera cameraComponent = playerObj.GetComponent<Camera>();
                cameraComponent.farClipPlane = cameraDistance;
                if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                {
                    cameraComponent.allowHDR = false;
                    cameraComponent.allowMSAA = true;
                }
                else
                {
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
                }

                //Lift it to about eye height above terrain
                location.y += 1.8f;
                playerObj.transform.position = location;

                //Set up UI
                string flyCameraUIPath = GetAssetPath("FlyCamera UI");
                if (!string.IsNullOrEmpty(flyCameraUIPath))
                {
                    flyCamControllerUI = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(flyCameraUIPath));
                    flyCamControllerUI.name = "FlyCamera UI";
                    flyCamControllerUI.transform.SetParent(playerObj.transform);
                }
            }
            else if (playerPrefabName == "FPSController")
            {
                GameObject playerPrefab = GaiaCommon1.AssetUtils.GetAssetPrefab(playerPrefabName);
                if (playerPrefab != null)
                {
                    location.y += 1f;
                    playerObj = Instantiate(playerPrefab, location, Quaternion.identity) as GameObject;
                    playerObj.name = "FPSController";
                    playerObj.tag = "Player";
                    playerObj.transform.position = location;
                    if (playerObj.GetComponent<AudioSource>() != null)
                    {
                        AudioSource theAudioSource = playerObj.GetComponent<AudioSource>();
                        theAudioSource.volume = 0.125f;
                    }
                    Camera cameraComponent = playerObj.GetComponentInChildren<Camera>();
                    if (cameraComponent != null)
                    {
                        cameraComponent.farClipPlane = cameraDistance;
                        if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                        {
                            cameraComponent.allowHDR = false;
                            cameraComponent.allowMSAA = true;
                        }
                        else
                        {
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
                        }
                    }
                }
            }
            else if (playerPrefabName == "ThirdPersonController")
            {
                GameObject playerPrefab = GaiaCommon1.AssetUtils.GetAssetPrefab(playerPrefabName);
                if (playerPrefab != null)
                {
                    location.y += 0.05f;
                    playerObj = Instantiate(playerPrefab, location, Quaternion.identity) as GameObject;
                    playerObj.name = "ThirdPersonController";
                    playerObj.tag = "Player";
                    playerObj.transform.position = location;
                }

                mainCam = new GameObject("Main Camera");
                location.y += 1.5f;
                location.z -= 5f;
                mainCam.transform.position = location;
                Camera cameraComponent = mainCam.AddComponent<Camera>();
                cameraComponent.farClipPlane = cameraDistance;
                if (m_gaiaSettings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                {
                    cameraComponent.allowHDR = false;
                    cameraComponent.allowMSAA = true;
                }
                else
                {
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
                }

#if !UNITY_2017_1_OR_NEWER
                mainCam.AddComponent<GUILayer>();
#endif
                mainCam.AddComponent<FlareLayer>();
                mainCam.AddComponent<AudioListener>();
                mainCam.tag = "MainCamera";

                CameraController cameraController = mainCam.AddComponent<CameraController>();
                cameraController.target = playerObj;
                cameraController.targetHeight = 1.8f;
                cameraController.distance = 5f;
                cameraController.maxDistance = 20f;
                cameraController.minDistance = 2.5f;
            }

            if (playerObj != null)
            {
                //Set time of day
#if UNITY_POST_PROCESSING_STACK_V2 && GAIA_PRESENT && !AMBIENT_SKIES
                Gaia.GX.ProceduralWorlds.AmbientSkiesSamples.GX_PostProcessing_DefaultDay();
#endif

                //Adjust the scene view to see the camera
                if (SceneView.lastActiveSceneView != null)
                {
                    SceneView.lastActiveSceneView.LookAtDirect(playerObj.transform.position, playerObj.transform.rotation);
                    Repaint();
                }
            }
                       

            return playerObj;
        }

        /// <summary>
        /// Create a scene exporter object
        /// </summary>
        /*
        GameObject ShowSceneExporter()
        {
            GameObject exporterObj = GameObject.Find("Exporter");
            if (exporterObj == null)
            {
                exporterObj = new GameObject("Exporter");
                exporterObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
                GaiaExporter exporter = exporterObj.AddComponent<GaiaExporter>();
                GameObject gaiaObj = GameObject.Find("Gaia");
                if (gaiaObj != null)
                {
                    exporterObj.transform.parent = gaiaObj.transform;
                    exporter.m_rootObject = gaiaObj;
                }
                exporter.m_defaults = m_defaults;
                exporter.m_resources = m_resources;
                exporter.IngestGaiaSetup();
            }
            return exporterObj;
                     */

        /// <summary>
        /// Create a wind zone
        /// </summary>
        GameObject CreateWindZone()
        {
            GameObject windZoneObj = GameObject.Find("Wind Zone");
            if (windZoneObj == null)
            {
                windZoneObj = new GameObject("Wind Zone");
                windZoneObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
                WindZone windZone = windZoneObj.AddComponent<WindZone>();
                windZone.windMain = 0.2f;
                windZone.windTurbulence = 0.2f;
                windZone.windPulseMagnitude = 0.2f;
                windZone.windPulseFrequency = 0.05f;
                GameObject gaiaObj = GameObject.Find("Gaia Environment");
                if (gaiaObj == null)
                {
                    gaiaObj = new GameObject("Gaia Environment");
                }
                windZoneObj.transform.parent = gaiaObj.transform;
            }
            return windZoneObj;
        }

        /// <summary>
        /// Create water
        /// </summary>
        GameObject CreateWater()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }
#if GAIA_PRESENT && !AMBIENT_WATER
            Gaia.GX.ProceduralWorlds.AmbientWaterSamples.GX_Water_AddWater();
#endif

            return GameObject.Find("Ambient Water Sample");
        }

        /// <summary>
        /// Create the sky
        /// </summary>
        void CreateSky()
        {
#if GAIA_PRESENT && !AMBIENT_SKIES
            Gaia.GX.ProceduralWorlds.AmbientSkiesSamples.GX_Skies_Day();
            Gaia.GX.ProceduralWorlds.AmbientSkiesSamples.GX_Skies_AddGlobalReflectionProbe();
#else
            Debug.Log("Lighting could not be created because Ambient Skies exists in this project! Please use Ambinet Skies to set up lighting in your scene!");
#endif
        }

        /// <summary>
        /// Create and return a screen shotter object
        /// </summary>
        /// <returns></returns>
        GameObject CreateScreenShotter()
        {
            GameObject shotterObj = GameObject.Find("Screen Shotter");
            if (shotterObj == null)
            {
                shotterObj = new GameObject("Screen Shotter");
                Gaia.ScreenShotter shotter = shotterObj.AddComponent<Gaia.ScreenShotter>();
                shotter.m_watermark = GaiaCommon1.AssetUtils.GetAsset("Made With Gaia Watermark.png", typeof(Texture2D)) as Texture2D;

                GameObject gaiaObj = GameObject.Find("Gaia Environment");
                if (gaiaObj == null)
                {
                    gaiaObj = new GameObject("Gaia Environment");
                }
                shotterObj.transform.parent = gaiaObj.transform;
                shotterObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
            }

            return shotterObj;
        }
#endregion

#region System Helpers
        /// <summary>
        /// Get a clamped size value
        /// </summary>
        /// <param name="newSize"></param>
        /// <returns></returns>
        float GetClampedSize(float newSize)
        {
            return Mathf.Clamp(newSize, 32f, m_settings.m_currentDefaults.m_size);
        }

        /// <summary>
        /// Checks and imports depends packages
        /// </summary>
        public void ImportDependsOnGaiaStartUp()
        {
            //If not unity 2018.1 and post processing missing from defines
            //Note that in Unity 2018_1 and newer post processing will be installed anyways from the package manager
#if !UNITY_POST_PROCESSING_STACK_V2 && !UNITY_2018_1_OR_NEWER
            if (string.IsNullOrEmpty(m_dependsInstalled))
            {
                var manager = GetWindow<GaiaManagerEditor>("Gaia Manager");
                if (manager != null)
                {
                    manager.Close();
                }

                if (EditorUtility.DisplayDialog(
                    m_editorUtils.GetTextValue("OOPSMISSING!"),
                    m_editorUtils.GetTextValue("Missing Depends"),
                    m_editorUtils.GetTextValue("OK")))
                {                  
                    ImportPackage("Gaia Dependencies 2017.unitypackage");
                    m_dependsInstalled = "PostProcessing-2";
                }
                
                return;
            }
#else
            //If post processing defined but other path checks are missing
            m_standardAssets = GetAssetPath("Characters");
            m_speedTreesB = GetAssetPath("Broadleaf");
            m_speedTreesC = GetAssetPath("Conifer");
            m_speedTreesP = GetAssetPath("Palm");

            if (string.IsNullOrEmpty(m_standardAssets) || string.IsNullOrEmpty(m_speedTreesB) || string.IsNullOrEmpty(m_speedTreesC) || string.IsNullOrEmpty(m_speedTreesP))
            {
                if (EditorUtility.DisplayDialog(
                    m_editorUtils.GetTextValue("OOPSMISSING!"),
                    m_editorUtils.GetTextValue("Missing Depends"),
                    m_editorUtils.GetTextValue("OK")))
                {
#if UNITY_2018_1_OR_NEWER
                    ImportPackage("Gaia Dependencies 2018.unitypackage");
#else
                    ImportPackage("Gaia Dependencies 2017.unitypackage");
#endif
#if !UNITY_2018_1_OR_NEWER
                    m_dependsInstalled = "PostProcessing-2";
#endif
                }

                return;
            }
#endif
        }


#region Helper methods

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns></returns>
        private static string GetAssetPath(string name)
        {
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
#endif
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

#endregion

        /// <summary>
        /// Get the latest news from the web site at most once every 24 hours
        /// </summary>
        /// <returns></returns>
        IEnumerator GetNewsUpdate()
        {
            TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks - m_settings.m_lastWebUpdate);
            if (elapsed.TotalHours < 24.0)
            {
                StopEditorUpdates();
            }
            else
            {
                if (PWApp.CONF != null)
                {
#if UNITY_2018_3_OR_NEWER
                    using (UnityWebRequest www = new UnityWebRequest("http://www.procedural-worlds.com/gaiajson.php?gv=gaia-" + PWApp.CONF.Version))
                    {
                        while (!www.isDone)
                        {
                            yield return www;
                        }

                        if (!string.IsNullOrEmpty(www.error))
                        {
                            //Debug.Log(www.error);
                        }
                        else
                        {
                            try
                            {
                                string result = www.url;
                                int first = result.IndexOf("####");
                                if (first > 0)
                                {
                                    result = result.Substring(first + 10);
                                    first = result.IndexOf("####");
                                    if (first > 0)
                                    {
                                        result = result.Substring(0, first);
                                        result = result.Replace("<br />", "");
                                        result = result.Replace("&#8221;", "\"");
                                        result = result.Replace("&#8220;", "\"");
                                        var message = JsonUtility.FromJson<GaiaMessages>(result);
                                        m_settings.m_latestNewsTitle = message.title;
                                        m_settings.m_latestNewsBody = message.bodyContent;
                                        m_settings.m_latestNewsUrl = message.url;
                                        m_settings.m_lastWebUpdate = DateTime.Now.Ticks;
                                        EditorUtility.SetDirty(m_settings);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //Debug.Log(e.Message);
                            }
                        }
                    }
#else
                    using (WWW www = new WWW("http://www.procedural-worlds.com/gaiajson.php?gv=gaia-" + PWApp.CONF.Version))
                    {
                        while (!www.isDone)
                        {
                            yield return www;
                        }

                        if (!string.IsNullOrEmpty(www.error))
                        {
                            //Debug.Log(www.error);
                        }
                        else
                        {
                            try
                            {
                                string result = www.text;
                                int first = result.IndexOf("####");
                                if (first > 0)
                                {
                                    result = result.Substring(first + 10);
                                    first = result.IndexOf("####");
                                    if (first > 0)
                                    {
                                        result = result.Substring(0, first);
                                        result = result.Replace("<br />", "");
                                        result = result.Replace("&#8221;", "\"");
                                        result = result.Replace("&#8220;", "\"");
                                        var message = JsonUtility.FromJson<GaiaMessages>(result);
                                        m_settings.m_latestNewsTitle = message.title;
                                        m_settings.m_latestNewsBody = message.bodyContent;
                                        m_settings.m_latestNewsUrl = message.url;
                                        m_settings.m_lastWebUpdate = DateTime.Now.Ticks;
                                        EditorUtility.SetDirty(m_settings);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //Debug.Log(e.Message);
                            }
                        }
                    }
                
#endif
                }
            }
            StopEditorUpdates();
        }

        /// <summary>
        /// Import Package
        /// </summary>
        /// <param name="packageName"></param>
        public static void ImportPackage(string packageName)
        {
            string packageGaia = AssetUtils.GetAssetPath(packageName);
            if (!string.IsNullOrEmpty(packageGaia))
            {
                AssetDatabase.ImportPackage(packageGaia, true);
            }
            else
                Debug.Log("Unable to find Gaia Dependencies.unitypackage");
        }

        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            EditorApplication.update += EditorUpdate;
        }

        //Stop editor updates
        public void StopEditorUpdates()
        {
            EditorApplication.update -= EditorUpdate;
        }

        /// <summary>
        /// This is executed only in the editor - using it to simulate co-routine execution and update execution
        /// </summary>
        void EditorUpdate()
        {
            if (m_updateCoroutine == null)
            {
                StopEditorUpdates();
            }
            else
            {
                m_updateCoroutine.MoveNext();
            }
        }
#endregion

#region GAIA eXtensions GX
        public static List<Type> GetTypesInNamespace(string nameSpace)
        {
            List<Type> gaiaTypes = new List<Type>();

            int assyIdx, typeIdx;
            System.Type[] types;
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (assyIdx = 0; assyIdx < assemblies.Length; assyIdx++)
            {
                if (assemblies[assyIdx].FullName.StartsWith("Assembly"))
                {
                    types = assemblies[assyIdx].GetTypes();
                    for (typeIdx = 0; typeIdx < types.Length; typeIdx++)
                    {
                        if (!string.IsNullOrEmpty(types[typeIdx].Namespace))
                        {
                            if (types[typeIdx].Namespace.StartsWith(nameSpace))
                            {
                                gaiaTypes.Add(types[typeIdx]);
                            }
                        }
                    }
                }
            }
            return gaiaTypes;
        }

        /// <summary>
        /// Return true if image FX have been included
        /// </summary>
        /// <returns></returns>
        public static bool GotImageFX()
        {
            List<Type> types = GetTypesInNamespace("UnityStandardAssets.ImageEffects");
            if (types.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

#endregion

#region Commented out tooltips
        ///// <summary>
        ///// The tooltips
        ///// </summary>
        //static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        //{
        //    { "Execution Mode", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
        //    { "Controller", "The type of control method that will be set up. " },
        //    { "Environment", "The type of environment that will be set up. This pre-configures your terrain settings to be better suited for the environment you are targeting. You can modify these setting by modifying the relevant terrain default settings." },
        //    { "Renderer", "The terrain renderer you are targeting. The 2018x renderers are only relevent when using Unity 2018 and above." },
        //    { "Terrain Size", "The size of the terrain you are setting up. Please be aware that larger terrain sizes are harder for Unity to render, and will result in slow frame rates. You also need to consider your target environment as well. A mobile or VR device will have problems with large terrains." },
        //    { "Terrain Defaults", "The default settings that will be used when creating new terrains." },
        //    { "Terrain Resources", "The texture, detail and tree resources that will be used when creating new terrains." },
        //    { "GameObject Resources", "The game object resources that will be passed to your GameObject spawners when creating new spawners." },
        //    { "1. Create Terrain & Show Stamper", "Creates your terrain based on the setting in the panel above. You use the stamper to terraform your terrain." },
        //    { "2. Create Spawners", "Creates the spawners based on your resources in the panel above. You use spawners to inject these resources into your scene." },
        //    { "3. Create Player, Water and Screenshotter", "Creates the things you most commonly need in your scene to make it playable." },
        //    { "3. Create Player, Wind, Water and Screenshotter", "Creates the things you most commonly need in your scene to make it playable." },
        //    { "Show Session Manager", "The session manager records stamping and spawning operations so that you can recreate your terrain later." },
        //    { "Create Terrain", "Creates a terrain based on your settings." },
        //    { "Create Coverage Texture Spawner", "Creates a texture spawner so you can paint your terrain." },
        //    { "Create Coverage Grass Spawner", "Creates a grass (terrain details) spawner so you can cover your terrain with grass." },
        //    { "Create Clustered Grass Spawner", "Creates a grass (terrain details) spawner so you can cover your terrain with patches with grass." },
        //    { "Create Coverage Terrain Tree Spawner", "Creates a terrain tree spawner so you can cover your terrain with trees." },
        //    { "Create Clustered Terrain Tree Spawner", "Creates a terrain tree spawner so you can cover your terrain with clusters with trees." },
        //    { "Create Coverage Prefab Tree Spawner", "Creates a tree spawner from prefabs so you can cover your terrain with trees." },
        //    { "Create Clustered Prefab Tree Spawner", "Creates a tree spawner from prefabs so you can cover your terrain with clusters with trees." },
        //    { "Create Coverage Prefab Spawner", "Creates a spawner from prefabs so you can cover your terrain with instantiations of those prefabs." },
        //    { "Create Clustered Prefab Spawner", "Creates a spawner from prefabs so you can cover your terrain with clusters of those prefabs." },
        //    { "Show Stamper", "Shows a stamper. Use the stamper to terraform your terrain." },
        //    { "Show Scanner", "Shows the scanner. Use the scanner to create new stamps from textures, world machine .r16 files, IBM 16 bit RAW file, MAC 16 bit RAW files, Terrains, and Meshes (with mesh colliders)." },
        //    { "Show Visualiser", "Shows the visualiser. Use the visualiser to visualise and configure fitness values for your resources." },
        //    { "Show Terrain Utilities", "Shows terrain utilities. These are a great way to add additional interest to your terrains." },
        //    { "Show Splatmap Exporter", "Shows splatmap exporter. Exports your texture splatmaps." },
        //    { "Show Grass Exporter", "Shows grass exporter. Exports your grass control maps." },
        //    { "Show Mesh Exporter", "Shows mesh exporter. Exports your terrain as a low poly mesh. Use in conjunction with Base Map Exporter and Normal Map Exporter in Terrain Utilties to create cool mesh features to use in the distance." },
        //    { "Show Shore Exporter", "Shows shore exporter. Exports a mask of your terrain shoreline." },
        //    { "Show Extension Exporter", "Shows extension exporter. Use extensions to save resource and spawner configurations for later use via the GX tab." },
        //};
#endregion
    }
}