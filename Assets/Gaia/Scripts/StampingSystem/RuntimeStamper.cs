using UnityEngine;
using System.Collections;
using System.IO;

namespace Gaia
{
    /// <summary>
    /// Runtime stamper sample - the stamp to be loaded and stamped my be stored in the resources directory
    /// </summary>
    public class RuntimeStamper : MonoBehaviour
    {
        //Show the gui
        public bool m_showGUI = true;

        //Show debug
        public bool m_showDebug = true;

        //Resource to be used as a stamp
        public string m_stampAddress = @"Gaia/Stamps/RuggedHills 1810 4";

        //Current progress
        public string m_currentProgress = "";

        //Current position
        #pragma warning disable 649
        private Rect m_currentPosition;
        #pragma warning restore 649

        //Stamper
        private Stamper m_stamper;

        //Set things up for first time execution
        void Awake()
        {
            //Dimensions of current position
            m_currentPosition.height = 20f;
            m_currentPosition.width = 300f;
        }

        // Use this for initialization
        void Start()
        {
            //Create the stamper
            CreateStamper();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            m_currentPosition.center = new Vector2(Screen.width / 2f, Screen.height - 20f);

            if (m_stamper != null)
            {
                m_currentProgress = string.Format("Stamp progress: " + m_stamper.m_stampProgress.ToString());
            }
        }

        //Display a GUI
        void OnGUI()
        {
            if (!m_showGUI)
            {
                return;
            }

            if (m_showGUI)
            {
                GUI.Label(m_currentPosition, m_currentProgress);
            }
        }

        //Create a stamper
        void CreateStamper()
        {
            string path = m_stampAddress;
            path = path.Replace("\\", "/");

            //Load the stamp
            TextAsset stamp = Resources.Load<TextAsset>(path);
            if (stamp == null)
            {
                m_currentProgress = "Failed to load stamp at " + path;
                if (m_showDebug)
                {
                    Debug.Log(m_currentProgress);
                }
            }
            else
            {
                m_currentProgress = "Loaded stamp at " + stamp.name;
                if (m_showDebug)
                {
                    if (m_showDebug)
                    {
                        Debug.Log(m_currentProgress);
                    }

                    GameObject stamper = new GameObject("Runtime Stamper");
                    m_stamper = stamper.AddComponent<Stamper>();
                    if (m_stamper.LoadRuntimeStamp(stamp) == true)
                    {
                        m_currentProgress = "Loaded Stamp";
                        m_stamper.FlattenTerrain();
                        m_stamper.FitToTerrain();
                        m_stamper.m_height = 6f;
                        m_stamper.m_distanceMask = AnimationCurve.Linear(0f, 1f, 1f, 0f);
                        m_stamper.m_rotation = 0f;
                        m_stamper.m_stickBaseToGround = true;
                        m_stamper.m_updateTimeAllowed = 1 / 15f;
                        m_stamper.UpdateStamp();
                        m_stamper.Stamp();
                    }
                    else
                    {
                        m_currentProgress = "Failed to load stamp";
                    }

                    if (m_showDebug)
                    {
                        Debug.Log(m_currentProgress);
                    }
                }
            }
        }
    }
}