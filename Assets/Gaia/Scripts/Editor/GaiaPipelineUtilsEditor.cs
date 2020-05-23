using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
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
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Gaia
{
    /// <summary>
    /// Handles all pipeline related code
    /// </summary>
    [Serializable]
    public class GaiaPipelineUtilsEditor : EditorWindow
    {
        #region Persisent Variables

        //Potential pipeline update states
        public enum PipelineStatus
        {
            Idle,
            Compiling,
            LoadPackageList,
            ProcessInstalledPackageList,
            InstallingPackages,
            InstallingWaterShaders,
            UpdatingMaterials,
            DeletingPackages,
            UpdatingBuildSettings,
            FinalizingEnvironment,
            FailedWithErrors,
            RemovingUnusedComponents,
            CleanUp,
            Success
        };

        //Original pipeline - where we are coming from
        [SerializeField]
        private GaiaConstants.EnvironmentRenderer m_originalPipeline = GaiaConstants.EnvironmentRenderer.BuiltIn;

        //New pipeline - where we are moving to
        [SerializeField]
        private GaiaConstants.EnvironmentRenderer m_pipelineToInstall = GaiaConstants.EnvironmentRenderer.BuiltIn;

        //Whether we should update materials or not
        [SerializeField]
        private bool m_updateMaterials = true;

        //Whether we should finalize the environment or not
        [SerializeField]
        private bool m_finalizeEnvironment = false;

        //What is the process we are executing
        [SerializeField]
        public string m_currentTask; 

        //Our current status
        [SerializeField]
        private PipelineStatus m_currentStatus = PipelineStatus.Idle;

        //Our previous status
        [SerializeField]
        private PipelineStatus m_previousStatus = PipelineStatus.Idle;

        //Our current status message
        [SerializeField]
        private string m_currentStatusMessage;

        //Package management related
#if UNITY_2018_1_OR_NEWER

        [SerializeField]
        private bool m_isAddingPackage = false;

        [SerializeField]
        private bool m_isRemovingPackage = false;

        [Serializable]
        public class PackageEntry
        {
            public string name;
            public string version;
        }
        [SerializeField]
        private List<PackageEntry> m_packagesToAdd;
        [SerializeField]
        private List<PackageEntry> m_packagesToRemove;
        [SerializeField]
        private Stack<int> m_missingPackages = new Stack<int>();
#endif

        #endregion

        #region Non Persisent Variables     

        public static GaiaManagerEditor m_gaiaManagerEditor;

        private GUIStyle m_boxStyle;
        private bool m_showDebug = false;
        private DateTime m_startTime;

#if UNITY_2018_1_OR_NEWER
        private AddRequest m_pmAddRequest = null;
        private RemoveRequest m_pmRemoveRequest = null;
        private ListRequest m_pmListRequest = null;
#endif

        #endregion

        #region External interface

        /// <summary>
        /// Show the pipeline utils editor
        /// </summary>
        public static void ShowGaiaPipelineUtilsEditor(GaiaConstants.EnvironmentRenderer oldRenderer, GaiaConstants.EnvironmentRenderer newRenderer, bool updateMaterials, GaiaManagerEditor gaiaManagerEditor, bool finalizeEnvironment)
        {
            m_gaiaManagerEditor = gaiaManagerEditor;

            var utilsWindow = ScriptableObject.CreateInstance<GaiaPipelineUtilsEditor>();

            if (utilsWindow != null)
            {
                utilsWindow.m_originalPipeline = oldRenderer;
                utilsWindow.m_pipelineToInstall = newRenderer;
                utilsWindow.m_currentTask = oldRenderer.ToString() + " --> " + newRenderer.ToString();
                utilsWindow.m_currentStatus = PipelineStatus.Idle;
                utilsWindow.m_previousStatus = PipelineStatus.Idle;
                utilsWindow.m_updateMaterials = updateMaterials;
                utilsWindow.m_finalizeEnvironment = finalizeEnvironment;

                utilsWindow.minSize = new Vector2(300f, 50f);
                var position = utilsWindow.position;
                position.width = 460f;
                position.height = 90f;
                position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height - 280f).center;
                utilsWindow.position = position;

                utilsWindow.ShowPopup();
            }
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// See if we can preload the manager with existing settings
        /// </summary>
        void OnEnable()
        {
            if (m_showDebug)
            {
                Debug.Log("Enabling : " + m_currentStatus.ToString());
            }

            //Start editor updates
            if (!Application.isPlaying)
            {
                StartEditorUpdates();
            }

            if (m_originalPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                //Removes Warning
            }

            if (m_updateMaterials)
            {
                //Removes Warning
            }

            if (m_finalizeEnvironment)
            {
                //Removes Warning
            }
        }

        /// <summary>
        /// On Disable
        /// </summary>
        void OnDisable()
        {
            StopEditorUpdates();
            if (m_showDebug)
            {
                Debug.Log("Disabling : " + m_currentStatus.ToString());
            }
        }

        /// <summary>
        /// On Destroy
        /// </summary>
        private void OnDestroy()
        {
            if (m_showDebug)
            {
                Debug.Log("Destroying");
            }
        }

        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }

        /// <summary>
        /// Stop editor updates
        /// </summary>
        public void StopEditorUpdates()
        {
            EditorApplication.update -= EditorUpdate;
        }

        /// <summary>
        /// Drive the update process
        /// </summary>
        void EditorUpdate()
        {
            //Check for state of compiler
            if (EditorApplication.isCompiling)
            {
                //Step into compilaton mode
                if (m_currentStatus != PipelineStatus.Compiling)
                {
                    m_previousStatus = m_currentStatus;
                    m_currentStatus = PipelineStatus.Compiling;
                    Repaint();
                }
            }
            else
            {
                //Step out of compilation mode
                if (m_currentStatus == PipelineStatus.Compiling)
                {
                    m_currentStatus = m_previousStatus;
                    m_startTime = DateTime.Now;
                    Repaint();
                }

                //Update state change timer
                if (m_currentStatus != m_previousStatus)
                {
                    m_startTime = DateTime.Now;
                    Repaint();
                }

                //Handle current state
                switch (m_currentStatus)
                {
                    case PipelineStatus.Idle:
                        m_previousStatus = m_currentStatus;
                        m_currentStatus = PipelineStatus.RemovingUnusedComponents;
                        if (m_gaiaManagerEditor != null)
                        {
                            m_gaiaManagerEditor.Close();
                            m_gaiaManagerEditor = null;
                        }
                        break;
#if UNITY_2018_1_OR_NEWER
                    case PipelineStatus.RemovingUnusedComponents:
                        RemoveObjectComponents();
                        break;
                    case PipelineStatus.LoadPackageList:
                        LoadPackageList();
                        break;
                    case PipelineStatus.ProcessInstalledPackageList:
                        ProcessInstalledPackageList();
                        break;
                    case PipelineStatus.InstallingPackages:
                        InstallPackages();
                        break;
                    case PipelineStatus.InstallingWaterShaders:
                        InstallWaterShaders();
                        break;
                    case PipelineStatus.UpdatingMaterials:
                        UpdateMaterials();
                        break;
                    case PipelineStatus.DeletingPackages:
                        DeletePackages();
                        break;
                    case PipelineStatus.UpdatingBuildSettings:
                        UpdateBuildSettings();
                        break;
                    case PipelineStatus.FinalizingEnvironment:
                        FinalizeEnvironment(m_finalizeEnvironment);
                        break;
                    case PipelineStatus.CleanUp:
                        CleanUpScene();
                        break;
#endif
                    case PipelineStatus.Success:
                        m_previousStatus = m_currentStatus;
                        Repaint();
                        if ((DateTime.Now - m_startTime).TotalSeconds > 5f)
                        {
                            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
                            gaiaSettings.m_currentRenderer = m_pipelineToInstall;
                            EditorUtility.SetDirty(gaiaSettings);
                            EditorSceneManager.MarkAllScenesDirty();
                            AssetDatabase.Refresh();

                            Close();
                        }
                        break;
                    case PipelineStatus.FailedWithErrors:
                        m_previousStatus = m_currentStatus;
                        Repaint();
                        if ((DateTime.Now - m_startTime).TotalSeconds > 5f)
                        {
                            Close();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Draw UX
        /// </summary>
        void OnGUI()
        {
            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Draw title
            GUILayout.BeginHorizontal("Pipeline Installer", m_boxStyle);
            EditorGUILayout.LabelField("");
            GUILayout.EndHorizontal();

            //Draw content
            EditorGUILayout.LabelField(new GUIContent("Current Task"), new GUIContent(m_currentTask));
            EditorGUILayout.LabelField(new GUIContent("Current Status"), new GUIContent(Regex.Replace(m_currentStatus.ToString(), "(\\B[A-Z])", " $1")));
            if (m_currentStatus == PipelineStatus.Success)
            {
                m_currentStatusMessage = "Exiting in " + ((int)(5 - (DateTime.Now - m_startTime).TotalSeconds)).ToString();
            }
            EditorGUILayout.LabelField(new GUIContent(" "), new GUIContent(m_currentStatusMessage));
        }

#endregion

        #region Pipeline Installation Methods & Data

#if UNITY_2018_1_OR_NEWER

        /// <summary>
        /// Cleans up script components on the scene
        /// </summary>
        /// <param name="renderPipelineSettings"></param>
        private void RemoveObjectComponents()
        {
            switch (m_pipelineToInstall)
            {
                case GaiaConstants.EnvironmentRenderer.BuiltIn:
                    #region Built-In
#if HDPipeline
                    HDAdditionalLightData hDAdditionalLightData = FindObjectOfType<HDAdditionalLightData>();
                    if (hDAdditionalLightData != null)
                    {
                        DestroyImmediate(hDAdditionalLightData);
                    }

                    GameObject underwaterProbe = GameObject.Find("Underwater Reflection Probe");
                    if (underwaterProbe != null)
                    {
                        DestroyImmediate(underwaterProbe);
                    }

                    AdditionalShadowData additionalShadowData = FindObjectOfType<AdditionalShadowData>();
                    if (additionalShadowData != null)
                    {
                        DestroyImmediate(additionalShadowData);
                    }

                    HDAdditionalCameraData hDAdditionalCameraData = FindObjectOfType<HDAdditionalCameraData>();
                    if (hDAdditionalCameraData != null)
                    {
                        DestroyImmediate(hDAdditionalCameraData);
                    }

                    GameObject planarReflectionProbeObject = GameObject.Find("Water Planar Reflections");
                    if (planarReflectionProbeObject != null)
                    {
                        DestroyImmediate(planarReflectionProbeObject);
                    }

                    GameObject densityVolume = GameObject.Find("Density Volume");
                    if (densityVolume != null)
                    {
                        DestroyImmediate(densityVolume);
                    }

                    GameObject hdWindSettings = GameObject.Find("High Definition Wind");
                    if (hdWindSettings != null)
                    {
                        DestroyImmediate(hdWindSettings);
                    }
#endif

#if LWPipeline && UNITY_2018_3_OR_NEWER
                LWRPAdditionalCameraData lWRPAdditionalCameraData = FindObjectOfType<LWRPAdditionalCameraData>();
                if (lWRPAdditionalCameraData != null)
                {
                    DestroyImmediate(lWRPAdditionalCameraData);
                }

                LWRPAdditionalLightData lWRPAdditionalLightData = FindObjectOfType<LWRPAdditionalLightData>();
                if (lWRPAdditionalLightData != null)
                {
                    DestroyImmediate(lWRPAdditionalLightData);
                }
#endif
                    #endregion
                    break;
                case GaiaConstants.EnvironmentRenderer.LightWeight2018x:
                    #region Lightweight
#if HDPipeline
                    HDAdditionalLightData hDAdditionalLightDataLW = FindObjectOfType<HDAdditionalLightData>();
                    if (hDAdditionalLightDataLW != null)
                    {
                        DestroyImmediate(hDAdditionalLightDataLW);
                    }

                    GameObject underwaterProbeLW = GameObject.Find("Underwater Reflection Probe");
                    if (underwaterProbeLW != null)
                    {
                        DestroyImmediate(underwaterProbeLW);
                    }

                    AdditionalShadowData additionalShadowDataLW = FindObjectOfType<AdditionalShadowData>();
                    if (additionalShadowDataLW != null)
                    {
                        DestroyImmediate(additionalShadowDataLW);
                    }

                    HDAdditionalCameraData hDAdditionalCameraDataLW = FindObjectOfType<HDAdditionalCameraData>();
                    if (hDAdditionalCameraDataLW != null)
                    {
                        DestroyImmediate(hDAdditionalCameraDataLW);
                    }

                    GameObject planarReflectionProbeObjectLW = GameObject.Find("Water Planar Reflections");
                    if (planarReflectionProbeObjectLW != null)
                    {
                        DestroyImmediate(planarReflectionProbeObjectLW);
                    }

                    GameObject densityVolumeLW = GameObject.Find("Density Volume");
                    if (densityVolumeLW != null)
                    {
                        DestroyImmediate(densityVolumeLW);
                    }

                    GameObject hdWindSettingsLW = GameObject.Find("High Definition Wind");
                    if (hdWindSettingsLW != null)
                    {
                        DestroyImmediate(hdWindSettingsLW);
                    }
#endif
                    #endregion
                    break;
                case GaiaConstants.EnvironmentRenderer.HighDefinition2018x:
                    #region High Definition
#if LWPipeline && UNITY_2018_3_OR_NEWER
                LWRPAdditionalCameraData lWRPAdditionalCameraDataHD =FindObjectOfType<LWRPAdditionalCameraData>();
                if (lWRPAdditionalCameraDataHD != null)
                {
                    DestroyImmediate(lWRPAdditionalCameraDataHD);
                }
                LWRPAdditionalLightData lWRPAdditionalLightDataHD = FindObjectOfType<LWRPAdditionalLightData>();
                if (lWRPAdditionalLightDataHD != null)
                {
                    DestroyImmediate(lWRPAdditionalLightDataHD);
                }
#endif
                #endregion
                    break;
            }

            if (m_showDebug)
            {
                Debug.Log("Components Removed Successfully");
            }
            m_currentStatus = PipelineStatus.LoadPackageList;
        }

        /// <summary>
        /// Only if pipeline is HD it'll recall the setup for the lighting setup to fix any bugs that wasn't called during the process
        /// </summary>
        private void CleanUpScene()
        {
            if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                //Get main probe and check if HD data is on the probe
                GameObject reflectionProbe = GameObject.Find("Global Reflection Probe");
                if (reflectionProbe != null)
                {
#if HDPipeline
                    if (reflectionProbe.GetComponent<HDAdditionalReflectionData>() == null)
                    {
                        reflectionProbe.AddComponent<HDAdditionalReflectionData>();
                    }
#endif
                }

                if (m_finalizeEnvironment)
                {
                    //Reapply lighting to fix glitches and bugs to ambient lighting
                    GaiaPipelineUtils.SetupHDEnvironmentalVolume(true, false, false, true, false, false, false, m_pipelineToInstall, "High Definition Wind", "High Definition Environment Volume", 1.5f, 2f, "Density Volume", "FFFFFF", 300f, new Vector3(0.1f, 0f, 0f), "HD Volume Profile", "AmbientSkiesSampleDay", "FFF4D3", 110f, 2.05f, 1200f, -25f);
                }            

#if HDPipeline
                //Bake reflection probes
                HDAdditionalReflectionData[] reflectionData = FindObjectsOfType<HDAdditionalReflectionData>();
                foreach (HDAdditionalReflectionData data in reflectionData)
                {
#if !UNITY_2019_1_OR_NEWER
                    data.reflectionProbe.RenderProbe();
#else
                    data.RequestRenderNextUpdate();
#endif
                }
#endif
                }
            else
            {
                //Bake reflection probes
                ReflectionProbe[] probes = FindObjectsOfType<ReflectionProbe>();
                foreach (ReflectionProbe probe in probes)
                {
                    probe.RenderProbe();
                }
            }

            GameObject sunLight = GaiaPipelineUtils.GetMainDirectionalLight();
            if (sunLight != null)
            {
                Component[] components = sunLight.GetComponents<Component>();
                foreach(Component component in components)
                {
                    if (component == null)
                    {
                        DestroyImmediate(component);
                    }
                }
            }

            GameObject camera = GaiaPipelineUtils.GetOrCreateMainCamera();
            if (camera != null)
            {
                Component[] components = camera.GetComponents<Component>();
                foreach (Component component in components)
                {
                    if (component == null)
                    {
                        DestroyImmediate(component);
                    }
                }
            }

            GameObject GlobalProbe = GameObject.Find("Global Reflection Probe");
            if (GlobalProbe != null)
            {
                Component[] components = GlobalProbe.GetComponents<Component>();
                foreach (Component component in components)
                {
                    if (component == null)
                    {
                        DestroyImmediate(component);
                    }
                }
            }

            m_currentStatus = PipelineStatus.Success;
            m_currentStatusMessage = "";
        }

        /// <summary>
        /// List packages via package manager and starts update
        /// </summary>
        private void LoadPackageList()
        {
            string[] packages = new string[0];

#if UNITY_2018_1
            packages = Directory.GetFiles(Application.dataPath, "PackageImportList2018_1.txt", SearchOption.AllDirectories);
#elif UNITY_2018_2
            packages = Directory.GetFiles(Application.dataPath, "PackageImportList2018_2.txt", SearchOption.AllDirectories);
#elif UNITY_2018_3
            if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2018_3_Builtin.txt", SearchOption.AllDirectories);
            }
            else if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2018_3_LW.txt", SearchOption.AllDirectories);
            }
            else if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2018_3_HD.txt", SearchOption.AllDirectories);
            }
#elif UNITY_2018_4
            if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2018_4_Builtin.txt", SearchOption.AllDirectories);
            }
            else if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2018_4_LW.txt", SearchOption.AllDirectories);
            }
            else if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2018_4_HD.txt", SearchOption.AllDirectories);
            }
