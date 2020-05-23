using UnityEngine;
using System.Collections;

namespace Gaia
{
    public class FrameRateManager : MonoBehaviour
    {
        //Quality manager related
        public bool m_showFPS = true;
        public bool m_showGUI = true;

        public bool m_autoQualityManagement = true;
        public float m_autoCheckInterval = 10f;
        public int m_targetFrameRate = 60;
        public int m_fontSize = 25;
        private float m_autoCheckTimeLeft = 0f;
        private int m_currentQuality = 0;
        private int m_maxQuality = 1;
        private string[] m_qualityNames;

        //FPS Related
        private float m_fpsUpdateInterval = 0.5f;
        private float m_fpsAccum = 0;
        private int m_fpsFrames = 0;
        private float m_fpsTimeLeft;
        private float m_fpsValue = 0f;
        private string m_fpsValueStr = string.Empty;
        private GUIStyle m_fpsStyle = new GUIStyle();
        private GUIStyle m_fpsShadowStyle = new GUIStyle();
        private Rect m_fpsLocation = new Rect(5, 5, 100, 25);
        private Rect m_fpsShadowLocation = new Rect(5 + 1, 5 + 1, 100, 25);

        void Start()
        {
            Application.targetFrameRate = m_targetFrameRate;
            m_maxQuality = QualitySettings.names.Length - 1;
            m_qualityNames = QualitySettings.names;
            m_currentQuality = QualitySettings.GetQualityLevel();
            m_autoCheckTimeLeft = m_autoCheckInterval;
            Debug.Log("Current quality is " + m_qualityNames[m_currentQuality]);

            //Increase the size of FPS
            m_fpsStyle.fontSize = m_fontSize;
            m_fpsShadowStyle.fontSize = m_fontSize;

            m_fpsTimeLeft = m_fpsUpdateInterval;
            m_fpsLocation = new Rect(Screen.width - 250, 5, 100, 25);
            m_fpsShadowLocation = new Rect(Screen.width - 250 + 1, 5 + 1, 100, 25);
        }

        void OnGUI()
        {
            if (!m_showFPS && !m_showGUI)
            {
                return;
            }

            if (m_showFPS)
            {
                GUI.Label(m_fpsShadowLocation, m_fpsValueStr, m_fpsShadowStyle);
                GUI.Label(m_fpsLocation, m_fpsValueStr, m_fpsStyle);
            }

            if (m_showGUI)
            {
                GUILayout.BeginVertical();
                for (int i = 0; i <= m_maxQuality; i++)
                {
                    if (GUILayout.Button(m_qualityNames[i]))
                    {
                        QualitySettings.SetQualityLevel(i, true);
                        m_currentQuality = i;
                        UpdateQuality();
                        m_autoQualityManagement = false; //Assume you are manually handling quality management
                    }
                }
                GUILayout.EndVertical();
            }
        }

        void Update()
        {
            m_fpsTimeLeft -= Time.deltaTime;
            m_fpsAccum += Time.timeScale / Time.deltaTime;
            ++m_fpsFrames;

            // Interval ended - update GUI text and start new interval
            if (m_fpsTimeLeft <= 0.0f)
            {
                m_fpsValue = m_fpsAccum / m_fpsFrames;

                if (m_fpsValue < 30)
                    m_fpsStyle.normal.textColor = Color.yellow;
                else if (m_fpsValue < 10)
                    m_fpsStyle.normal.textColor = Color.red;
                else
                    m_fpsStyle.normal.textColor = Color.green;

                m_fpsValueStr = System.String.Format("{0:f0} {1}", m_fpsValue, m_qualityNames[m_currentQuality].Substring(0, 3));

                m_fpsTimeLeft = m_fpsUpdateInterval;
                m_fpsAccum = 0.0F;
                m_fpsFrames = 0;
            }

            if (m_autoQualityManagement)
            {
                m_autoCheckTimeLeft -= Time.deltaTime;
                if (m_autoCheckTimeLeft < 0.0f)
                {
                    if ((m_fpsValue + 10f) >= m_targetFrameRate)
                    {
                        IncreaseQuality();
                    }
                    else if ((m_fpsValue - 10f) <= m_targetFrameRate)
                    {
                        DecreaseQuality();
                    }
                    m_autoCheckTimeLeft = m_autoCheckInterval;
                }
            }
        }

