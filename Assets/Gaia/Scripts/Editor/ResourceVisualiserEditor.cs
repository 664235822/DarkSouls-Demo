using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// Editor for reource visualiser
    /// </summary>
    [CustomEditor(typeof(ResourceVisualiser))]
    public class ResourceVisualiserEditor : Editor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        ResourceVisualiser m_visualiser;
        private float m_seaLevel = 0f;

        void OnEnable()
        {
            //Grab the active terrain height map
            if (Gaia.TerrainHelper.GetActiveTerrainCount() == 1 && Gaia.TerrainHelper.GetActiveTerrain() != null)
            {
                //Get sea level
                GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
                m_seaLevel = sceneInfo.m_seaLevel;

                //Get our manager
                m_visualiser = (ResourceVisualiser)target;
                m_visualiser.Initialise();
                m_visualiser.Visualise();

                //Update resource sea level
                m_visualiser.m_resources.ChangeSeaLevel(m_seaLevel);
            }
        }

        public override void OnInspectorGUI()
        {
            //Get our manager
            m_visualiser = (ResourceVisualiser)target;

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
                m_wrapStyle.wordWrap = true;
            }

            //Create a nice text intro
            GUILayout.BeginVertical("Resource Visualiser", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The resource visualiser allows you to visualise and edit your resources spawn criteria. Visualiser is CPU intensive, so make range smaller, and resolution larger if you suffer from lag.", m_wrapStyle);
            GUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (m_visualiser.m_resources == null)
            {
                GUILayout.BeginVertical("ERROR!!!", m_boxStyle);
                GUILayout.Space(20);
                EditorGUILayout.LabelField("You must select a resources file to use the Visualiser!!", m_wrapStyle);
                GUILayout.EndVertical();
                EditorGUI.EndChangeCheck();
                return;
            }

            //Create a nice text intro
            GUILayout.BeginVertical("Resource Editor", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Select and edit your resource and its spawn criteria here. Changes will show in real time.", m_wrapStyle);
            GUILayout.EndVertical();

            m_visualiser.m_selectedResourceType = (Gaia.GaiaConstants.SpawnerResourceType)EditorGUILayout.EnumPopup(GetLabel("Resource Type"), m_visualiser.m_selectedResourceType);

            GUIContent[] assetChoices;
            switch (m_visualiser.m_selectedResourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        assetChoices = new GUIContent[m_visualiser.m_resources.m_texturePrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_visualiser.m_resources.m_texturePrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_visualiser.m_resources.m_texturePrototypes[assetIdx].m_name);
                        }
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        assetChoices = new GUIContent[m_visualiser.m_resources.m_detailPrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_visualiser.m_resources.m_detailPrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_visualiser.m_resources.m_detailPrototypes[assetIdx].m_name);
                        }
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        assetChoices = new GUIContent[m_visualiser.m_resources.m_treePrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_visualiser.m_resources.m_treePrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_visualiser.m_resources.m_treePrototypes[assetIdx].m_name);
                        }
                        break;
                    }
                default:
                    {
                        assetChoices = new GUIContent[m_visualiser.m_resources.m_gameObjectPrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_visualiser.m_resources.m_gameObjectPrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_visualiser.m_resources.m_gameObjectPrototypes[assetIdx].m_name);
                        }
                        break;
                    }
            }


            if (assetChoices.Length > 0)
            {
                m_visualiser.m_selectedResourceIdx = EditorGUILayout.Popup(GetLabel("Selected Resource"), m_visualiser.m_selectedResourceIdx, assetChoices);

                //Then select and display the editor
                switch (m_visualiser.m_selectedResourceType)
                {
                    case GaiaConstants.SpawnerResourceType.TerrainTexture:
                        {
                            if (m_visualiser.m_selectedResourceIdx >= m_visualiser.m_resources.m_texturePrototypes.Length)
                            {
                                m_visualiser.m_selectedResourceIdx = 0;
                            }
                            ResourceProtoTextureSO so = ScriptableObject.CreateInstance<ResourceProtoTextureSO>();
                            so.m_texture = m_visualiser.m_resources.m_texturePrototypes[m_visualiser.m_selectedResourceIdx];
                            var ed = Editor.CreateEditor(so);
                            ed.OnInspectorGUI();
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainDetail:
                        {
                            //Fix up indexes
                            if (m_visualiser.m_selectedResourceIdx >= m_visualiser.m_resources.m_detailPrototypes.Length)
                            {
                                m_visualiser.m_selectedResourceIdx = 0;
                            }
                            ResourceProtoDetailSO so = ScriptableObject.CreateInstance<ResourceProtoDetailSO>();
                            so.m_detail = m_visualiser.m_resources.m_detailPrototypes[m_visualiser.m_selectedResourceIdx];
                            var ed = Editor.CreateEditor(so);
                            ed.OnInspectorGUI();
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainTree:
                        {
                            //Fix up indexes
                            if (m_visualiser.m_selectedResourceIdx >= m_visualiser.m_resources.m_treePrototypes.Length)
                            {
                                m_visualiser.m_selectedResourceIdx = 0;
                            }
                            ResourceProtoTreeSO so = ScriptableObject.CreateInstance<ResourceProtoTreeSO>();
                            so.m_tree = m_visualiser.m_resources.m_treePrototypes[m_visualiser.m_selectedResourceIdx];
                            var ed = Editor.CreateEditor(so);
                            ed.OnInspectorGUI();
                            break;
                        }
                    default:
                        {
                            //Fix up indexes
                            if (m_visualiser.m_selectedResourceIdx >= m_visualiser.m_resources.m_gameObjectPrototypes.Length)
                            {
                                m_visualiser.m_selectedResourceIdx = 0;
                            }
                            ResourceProtoGameObjectSO so = ScriptableObject.CreateInstance<ResourceProtoGameObjectSO>();
                            so.m_gameObject = m_visualiser.m_resources.m_gameObjectPrototypes[m_visualiser.m_selectedResourceIdx];
                            var ed = Editor.CreateEditor(so);
                            ed.OnInspectorGUI();
                            break;
                        }
                }
            }

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_visualiser.m_resources, "Made resource changes");
                EditorUtility.SetDirty(m_visualiser.m_resources);
            }

            //Update some key fields in the spawner
            m_visualiser.m_spawner.m_resources = m_visualiser.m_resources;
            m_visualiser.m_spawner.m_spawnCollisionLayers = m_visualiser.m_fitnessCollisionLayers;
            m_visualiser.m_spawner.m_spawnRange = m_visualiser.m_range;

            //Terrain info
            //Create a nice text intro
            GUILayout.BeginVertical("Terrain Info", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Hit control key to view detailed information at current mouse position - mouse must be over the visualiser to register data.", m_wrapStyle);
            GUILayout.EndVertical();

            EditorGUILayout.Vector3Field(GetLabel("Location"), m_visualiser.m_lastHitPoint);

            EditorGUILayout.Toggle(GetLabel("Virgin"), m_visualiser.m_lastHitWasVirgin);
            if (!string.IsNullOrEmpty(m_visualiser.m_lastHitObjectname))
            {
                EditorGUILayout.TextField(GetLabel("Hit Object"), m_visualiser.m_lastHitObjectname);
            }
            EditorGUILayout.FloatField(GetLabel("Hit Height"), m_visualiser.m_lastHitHeight);
            EditorGUILayout.FloatField(GetLabel("Terrain Height"), m_visualiser.m_lastHitTerrainHeight);
            EditorGUILayout.FloatField(GetLabel("Height Above Sea"), m_visualiser.m_lastHitTerrainRelativeHeight);
            EditorGUILayout.FloatField(GetLabel("Terrain Slope"), m_visualiser.m_lastHitTerrainSlope);
            EditorGUILayout.FloatField(GetLabel("Area Slope"), m_visualiser.m_lastHitAreaSlope);
            EditorGUILayout.FloatField(GetLabel("Fitness"), m_visualiser.m_lastHitFitness);
        }

        void OnSceneGUI()
        {
            if (Event.current.control == true)
            {
                //Work out where the mouse is and get fitness
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 10000f))
                {
                    if (hitInfo.point != m_visualiser.m_lastHitPoint)
                    {
                        m_visualiser.m_lastHitPoint = hitInfo.point;
                        SpawnInfo spawnInfo = m_visualiser.GetSpawnInfo(hitInfo.point);

                        if (!spawnInfo.m_outOfBounds)
                        {
                            m_visualiser.m_lastHitObjectname = spawnInfo.m_hitObject.name;
                            m_visualiser.m_lastHitFitness = spawnInfo.m_fitness;
                            m_visualiser.m_lastHitHeight = spawnInfo.m_hitLocationWU.y;
                            m_visualiser.m_lastHitTerrainHeight = spawnInfo.m_terrainHeightWU;
                            m_visualiser.m_lastHitTerrainRelativeHeight = m_visualiser.m_lastHitTerrainHeight - spawnInfo.m_spawner.m_resources.m_seaLevel;
                            m_visualiser.m_lastHitTerrainSlope = spawnInfo.m_terrainSlopeWU;
                            m_visualiser.m_lastHitAreaSlope = spawnInfo.m_areaAvgSlopeWU;
                            m_visualiser.m_lastHitWasVirgin = spawnInfo.m_wasVirginTerrain;
                        }
                        else
                        {
                            m_visualiser.m_lastHitPoint = spawnInfo.m_hitLocationWU;
                            m_visualiser.m_lastHitObjectname = "Out of BOUNDS";
                            m_visualiser.m_lastHitFitness = 0f;
                            m_visualiser.m_lastHitHeight = 0f;
                            m_visualiser.m_lastHitTerrainHeight = 0f;
                            m_visualiser.m_lastHitTerrainRelativeHeight = 0f;
                            m_visualiser.m_lastHitTerrainSlope = 0f;
                            m_visualiser.m_lastHitAreaSlope = 0f;
                            m_visualiser.m_lastHitWasVirgin = false;
                        }
                    }
                }
                else
                {
                    m_visualiser.m_lastHitPoint = ray.origin;
                    m_visualiser.m_lastHitObjectname = "Out of BOUNDS";
                    m_visualiser.m_lastHitFitness = 0f;
                    m_visualiser.m_lastHitHeight = 0f;
                    m_visualiser.m_lastHitTerrainHeight = 0f;
                    m_visualiser.m_lastHitTerrainRelativeHeight = 0f;
                    m_visualiser.m_lastHitTerrainSlope = 0f;
                    m_visualiser.m_lastHitAreaSlope = 0f;
                    m_visualiser.m_lastHitWasVirgin = false;
                }
            }
        }


        /// <summary>
        /// Get a content label - look the tooltip up if possible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GUIContent GetLabel(string name)
        {
            string tooltip = "";
            if (m_tooltips.TryGetValue(name, out tooltip))
            {
                return new GUIContent(name, tooltip);
            }
            else
            {
                return new GUIContent(name);
            }
        }

        /// <summary>
        /// The tooltips
        /// </summary>
        static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        {
            { "Location", "Location in world units where the raycast hit occurred." },
            { "Virgin", "True when only terrain is detected at this location. False if any other object with a collider is hit." },
            { "Hit Object", "The name of the object that was detected at this location." },
            { "Hit Height", "The height at which the hit was detected in world units." },
            { "Terrain Height", "The terrain height at this locaiton world units." },
            { "Height Above Sea", "The terrain height relative to sea level at which the hit was detected in world units." },
            { "Terrain Slope", "The terrain slope at this location - 0 == flat, 90 == vertical." },
            { "Area Slope", "The average terrain slope across the bounded area at this location based on bounds radius in the objects dna when bounded area check selected - 0 == flat, 90 == vertical." },
            { "Fitness", "Fitness at this location." },
        };

    }
}