#elif UNITY_2019_1_OR_NEWER
            if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2019_1_Builtin.txt", SearchOption.AllDirectories);
            }
            else if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2019_1_LW.txt", SearchOption.AllDirectories);
            }
            else if (m_pipelineToInstall == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                packages = Directory.GetFiles(Application.dataPath, "PackageImportList2019_1_HD.txt", SearchOption.AllDirectories);
            }
#endif

            if (packages.Length == 0)
            {
                Debug.LogError("[Pipeline Utils] Couldn't find packages list.");
                m_currentStatusMessage = "Couldn't find packages list.";
                m_currentStatus = PipelineStatus.FailedWithErrors;
                return;
            }

            string packageListPath = packages[0];
            m_packagesToAdd = new List<PackageEntry>();
            string[] content = File.ReadAllLines(packageListPath);
            foreach (var line in content)
            {
                var split = line.Split('@');
                PackageEntry entry = new PackageEntry();

                entry.name = split[0];
                entry.version = split.Length > 1 ? split[1] : null;

                m_packagesToAdd.Add(entry);
            }
            m_currentStatus = PipelineStatus.ProcessInstalledPackageList;
            m_pmListRequest = Client.List();
        }

        /// <summary>
        /// Process the installed package list
        /// </summary>
        private void ProcessInstalledPackageList()
        {
            if (m_pmListRequest != null)
            {
                if (m_pmListRequest.IsCompleted)
                {
                    bool[] foundPackages = new bool[m_packagesToAdd.Count];
                    for (int i = 0; i < foundPackages.Length; ++i)
                    {
                        foundPackages[i] = false;
                    }
                    foreach (var package in m_pmListRequest.Result)
                    {
                        for (int i = 0; i < foundPackages.Length; ++i)
                        {
                            if (package.packageId.Contains(m_packagesToAdd[i].name))
                            {
                                foundPackages[i] = true;
                                if (m_showDebug)
                                {
                                    Debug.Log("[Pipeline Utils] " + m_packagesToAdd[i].name + " already imported.");
                                }
                            }
                        }
                    }
                    for (int i = 0; i < foundPackages.Length; ++i)
                    {
                        if (!foundPackages[i])
                        {
                            m_missingPackages.Push(i);
                        }
                    }
                    m_pmListRequest = null;
                    m_currentStatus = PipelineStatus.InstallingPackages;
                }
                else if (m_pmListRequest.Error != null)
                {
                    Debug.LogError("[Pipeline Utils] Error : " + m_pmListRequest.Error.message);
                    m_currentStatus = PipelineStatus.FailedWithErrors;
                    m_currentStatusMessage = m_pmListRequest.Error.message;
                    m_pmListRequest = null;
                }
            }
        }

        /// <summary>
        /// Install packages
        /// </summary>
        private void InstallPackages()
        {
            if (!m_isAddingPackage)
            {
                if (m_missingPackages.Count == 0)
                {
                    m_currentStatus = PipelineStatus.InstallingWaterShaders;
                    m_currentStatusMessage = "";
                }
                else
                {
                    int package = m_missingPackages.Pop();
                    string name = m_packagesToAdd[package].name;
                    if (m_packagesToAdd[package].version != null)
                    {
                        name += "@" + m_packagesToAdd[package].version;
                    }

                    if (m_showDebug)
                    {
                        Debug.Log("[Pipeline Utils] - Adding package " + name);
                    }
                    m_isAddingPackage = true;
                    m_currentStatusMessage = name;
                    m_pmAddRequest = Client.Add(name);
                    Repaint();
                }
            }
            else
            {
                if (m_pmAddRequest.Status == StatusCode.Failure)
                {
                    m_currentStatus = PipelineStatus.FailedWithErrors;
                    if (m_pmAddRequest.Error != null)
                    {
                        if (!string.IsNullOrEmpty(m_pmAddRequest.Error.message))
                        {
                            Debug.LogError("[Pipeline Utils] - Add Package Error : " + m_pmAddRequest.Error.message);
                            m_currentStatusMessage = m_pmAddRequest.Error.message;
                        }
                        else
                        {
                            Debug.LogError("[Pipeline Utils] - Add Package Failure");
                            m_currentStatusMessage = "Unknown failure";
                        }
                    }
                    else
                    {
                        Debug.LogError("[Pipeline Utils] - Add Package Failure");
                        m_currentStatusMessage = "Unknown failure";
                    }
                    m_isAddingPackage = false;
                    m_pmAddRequest = null;
                }
                else if (m_pmAddRequest.Status == StatusCode.Success)
                {
                    if (m_showDebug && m_pmAddRequest.Result != null)
                    {
                        Debug.Log("[Pipeline Utils] - Added package " + m_pmAddRequest.Result.displayName);
                    }
                    m_currentStatusMessage = "";
                    m_isAddingPackage = false;
                    m_pmAddRequest = null;
                }
            }
        }

        /// <summary>
        /// Install / deinstall LW and HD water shaders
        /// </summary>
        private void InstallWaterShaders()
        {
            string waterLW = GetAssetPath("Simple Water Sample LW");
            string waterHD = GetAssetPath("Simple Water Sample HD");
            string water2019HD = GetAssetPath("2019 Water Sample HD");

            switch (m_pipelineToInstall)
            {
                case GaiaConstants.EnvironmentRenderer.BuiltIn:
                {
                        if (!string.IsNullOrEmpty(waterLW))
                        {
                            if (waterLW.Contains(".shader"))
                            {
                                FileUtil.MoveFileOrDirectory(waterLW, waterLW.Replace(".shader", ".txt"));
                            }
                        }
                        if (!string.IsNullOrEmpty(waterHD))
                        {
                            if (waterHD.Contains(".shader"))
                            {
                                FileUtil.MoveFileOrDirectory(waterHD, waterHD.Replace(".shader", ".txt"));
                            }
                        }
                        if (!string.IsNullOrEmpty(water2019HD))
                        {
                            if (water2019HD.Contains(".shader"))
                            {
                                FileUtil.MoveFileOrDirectory(water2019HD, water2019HD.Replace(".shader", ".txt"));
                            }
                        }
                        break;
                }
                case GaiaConstants.EnvironmentRenderer.LightWeight2018x:
                {
                        if (!string.IsNullOrEmpty(waterLW))
                        {
                            if (waterLW.Contains(".txt"))
                            {
                                FileUtil.MoveFileOrDirectory(waterLW, waterLW.Replace(".txt", ".shader"));
                            }
                        }
                        if (!string.IsNullOrEmpty(waterHD))
                        {
                            if (waterHD.Contains(".shader"))
                            {
                                FileUtil.MoveFileOrDirectory(waterHD, waterHD.Replace(".shader", ".txt"));
                            }
                        }
                        if (!string.IsNullOrEmpty(water2019HD))
                        {
                            if (water2019HD.Contains(".shader"))
                            {
                                FileUtil.MoveFileOrDirectory(water2019HD, water2019HD.Replace(".shader", ".txt"));
                            }
                        }
                        break;
                }
                case GaiaConstants.EnvironmentRenderer.HighDefinition2018x:
                {
                        if (!string.IsNullOrEmpty(waterLW))
                        {
                            if (waterLW.Contains(".shader"))
                            {
                                FileUtil.MoveFileOrDirectory(waterLW, waterLW.Replace(".shader", ".txt"));
                            }
                        }
#if !UNITY_2019_1_OR_NEWER
                        if (!string.IsNullOrEmpty(waterHD))
                        {
                            if (waterHD.Contains(".txt"))
                            {
                                FileUtil.MoveFileOrDirectory(waterHD, waterHD.Replace(".txt", ".shader"));
                            }
                        }
#else
                        if (!string.IsNullOrEmpty(water2019HD))
                        {
                            if (water2019HD.Contains(".txt"))
                            {
                                FileUtil.MoveFileOrDirectory(water2019HD, water2019HD.Replace(".txt", ".shader"));
                            }
                        }
#endif
                        break;
                }
            }

            m_currentStatus = PipelineStatus.UpdatingMaterials;
            m_currentStatusMessage = "";
        }

        /// <summary>
        /// Update materials
        /// </summary>
        private void UpdateMaterials()
        {
            if (!m_updateMaterials)
            {
                m_currentStatus = PipelineStatus.DeletingPackages;
                m_currentStatusMessage = "";
                return;
            }

            //First check if all the required shaders can be found in the project
            Shader shaderBuiltInStandard = Shader.Find("Standard");
            Shader shaderBuiltInStandardSpecular = Shader.Find("Standard (Specular setup)");

            Shader shaderLWRPLit = Shader.Find("Lightweight Render Pipeline/Lit");
            Shader shaderLWRPSimpleLit = Shader.Find("Lightweight Render Pipeline/Simple Lit");

            Shader shaderHDRPLit = Shader.Find("HDRP/Lit");
            Shader shaderHDRPLitTesselation = Shader.Find("HDRP/LitTessellation");

            bool builtInShadersPresent = false;
            bool LWRPShadersPresent = false;
            bool HDRPShadersPresent = false;

            if (shaderBuiltInStandard != null && shaderBuiltInStandardSpecular != null)
            {
                builtInShadersPresent = true;
            }

            if (shaderLWRPLit != null && shaderLWRPSimpleLit != null)
            {
                LWRPShadersPresent = true;
            }

            if (shaderHDRPLit != null && shaderHDRPLitTesselation != null)
            {
                HDRPShadersPresent = true;
            }

            var allMaterialGUIDS = AssetDatabase.FindAssets("t:Material");

            switch (m_pipelineToInstall)
            {
                case GaiaConstants.EnvironmentRenderer.BuiltIn:
                    if (builtInShadersPresent)
                    {
                        foreach (string materialGuid in allMaterialGUIDS)
                        {
                            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(materialGuid));
                            if (LWRPShadersPresent)
                            {
                                if (material.shader == shaderLWRPLit) material.shader = shaderBuiltInStandard;
                                if (material.shader == shaderLWRPSimpleLit) material.shader = shaderBuiltInStandardSpecular;
                            }
                            if (HDRPShadersPresent)
                            {
                                if (material.shader == shaderHDRPLit) material.shader = shaderBuiltInStandard;
                                if (material.shader == shaderHDRPLitTesselation) material.shader = shaderBuiltInStandardSpecular;
                            }
                        }
                    }
                    else
                    {
                        m_currentStatus = PipelineStatus.FailedWithErrors;
                        m_currentStatusMessage = "Missing builtin shaders";
                        Debug.LogError("Trying to switch to built-in rendering, but the built in shaders are not present in this project! Shaders can't be updated.");
                        return;
                    }
                    break;
                case GaiaConstants.EnvironmentRenderer.LightWeight2018x:
                    if (LWRPShadersPresent)
                    {
                        foreach (string materialGuid in allMaterialGUIDS)
                        {
                            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(materialGuid));
                            if (builtInShadersPresent)
                            {
                                if (material.shader == shaderBuiltInStandard) material.shader = shaderLWRPLit;
                                if (material.shader == shaderBuiltInStandardSpecular) material.shader = shaderLWRPSimpleLit;
                            }
                            if (HDRPShadersPresent)
                            {
                                if (material.shader == shaderHDRPLit) material.shader = shaderLWRPLit;
                                if (material.shader == shaderHDRPLitTesselation) material.shader = shaderLWRPSimpleLit;
                            }
                        }
                    }
                    else
                    {
                        m_currentStatus = PipelineStatus.FailedWithErrors;
                        m_currentStatusMessage = "Missing LWRP shaders";
                        Debug.LogError("Trying to switch to LWRP rendering, but the LWRP standard shaders are not present in this project! Shaders can't be updated.");
                    }
                    break;
                case GaiaConstants.EnvironmentRenderer.HighDefinition2018x:
                    if (HDRPShadersPresent)
                    {
                        foreach (string materialGuid in allMaterialGUIDS)
                        {
                            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(materialGuid));
                            if (builtInShadersPresent)
                            {
                                if (material.shader == shaderBuiltInStandard) material.shader = shaderHDRPLit;
                                if (material.shader == shaderBuiltInStandardSpecular) material.shader = shaderHDRPLit;
                            }
                            if (LWRPShadersPresent)
                            {
                                if (material.shader == shaderLWRPLit) material.shader = shaderHDRPLit;
                                if (material.shader == shaderLWRPSimpleLit) material.shader = shaderHDRPLit;
                            }
                        }
                    }
                    else
                    {
                        m_currentStatus = PipelineStatus.FailedWithErrors;
                        m_currentStatusMessage = "Missing HDRP shaders";
                        Debug.LogError("Trying to switch to HDRP rendering, but the HDRP standard shaders are not present in this project! Shaders can't be updated.");
                    }
                    break;
            }

            AssetDatabase.Refresh();
            m_currentStatus = PipelineStatus.DeletingPackages;
            m_currentStatusMessage = "";
        }

        /// <summary>
        /// Determine what to delete and then delete it
        /// </summary>
        private void DeletePackages()
        {
            if (!m_isRemovingPackage)
            {
                bool removeLW = false;
                bool removeHD = false;
                switch (m_pipelineToInstall)
                {                   
                    case GaiaConstants.EnvironmentRenderer.BuiltIn:
                        removeLW = true;
                        removeHD = true;
                        break;
                    case GaiaConstants.EnvironmentRenderer.LightWeight2018x:
                        removeHD = true;
                        break;
                    case GaiaConstants.EnvironmentRenderer.HighDefinition2018x:
                        removeLW = true;
                        break;
                }

                //Check for & remove LWRP
                if (removeLW && Shader.Find("Lightweight Render Pipeline/Lit"))
                {
                    if (m_showDebug)
                    {
                        Debug.Log("[Pipeline Utils] - Removing com.unity.render-pipelines.lightweight");
                    }
                    m_isRemovingPackage = true;
                    m_currentStatusMessage = "com.unity.render-pipelines.lightweight";
                    m_pmRemoveRequest = Client.Remove("com.unity.render-pipelines.lightweight");
                    Repaint();
                    return;
                }

                //Check for remove HDRP
                if (removeHD && Shader.Find("HDRP/Lit"))
                {
                    if (m_showDebug)
                    {
                        Debug.Log("[Pipeline Utils] - Removing com.unity.render-pipelines.high-definition");
                    }
                    m_isRemovingPackage = true;
                    m_currentStatusMessage = "com.unity.render-pipelines.high-definition";
                    m_pmRemoveRequest = Client.Remove("com.unity.render-pipelines.high-definition");
                    Repaint();
                    return;
                }

                //Outa here
                m_isRemovingPackage = false;
                m_currentStatus = PipelineStatus.UpdatingBuildSettings;
                m_currentStatusMessage = "";
            }
            else
            {
                if (m_pmRemoveRequest.Status == StatusCode.Failure)
                {
                    m_currentStatus = PipelineStatus.FailedWithErrors;
                    if (m_pmRemoveRequest.Error != null)
                    {
                        Debug.LogError("[Pipeline Utils] - Remove Package Error : " + m_pmRemoveRequest.Error.message);
                        m_currentStatusMessage = m_pmRemoveRequest.Error.message;
                    }
                    else
                    {
                        Debug.LogError("[Pipeline Utils] - Remove Package Failure");
                        m_currentStatusMessage = "Unknown failure";
                    }
                    m_isRemovingPackage = false;
                    m_pmRemoveRequest = null;
                }
                else if (m_pmRemoveRequest.Status == StatusCode.Success)
                {
                    if (m_showDebug && !string.IsNullOrEmpty(m_pmRemoveRequest.PackageIdOrName))
                    {
                        Debug.Log("[Pipeline Utils] - Removed package " + m_pmRemoveRequest.PackageIdOrName);
                    }
                    m_currentStatusMessage = "";
                    m_isRemovingPackage = false;
                    m_pmRemoveRequest = null;
                }
            }
        }

        /// <summary>
        /// Update the build settings
        /// </summary>
        private void UpdateBuildSettings()
        {
            bool isChanged = false;
            switch (m_pipelineToInstall)
            {
                case GaiaConstants.EnvironmentRenderer.BuiltIn:
                    {
                        string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                        if (currBuildSettings.Contains("LWPipeline"))
                        {
                            currBuildSettings = currBuildSettings.Replace("LWPipeline;", "");
                            currBuildSettings = currBuildSettings.Replace("LWPipeline", "");
                            isChanged = true;
                        }
                        if (currBuildSettings.Contains("HDPipeline"))
                        {
                            currBuildSettings = currBuildSettings.Replace("HDPipeline;", "");
                            currBuildSettings = currBuildSettings.Replace("HDPipeline", "");
                            isChanged = true;
                        }
                        if (isChanged)
                        {
                            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings);
                            return;
                        }
                        break;
                    }
                case GaiaConstants.EnvironmentRenderer.LightWeight2018x:
                    {
                        string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                        if (!currBuildSettings.Contains("LWPipeline"))
                        {
                            if (string.IsNullOrEmpty(currBuildSettings))
                            {
                                currBuildSettings = "LWPipeline";
                            }
                            else
                            {
                                currBuildSettings += ";LWPipeline";
                            }
                            isChanged = true;
                        }
                        if (currBuildSettings.Contains("HDPipeline"))
                        {
                            currBuildSettings = currBuildSettings.Replace("HDPipeline;", "");
                            currBuildSettings = currBuildSettings.Replace("HDPipeline", "");
                            isChanged = true;
                        }
                        if (isChanged)
                        {
                            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings);
                            return;
                        }
                        break;
                    }
                case GaiaConstants.EnvironmentRenderer.HighDefinition2018x:
                    {
                        string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                        if (!currBuildSettings.Contains("HDPipeline"))
                        {
                            if (string.IsNullOrEmpty(currBuildSettings))
                            {
                                currBuildSettings = "HDPipeline";
                            }
                            else
                            {
                                currBuildSettings += ";HDPipeline";
                            }
                            isChanged = true;
                        }
                        if (currBuildSettings.Contains("LWPipeline"))
                        {
                            currBuildSettings = currBuildSettings.Replace("LWPipeline;", "");
                            currBuildSettings = currBuildSettings.Replace("LWPipeline", "");
                            isChanged = true;
                        }
                        if (isChanged)
                        {
                            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings);
                            return;
                        }
                        break;
                    }
            }

            m_currentStatus = PipelineStatus.FinalizingEnvironment;
            m_currentStatusMessage = "";
        }

        /// <summary>
        /// Now perform final version specific environmental setup
        /// </summary>
        private void FinalizeEnvironment(bool finalise)
        {
            switch (m_pipelineToInstall)
            {
                case GaiaConstants.EnvironmentRenderer.BuiltIn:
                    if (finalise)
                    {
                        SetPostProcessingStyle("Ambient Sample Default Day Post Processing");
                        GaiaPipelineUtils.SetupPipeline(m_pipelineToInstall, null, null, null, "Procedural Worlds/Simple Water", true);
                    }

                    break;
                case GaiaConstants.EnvironmentRenderer.LightWeight2018x:
                    if (finalise)
                    {
                        SetPostProcessingStyle("Ambient Sample Default Day Post Processing");
                        GaiaPipelineUtils.SetupPipeline(m_pipelineToInstall, "Procedural Worlds Lightweight Pipeline Profile", "Pipeline Terrain Material", "Lightweight Render Pipeline/Terrain/Lit", "Procedural Worlds/Simple Water LW", true);
                    }
                    
                    break;
                case GaiaConstants.EnvironmentRenderer.HighDefinition2018x:
                    if (finalise)
                    {
                        SetPostProcessingStyle("Ambient Sample Default Day Post Processing");
#if !UNITY_2019_1_OR_NEWER
                        GaiaPipelineUtils.SetupPipeline(m_pipelineToInstall, "Procedural Worlds HDRenderPipelineAsset", "Pipeline Terrain Material", "HDRP/TerrainLit", "Procedural Worlds/Simple Water HD", true);
#else
                        GaiaPipelineUtils.SetupPipeline(m_pipelineToInstall, "Procedural Worlds HDRenderPipelineAsset", "Pipeline Terrain Material", "HDRP/TerrainLit", "Procedural Worlds/Simple Water 2019 HD", true);
#endif
                    }

                    break;
            }

            m_currentStatus = PipelineStatus.CleanUp;
        }