        void UpdateQuality()
        {

            //Dont change the settings if there is no change in quality
            if (QualitySettings.GetQualityLevel() == m_currentQuality)
            {
                return;
            }

            Debug.Log("Changing quality to " + m_qualityNames[m_currentQuality]);

            //Update the terrain quality settings
            switch (m_currentQuality)
            {
                case 0:
                    Terrain.activeTerrain.treeDistance = 250.0f;
                    Terrain.activeTerrain.treeBillboardDistance = 30.0f;
                    Terrain.activeTerrain.treeCrossFadeLength = 5.0f;
                    Terrain.activeTerrain.treeMaximumFullLODCount = 5;
                    Terrain.activeTerrain.detailObjectDistance = 40.0f;
                    Terrain.activeTerrain.detailObjectDensity = 0.1f;
                    Terrain.activeTerrain.heightmapPixelError = 20.0f;
                    Terrain.activeTerrain.heightmapMaximumLOD = 1;
                    Terrain.activeTerrain.basemapDistance = 100.0f;
                    break;

                case 1:
                    Terrain.activeTerrain.treeDistance = 500.0f;
                    Terrain.activeTerrain.treeBillboardDistance = 50.0f;
                    Terrain.activeTerrain.treeCrossFadeLength = 10.0f;
                    Terrain.activeTerrain.treeMaximumFullLODCount = 10;
                    Terrain.activeTerrain.detailObjectDistance = 50.0f;
                    Terrain.activeTerrain.detailObjectDensity = 0.25f;
                    Terrain.activeTerrain.heightmapPixelError = 10.0f;
                    Terrain.activeTerrain.heightmapMaximumLOD = 1;
                    Terrain.activeTerrain.basemapDistance = 250.0f;
                    break;

                case 2:
                    Terrain.activeTerrain.treeDistance = 650.0f;
                    Terrain.activeTerrain.treeBillboardDistance = 75.0f;
                    Terrain.activeTerrain.treeCrossFadeLength = 25.0f;
                    Terrain.activeTerrain.treeMaximumFullLODCount = 20;
                    Terrain.activeTerrain.detailObjectDistance = 60.0f;
                    Terrain.activeTerrain.detailObjectDensity = 0.4f;
                    Terrain.activeTerrain.heightmapPixelError = 8.0f;
                    Terrain.activeTerrain.heightmapMaximumLOD = 0;
                    Terrain.activeTerrain.basemapDistance = 500.0f;
                    break;

                case 3:
                    Terrain.activeTerrain.treeDistance = 800.0f;
                    Terrain.activeTerrain.treeBillboardDistance = 100.0f;
                    Terrain.activeTerrain.treeCrossFadeLength = 40.0f;
                    Terrain.activeTerrain.treeMaximumFullLODCount = 30;
                    Terrain.activeTerrain.detailObjectDistance = 80.0f;
                    Terrain.activeTerrain.detailObjectDensity = 0.7f;
                    Terrain.activeTerrain.heightmapPixelError = 5.0f;
                    Terrain.activeTerrain.heightmapMaximumLOD = 0;
                    Terrain.activeTerrain.basemapDistance = 800.0f;
                    break;

                case 4:
                    Terrain.activeTerrain.treeDistance = 1000.0f;
                    Terrain.activeTerrain.treeBillboardDistance = 150.0f;
                    Terrain.activeTerrain.treeCrossFadeLength = 50.0f;
                    Terrain.activeTerrain.treeMaximumFullLODCount = 50;
                    Terrain.activeTerrain.detailObjectDistance = 120.0f;
                    Terrain.activeTerrain.detailObjectDensity = 1f;
                    Terrain.activeTerrain.heightmapPixelError = 5f;
                    Terrain.activeTerrain.heightmapMaximumLOD = 0;
                    Terrain.activeTerrain.basemapDistance = 1000.0f;
                    break;

                case 5:
                    Terrain.activeTerrain.treeDistance = 2000.0f;
                    Terrain.activeTerrain.treeBillboardDistance = 200.0f;
                    Terrain.activeTerrain.treeCrossFadeLength = 50.0f;
                    Terrain.activeTerrain.treeMaximumFullLODCount = 100;
                    Terrain.activeTerrain.detailObjectDistance = 150f;
                    Terrain.activeTerrain.detailObjectDensity = 1f;
                    Terrain.activeTerrain.heightmapPixelError = 5f;
                    Terrain.activeTerrain.heightmapMaximumLOD = 0;
                    Terrain.activeTerrain.basemapDistance = 1000.0f;
                    break;
            }
        }

        bool IncreaseQuality()
        {
            if (m_currentQuality < m_maxQuality)
            {
                m_currentQuality++;
                UpdateQuality();
            }
            return true;
        }

        bool DecreaseQuality()
        {
            if (m_currentQuality > 0)
            {
                m_currentQuality--;
                UpdateQuality();
            }
            return true;
        }
    }
}