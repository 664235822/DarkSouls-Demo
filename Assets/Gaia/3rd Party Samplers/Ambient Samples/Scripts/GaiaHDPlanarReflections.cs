#if HDPipeline
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace Gaia
{
    public class GaiaHDPlanarReflections : MonoBehaviour
    {
        #region Variables
        private PlanarReflectionProbe m_planarReflectionProbe;
        private Transform m_player;

        private GaiaSettings m_gaiaSettings;
        #endregion

        #region Setup
        /// <summary>
        /// Start setup
        /// </summary>
        private void Start()
        {
            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }
            if (m_planarReflectionProbe == null)
            {
                m_planarReflectionProbe = FindObjectOfType<PlanarReflectionProbe>();
            }
            if (m_player == null)
            {
                m_player = GetThePlayer();
            }

            if (m_planarReflectionProbe != null)
            {
                OptimizePlanarReflections(m_planarReflectionProbe);
                #if UNITY_2019_1_OR_NEWER
                m_planarReflectionProbe.RequestRenderNextUpdate();
                #else
                m_planarReflectionProbe.RequestRealtimeRender();
#endif
            }
        }
#endregion

        #region Helper Methods
        /// <summary>
        /// Gets player and returns if found
        /// </summary>
        /// <returns>Player</returns>
        public Transform GetThePlayer()
        {
#if UNITY_EDITOR
            Transform thePlayer = null;
            if (m_gaiaSettings.m_currentController == GaiaConstants.EnvironmentControllerType.FirstPerson)
            {
                if (GameObject.Find("FirstPersonCharacter") != null)
                {
                    thePlayer = GameObject.Find("FirstPersonCharacter").transform;

                    if (thePlayer != null)
                    {
                        return thePlayer;
                    }
                }
            }

            if (m_gaiaSettings.m_currentController == GaiaConstants.EnvironmentControllerType.FlyingCamera)
            {
                if (GameObject.Find("FlyCam") != null)
                {
                    thePlayer = GameObject.Find("FlyCam").transform;

                    if (thePlayer != null)
                    {
                        return thePlayer;
                    }
                }
            }

            if (m_gaiaSettings.m_currentController == GaiaConstants.EnvironmentControllerType.ThirdPerson)
            {
                if (GameObject.Find("Main Camera") != null)
                {
                    thePlayer = GameObject.Find("Main Camera").transform;

                    if (thePlayer != null)
                    {
                        return thePlayer;
                    }
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// Optimizes the planar reflections
        /// </summary>
        /// <param name="planarReflectionProbe"></param>
        private void OptimizePlanarReflections(PlanarReflectionProbe planarReflectionProbe)
        {
            if (planarReflectionProbe != null)
            {
                PlanarReflectionProbe planar = planarReflectionProbe;
                if (planar != null)
                {
#if !UNITY_2019_1_OR_NEWER
                    planar.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
                    planar.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
                    planar.captureSettings.farClipPlane = 25f;
                    planar.captureSettings.overrides = CaptureSettingsOverrides.FieldOfview;
                    planar.captureSettings.fieldOfView = 90f;
                    planar.RequestRealtimeRender();
#else
                    planar.mode = ProbeSettings.Mode.Realtime;
#endif
                }

                HDAdditionalReflectionData data = planar.gameObject.GetComponent<HDAdditionalReflectionData>();
                if (data != null)
                {
#if !UNITY_2019_1_OR_NEWER
                    data.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
                    data.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
                    data.captureSettings.farClipPlane = 250f;
                    data.captureSettings.fieldOfView = 90f;
#else
                    data.mode = ProbeSettings.Mode.Realtime;
#endif
                }
            }
        }
#endregion
    }
}
#endif