#endif

        #endregion

        #region Generic Utils

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns>The path or null</returns>
        private string GetAssetPath(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            return null;
        }

        /// <summary>
        /// Set the post processing in the scene
        /// </summary>
        public static void SetPostProcessingStyle(string ppName)
        {
            //Hack to not set PP on ultralight and mobile - because it is too expensive
            GaiaSettings settings = GaiaUtils.GetGaiaSettings();
            if (settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.UltraLight ||
                settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.MobileAndVR)
            {
                return;
            }

#if UNITY_5_6_0
                return;
#endif

#if UNITY_POST_PROCESSING_STACK_V2
            GameObject theParentObject = GaiaPipelineUtils.GetOrCreateParentObject("Gaia Environment");
            GameObject postProcessingVolumeObject = GameObject.Find("Global Post Processing");
            GameObject mainCameraObject = GaiaPipelineUtils.GetOrCreateMainCamera();

            //If the post processing volume is null it creates one
            if (postProcessingVolumeObject == null)
            {
                postProcessingVolumeObject = new GameObject("Global Post Processing");
                postProcessingVolumeObject.transform.parent = theParentObject.transform;
                postProcessingVolumeObject.layer = LayerMask.NameToLayer("TransparentFX");

                var ppVol = postProcessingVolumeObject.AddComponent<PostProcessVolume>();
                ppVol.isGlobal = true;
                ppVol.priority = 0f;
                ppVol.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GaiaPipelineUtils.GetAssetPath(ppName));
                ppVol.weight = 1f;
                ppVol.blendDistance = 0f;
            }
            else
            {
                var ppVol = postProcessingVolumeObject.GetComponent<PostProcessVolume>();
                ppVol.isGlobal = true;
                ppVol.priority = 0f;
                ppVol.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GaiaPipelineUtils.GetAssetPath(ppName));
                ppVol.weight = 1f;
                ppVol.blendDistance = 0f;
            }

            PostProcessProfile profile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GaiaPipelineUtils.GetAssetPath(ppName));
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

                if (settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                {
                    UnityEngine.Rendering.PostProcessing.MotionBlur motionBlur;
                    if (profile.TryGetSettings(out motionBlur))
                    {
                        motionBlur.active = false;
                    }
                }
                else
                {
                    UnityEngine.Rendering.PostProcessing.MotionBlur motionBlur;
                    if (profile.TryGetSettings(out motionBlur))
                    {
                        motionBlur.active = true;
                    }
                }
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

        #endregion
    }
}