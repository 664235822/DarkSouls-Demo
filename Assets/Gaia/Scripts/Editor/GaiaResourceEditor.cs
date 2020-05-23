using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Gaia {

    /// <summary>
    /// Editor for reource manager
    /// </summary>
    [CustomEditor(typeof(GaiaResource))]
    public class GaiaResourceEditor : Editor {

        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        GaiaResource m_resource;
        private DateTime m_lastSaveDT = DateTime.Now;

        void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            //Get our resource
            m_resource = (GaiaResource)target;

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
            GUILayout.BeginVertical("Gaia Resource", m_boxStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("The resource manager allows you to manage the resources used by your terrain and spawner. To see what every setting does you can hover over it.\n\nGet From Terrain - Pick up resources and settings from the current terrain.\n\nUpdate DNA - Updates DNA for all resources and automatically calculate sizes.\n\nApply To Terrain - Apply terrain specific settings such as texture, detail and tree prototypes back into the terrain. Prefab this settings to save time creating your next terrain.", m_wrapStyle);
            EditorGUILayout.LabelField("These are the resources used by the Spawning & Stamping system. Create a terrain, add textures, details and trees, then press Get Resources From Terrain to load. To see how the settings influence the system you can hover over them.", m_wrapStyle);
            GUILayout.EndVertical();

            float oldSeaLevel = m_resource.m_seaLevel;
            float oldHeight = m_resource.m_terrainHeight;

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            DropAreaGUI();

            GUILayout.BeginVertical("Resource Controller", m_boxStyle);
            GUILayout.Space(20);

            if (GUILayout.Button(GetLabel("Set Asset Associations")))
            {
                if (EditorUtility.DisplayDialog("Set Asset Associations", "This will update your asset associations and can not be undone ! Here temporarily until hidden.", "Yes", "No"))
                {
                    if (m_resource.SetAssetAssociations())
                    {
                        EditorUtility.SetDirty(m_resource);
                    }
                }
            }


            if (GUILayout.Button(GetLabel("Associate Assets")))
            {
                if (EditorUtility.DisplayDialog("Associate Assets", "This will locate and associate the first resource found that matches your asset and can not be undone !", "Yes", "No"))
                {
                    if (m_resource.AssociateAssets())
                    {
                        EditorUtility.SetDirty(m_resource);
                    }
                }
            }
 

            if (GUILayout.Button(GetLabel("Get Resources From Terrain")))
            {
                if (EditorUtility.DisplayDialog("Get Resources from Terrain ?", "Are you sure you want to get / update your resource prototypes from the terrain ? This will update your settings and can not be undone !", "Yes", "No"))
                {
                    m_resource.UpdatePrototypesFromTerrain();
                    EditorUtility.SetDirty(m_resource);
                }
            }

            if (GUILayout.Button(GetLabel("Replace Resources In Terrains")))
            {
                if (EditorUtility.DisplayDialog("Replace Resources in ALL Terrains ?", "Are you sure you want to replace the resources in ALL terrains with these? This can not be undone !", "Yes", "No"))
                {
                    m_resource.ApplyPrototypesToTerrain();
                }
            }

            if (GUILayout.Button(GetLabel("Add Missing Resources To Terrains")))
            {
                if (EditorUtility.DisplayDialog("Add Missing Resources to ALL Terrains ?", "Are you sure you want to add your missing resource prototypes to ALL terrains ? This can not be undone !", "Yes", "No"))
                {
                    m_resource.AddMissingPrototypesToTerrain();
                }
            }


            if (m_resource.m_texturePrototypes.GetLength(0) == 0)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(GetLabel("Create Coverage Texture Spawner")))
            {
                m_resource.CreateCoverageTextureSpawner(GetRangeFromTerrain(), GetTextureIncrementFromTerrain());
            }
            GUI.enabled = true;


            if (m_resource.m_detailPrototypes.GetLength(0) == 0)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(GetLabel("Create Clustered Grass Spawner")))
            {
                m_resource.CreateClusteredDetailSpawner(GetRangeFromTerrain(), GetDetailIncrementFromTerrain());
            }
            if (GUILayout.Button(GetLabel("Create Coverage Grass Spawner")))
            {
                m_resource.CreateCoverageDetailSpawner(GetRangeFromTerrain(), GetDetailIncrementFromTerrain());
            }
            GUI.enabled = true;


            if (m_resource.m_treePrototypes.GetLength(0) == 0)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(GetLabel("Create Clustered Terrain Tree Spawner")))
            {
                m_resource.CreateClusteredTreeSpawner(GetRangeFromTerrain());
            }
            if (GUILayout.Button(GetLabel("Create Coverage Terrain Tree Spawner")))
            {
                m_resource.CreateCoverageTreeSpawner(GetRangeFromTerrain());
            }
            GUI.enabled = true;

            if (m_resource.m_gameObjectPrototypes.GetLength(0) == 0)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(GetLabel("Create Clustered Prefab Spawner")))
            {
                m_resource.CreateClusteredGameObjectSpawner(GetRangeFromTerrain());
            }
            if (GUILayout.Button(GetLabel("Create Coverage Prefab Spawner")))
            {
                m_resource.CreateCoverageGameObjectSpawner(GetRangeFromTerrain());
            }
            GUI.enabled = true;

            if (GUILayout.Button(GetLabel("Visualise")))
            {
                GameObject gaiaObj = GameObject.Find("Gaia");
                if (gaiaObj == null)
                {
                    gaiaObj = new GameObject("Gaia");
                }
                GameObject visualiserObj = GameObject.Find("Visualiser");
                if (visualiserObj == null)
                {
                    visualiserObj = new GameObject("Visualiser");
                    visualiserObj.AddComponent<ResourceVisualiser>();
                    visualiserObj.transform.parent = gaiaObj.transform;
                }
                ResourceVisualiser visualiser = visualiserObj.GetComponent<ResourceVisualiser>();
                visualiser.m_resources = m_resource;
                Selection.activeGameObject = visualiserObj;
            }

            GUILayout.Space(5f);
            GUILayout.EndVertical();

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                if (oldHeight != m_resource.m_terrainHeight)
                {
                    m_resource.ChangeHeight(oldHeight, m_resource.m_terrainHeight);
                }

                if (oldSeaLevel != m_resource.m_seaLevel)
                {
                    m_resource.ChangeSeaLevel(oldSeaLevel, m_resource.m_seaLevel);
                }
                Undo.RecordObject(m_resource, "Made resource changes");
                EditorUtility.SetDirty(m_resource);

                //Stop the save from going nuts
                if ((DateTime.Now - m_lastSaveDT).Seconds > 5)
                {
                    m_lastSaveDT = DateTime.Now;
                    AssetDatabase.SaveAssets();
                }
            }
        }

        public void DropAreaGUI()
        {
            //Drop out if no resource selected
            if (m_resource == null)
            {
                return;
            }

            //Ok - set up for drag and drop
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "Drop Game Objects / Prefabs Here", m_boxStyle);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

