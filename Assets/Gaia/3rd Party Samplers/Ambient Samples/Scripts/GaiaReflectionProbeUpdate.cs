using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace Gaia
{
    /// <summary>
    /// This class should be attached to an object that has a reflection probe.
    ///
    /// It will use distance and/or time to trigger reflection probe updates.
    ///
    /// It is assumed that probe updates will happen over multiple frames, so the variable
    /// m_isUpdatingProbe should be set at the beginning of an update, and then reset at
    /// the end of an update. No further updates will be scheduled while this
    /// variable is set.
    /// 
    /// </summary>

    [RequireComponent(typeof(ReflectionProbe))]
    public class GaiaReflectionProbeUpdate : MonoBehaviour
    {
        /// <summary>
        /// Reflection probe quality
        /// </summary>
        public enum ReflectionQuality { VeryLow, Low, Medium, High, VeryHigh, Ultra }

        /// <summary>
        /// Reflection probe far render distance
        /// </summary>
        public enum RenderDistanceQuality { Close, Near, Far, VeryFar, ExtremelyFar }

        #region Public Variables
        [Header("Probe Configuration [Applied OnStart]")]
        [Tooltip("Sets the probe reflection quality")]
        public ReflectionQuality m_probeResolution = ReflectionQuality.Medium;

        [Tooltip("Sets the probe render distance")]
        public RenderDistanceQuality m_probeRenderDistance = RenderDistanceQuality.Near;

        [Tooltip("If on the reflection probe will follow the camera position")]
        public bool m_followCamera = false;

        [Tooltip("Offset above the camera that probe will be adjusted by when following camera. Zero gives more accurate reflections, but captures surrounding trees and other objects.")]
        public float m_followHeightOffset = 30f;

        [Tooltip("Sets box projection")]
        public bool m_boxProjection = false;

        [Tooltip("Sets hdr on the reflection probe")]
        public bool m_useHDR = true;

        #endregion

        #region Private Worker Variables
        private int m_renderID;
        private ReflectionProbe m_reflectionProbe;
        private bool m_cameraIsMoving = false;
        private Vector3 m_lastLocation;

        //[HideInInspector]
        public GameObject m_mainCameraObject;
        #endregion

        #region Start Function
        void Start()
        {
            m_mainCameraObject = GetOrCreateMainCamera();
            m_reflectionProbe = gameObject.GetComponent<ReflectionProbe>();
            SetProbeSettings();
        }
        #endregion

        #region Update Function

        /// <summary>
        /// Process update at end of other updates
        /// </summary>
        void LateUpdate()
        {
            //Work out if our player is moving
            if (m_mainCameraObject != null)
            {
                if (m_lastLocation != m_mainCameraObject.transform.position)
                {
                    m_lastLocation = m_mainCameraObject.transform.position;
                    m_cameraIsMoving = true;
                }
                else
                {
                    m_cameraIsMoving = false;
                }
            }

            //Process accordingly
            if (!m_cameraIsMoving)
            {
                if (m_reflectionProbe.mode == ReflectionProbeMode.Realtime)
                {
                    m_reflectionProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
                }
            }
            else
            {
                //Move the probe
                if (m_followCamera)
                {
                    ProbeFollow();
                }

                //Set new mode
                if (m_reflectionProbe.mode == ReflectionProbeMode.Realtime)
                {
                    m_reflectionProbe.refreshMode = ReflectionProbeRefreshMode.EveryFrame;
                }
            }
        }

        #endregion

        #region Probe Settings Setup
        /// <summary>
        /// Sets the probe settings
        /// </summary>
        public void SetProbeSettings()
        {
            ReflectionProbe reflectionProbe = gameObject.GetComponent<ReflectionProbe>();
            reflectionProbe.timeSlicingMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
            reflectionProbe.boxProjection = m_boxProjection;
            reflectionProbe.hdr = m_useHDR;
            reflectionProbe.intensity = 1f;
            reflectionProbe.importance = 1;
            reflectionProbe.clearFlags = ReflectionProbeClearFlags.Skybox;
            reflectionProbe.center = new Vector3(0f, 0f, 0f);
            if (GameObject.Find("Ambient Water Sample"))
            {
                reflectionProbe.size = new Vector3(125f, 10000f, 125f);
                reflectionProbe.mode = ReflectionProbeMode.Realtime;
                reflectionProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            }

            switch (m_probeResolution)
            {
                case ReflectionQuality.VeryLow:
                    reflectionProbe.resolution = 32;
                    reflectionProbe.shadowDistance = 5f;
                    break;
                case ReflectionQuality.Low:
                    reflectionProbe.resolution = 64;
                    reflectionProbe.shadowDistance = 10f;
                    break;
                case ReflectionQuality.Medium:
                    reflectionProbe.resolution = 128;
                    reflectionProbe.shadowDistance = 25f;
                    break;
                case ReflectionQuality.High:
                    reflectionProbe.resolution = 256;
                    reflectionProbe.shadowDistance = 75f;
                    break;
                case ReflectionQuality.VeryHigh:
                    reflectionProbe.resolution = 512;
                    reflectionProbe.shadowDistance = 100f;
                    break;
                case ReflectionQuality.Ultra:
                    reflectionProbe.resolution = 1024;
                    reflectionProbe.shadowDistance = 150f;
                    break;
            }

            switch (m_probeRenderDistance)
            {
                case RenderDistanceQuality.Close:
                    reflectionProbe.farClipPlane = 250f;
                    break;
                case RenderDistanceQuality.Near:
                    reflectionProbe.farClipPlane = 500f;
                    break;
                case RenderDistanceQuality.Far:
                    reflectionProbe.farClipPlane = 1000f;
                    break;
                case RenderDistanceQuality.VeryFar:
                    reflectionProbe.farClipPlane = 2000f;
                    break;
                case RenderDistanceQuality.ExtremelyFar:
                    reflectionProbe.farClipPlane = 4000f;
                    break;
            }

            if (reflectionProbe.IsFinishedRendering(m_renderID))
            {
                m_renderID = reflectionProbe.RenderProbe();
            }
        }
        #endregion

        #region Probe Follows Camera
        /// <summary>
        /// Probe follows the camera around
        /// </summary>
        public void ProbeFollow()
        {
            if (m_mainCameraObject != null)
            {
                Vector3 cameraLocation = m_mainCameraObject.transform.position;
                cameraLocation.y += m_followHeightOffset;
                m_reflectionProbe.gameObject.transform.localPosition = cameraLocation;
            }
        }
        #endregion

        #region Utils

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

        #endregion
    }
}