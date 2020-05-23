using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// This object stores Gaia settings. It remembers what you have been working on, and resets these when you start up the Gaia Manager window.
    /// </summary>
    public class GaiaSettings : ScriptableObject
    {
        [Header("Current Settings")]
        public GaiaConstants.EnvironmentControllerType m_currentController = GaiaConstants.EnvironmentControllerType.FirstPerson;
        [Tooltip("Target size.")]
        public GaiaConstants.EnvironmentSize m_currentSize = GaiaConstants.EnvironmentSize.Is2048MetersSq;
        [Tooltip("Current target environment.")]
        public GaiaConstants.EnvironmentTarget m_currentEnvironment = GaiaConstants.EnvironmentTarget.Desktop;
        [Tooltip("Current target renderer.")]
        public GaiaConstants.EnvironmentRenderer m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
        [Tooltip("Current defaults object.")]
        public GaiaDefaults m_currentDefaults;
        [Tooltip("Current terrain resources object.")]
        public GaiaResource m_currentResources;
        [Tooltip("Current game object resources object.")]
        public GaiaResource m_currentGameObjectResources;
        [Tooltip("Current size divisor.")]
        public float m_currentSizeDivisor = 1f;
        [Tooltip("Current prefab name for the player object.")]
        public string m_currentPlayerPrefabName = "FPSController";
        [Tooltip("Current prefab name for the water object.")]
        public string m_currentWaterPrefabName = "Water4Advanced";
        [Tooltip("Current path for terrain layer storage. Must include 'Assets\' at the beginning.")]
        public string m_currentTerrainLayerStoragePath = "Assets\\GaiaTerrainLayers\\";

        [Tooltip("Publisher name for exported extensions.")]
        public string m_publisherName = "";
        [Tooltip("Default prefab name for the first person player object.")]
        public string m_fpsPlayerPrefabName = "FPSController";
        [Tooltip("Default prefab name for the third person player object.")]
        public string m_3pPlayerPrefabName = "ThirdPersonController";
        [Tooltip("Default prefab name for the roller ball player object.")]
        public string m_rbPlayerPrefabName = "RollerBall";
        [Tooltip("Default prefab name for the light weight water object.")]
        public string m_waterMobilePrefabName = "WaterBasicDaytime";
        [Tooltip("Default prefab name for the water object.")]
        public string m_waterPrefabName = "Water4Advanced";
        [Tooltip("Show or hide tooltips in all custom editors.")]
        public bool m_showTooltips = true;

        [Header("Alternative Configurations")]
        [Tooltip("Ultra light defaults object.")]
        public GaiaDefaults m_ultraLightDefaults;
        [Tooltip("Ultra light resources object.")]
        public GaiaResource m_ultraLightResources;
        [Tooltip("Ultra light gameobject resources object.")]
        public GaiaResource m_ultraLightGameObjectResources;
        [Tooltip("Mobile defaults object.")]
        public GaiaDefaults m_mobileDefaults;
        [Tooltip("Mobile resources object.")]
        public GaiaResource m_mobileResources;
        [Tooltip("Mobile game object resources object.")]
        public GaiaResource m_mobileGameObjectResources;
        [Tooltip("Desktop defaults object.")]
        public GaiaDefaults m_desktopDefaults;
        [Tooltip("Desktop resources object.")]
        public GaiaResource m_desktopResources;
        [Tooltip("Desktop game object resources object.")]
        public GaiaResource m_desktopGameObjectResources;
        [Tooltip("Powerful desktop defaults object.")]
        public GaiaDefaults m_powerDesktopDefaults;
        [Tooltip("Powerful desktop resources object.")]
        public GaiaResource m_powerDesktopResources;
        [Tooltip("Powerful desktop gamem objcet resources object.")]
        public GaiaResource m_powerDesktopGameObjectResources;

        [Header("News")]
        public long m_lastWebUpdate = 0;
        public bool m_hideHeroMessage = false;
        public string m_latestNewsTitle = "Latest News";
        public string m_latestNewsBody = "Here is the news";
        public string m_latestNewsUrl = "http://www.procedural-worlds.com/blog/";
        public Texture2D m_latestNewsImage;
    }
}