#if UNITY_2018_3_OR_NEWER
                        //Work out if we have prefab instances or prefab objects
                        bool havePrefabInstances = false;
                        foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                        {
                            PrefabAssetType pt = PrefabUtility.GetPrefabAssetType(dragged_object);

                            if (pt == PrefabAssetType.Regular || pt == PrefabAssetType.Model)
                            {
                                havePrefabInstances = true;
                                break;
                            }
                        }

                        if (havePrefabInstances)
                        {
                            List<GameObject> prototypes = new List<GameObject>();

                            foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                            {
                                PrefabAssetType pt = PrefabUtility.GetPrefabAssetType(dragged_object);

                                if (pt == PrefabAssetType.Regular || pt == PrefabAssetType.Model)
                                {
                                    prototypes.Add(dragged_object as GameObject);
                                }
                                else
                                {
                                    Debug.LogWarning("You may only add prefab instances!");
                                }
                            }

                            //Same them as a single entity
                            if (prototypes.Count > 0)
                            {
                                m_resource.AddGameObject(prototypes);
                            }
                        }
                        else
                        {
                            foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                            {
                                if (PrefabUtility.GetPrefabAssetType(dragged_object) == PrefabAssetType.Regular)
                                {
                                    m_resource.AddGameObject(dragged_object as GameObject);
                                }
                                else
                                {
                                    Debug.LogWarning("You may only add prefabs or game objects attached to prefabs!");
                                }
                            }
                        }
#else

                        //Work out if we have prefab instances or prefab objects
                        bool havePrefabInstances = false;
                        foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                        {
                            PrefabType pt = PrefabUtility.GetPrefabType(dragged_object);

                            if (pt == PrefabType.PrefabInstance || pt == PrefabType.ModelPrefabInstance)
                            {
                                havePrefabInstances = true;
                                break;
                            }
                        }

                        if (havePrefabInstances)
                        {
                            List<GameObject> prototypes = new List<GameObject>();

                            foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                            {
                                PrefabType pt = PrefabUtility.GetPrefabType(dragged_object);

                                if (pt == PrefabType.PrefabInstance || pt == PrefabType.ModelPrefabInstance)
                                {
                                    prototypes.Add(dragged_object as GameObject);
                                }
                                else
                                {
                                    Debug.LogWarning("You may only add prefab instances!");
                                }
                            }

                            //Same them as a single entity
                            if (prototypes.Count > 0)
                            {
                                m_resource.AddGameObject(prototypes);
                            }
                        }
                        else
                        {
                            foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                            {
                                if (PrefabUtility.GetPrefabType(dragged_object) == PrefabType.Prefab)
                                {
                                    m_resource.AddGameObject(dragged_object as GameObject);
                                }
                                else
                                {
                                    Debug.LogWarning("You may only add prefabs or game objects attached to prefabs!");
                                }
                            }
                        }
#endif
                    }
                    break;
            }
        }


        /// <summary>
        /// Get the range from the terrain
        /// </summary>
        /// <returns>Range from currently active terrain or 1024f</returns>
        private float GetRangeFromTerrain()
        {
            float range = 1024f;
            Terrain t = Gaia.TerrainHelper.GetActiveTerrain();
            if (t != null)
            {
                range = Mathf.Max(t.terrainData.size.x, t.terrainData.size.z) / 2f;
            }
            return range;
        }

        /// <summary>
        /// Get texture increment from terrain
        /// </summary>
        /// <returns></returns>
        private float GetTextureIncrementFromTerrain()
        {
            float increment = 1f;
            Terrain t = Gaia.TerrainHelper.GetActiveTerrain();
            if (t != null)
            {
                if (t.terrainData != null)
                {
                    increment = Mathf.Max(t.terrainData.size.x, t.terrainData.size.z) / (float)t.terrainData.alphamapResolution;
                }
            }
            return increment;
        }

        /// <summary>
        /// Get detail increment from terrain
        /// </summary>
        /// <returns></returns>
        private float GetDetailIncrementFromTerrain()
        {
            float increment = 1f;
            Terrain t = Gaia.TerrainHelper.GetActiveTerrain();
            if (t != null)
            {
                if (t.terrainData != null)
                {
                    increment = Mathf.Max(t.terrainData.size.x, t.terrainData.size.z) / (float)t.terrainData.detailResolution;
                }
            }
            return increment;
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
            { "Get From Terrain", "Get or update the resource prototypes from the current terrain." },
            { "Apply To Terrains", "Apply the resource prototypes into all existing terrains." },
            { "Visualise", "Visualise the fitness of resource prototypes." },
        };

    }
}
