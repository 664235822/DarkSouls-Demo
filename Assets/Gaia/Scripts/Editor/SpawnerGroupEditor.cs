using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Gaia
{
    [CustomEditor(typeof(SpawnerGroup))]
    public class SpawnerGroupEditor : Editor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        SpawnerGroup m_spawnerGroup;
        DateTime m_timeSinceLastUpdate = DateTime.Now;
        bool m_startedUpdates = false;

        void OnEnable()
        {
            m_spawnerGroup = (SpawnerGroup)target;
            StartEditorUpdates();
        }

        void OnDisable()
        {
        }

        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            if (!m_startedUpdates)
            {
                m_startedUpdates = true;
                EditorApplication.update += EditorUpdate;
            }
        }

        /// <summary>
        /// Stop editor updates
        /// </summary>
        public void StopEditorUpdates()
        {
            if (m_startedUpdates)
            {
                m_startedUpdates = false;
                EditorApplication.update -= EditorUpdate;
            }
        }

        /// <summary>
        /// This is used just to force the editor to repaint itself
        /// </summary>
        void EditorUpdate()
        {
            if (m_spawnerGroup != null)
            {
                if (m_spawnerGroup.m_updateCoroutine != null)
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalMilliseconds > 500)
                    {
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
                else
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalSeconds > 5)
                    {
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            //Get our spawner
            m_spawnerGroup = (SpawnerGroup)target;

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

            //Fix up names if necessary
            m_spawnerGroup.FixNames();

            //Create a nice text intro
            GUILayout.BeginVertical("Spawner Group", m_boxStyle);
                GUILayout.Space(20);
                EditorGUILayout.LabelField("A Spawner Group allows you to chain a set of spawners together. The command buttons you run will be run on all sub spawners. It may take a little while so be patient.", m_wrapStyle);
            GUILayout.EndVertical();

            //Disable if spawning
            if (m_spawnerGroup.m_updateCoroutine != null)
            {
                GUI.enabled = false;
            }

            DrawDefaultInspector();

            GUILayout.BeginVertical("Terrain Helper", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();

                    if (GUILayout.Button(GetLabel("Flatten")))
                    {
                        if (EditorUtility.DisplayDialog("Flatten Terrain tiles ?", "Are you sure you want to flatten all terrain tiles - this can not be undone ?", "Yes", "No"))
                        {
                            TerrainHelper.Flatten();
                        }
                    }
                    if (GUILayout.Button(GetLabel("Smooth")))
                    {
                        if (EditorUtility.DisplayDialog("Smooth Terrain tiles ?", "Are you sure you want to smooth all terrain tiles - this can not be undone ?", "Yes", "No"))
                        {
                            TerrainHelper.Smooth(1);
                        }
                    }
                    if (GUILayout.Button(GetLabel("Clear Trees")))
                    {
                        if (EditorUtility.DisplayDialog("Clear Terrain trees ?", "Are you sure you want to clear all terrain trees - this can not be undone ?", "Yes", "No"))
                        {
                            TerrainHelper.ClearTrees();
                        }
                    }
                    if (GUILayout.Button(GetLabel("Clear Details")))
                    {
                        if (EditorUtility.DisplayDialog("Clear Terrain details ?", "Are you sure you want to clear all terrain details - this can not be undone ?", "Yes", "No"))
                        {
                            TerrainHelper.ClearDetails();
                        }
                    }

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            GUILayout.EndVertical();

            //Re-enable
            GUI.enabled = true;

            //Display progress
            if (m_spawnerGroup.m_updateCoroutine != null)
            {
                GUILayout.BeginVertical("Spawner Controller", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Cancel")))
                {
                    m_spawnerGroup.CancelSpawn();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();

                //Draw the various progress bars
                for (int idx = 0; idx < m_spawnerGroup.m_spawners.Count; idx++)
                {
                    //Display progress
                    if (m_spawnerGroup.m_spawners[idx].m_spawner.m_spawnProgress > 0f && m_spawnerGroup.m_spawners[idx].m_spawner.m_spawnProgress < 1f)
                    {
                        ProgressBar(string.Format("{0} ({1:0.0}%)", m_spawnerGroup.m_spawners[idx].m_name, m_spawnerGroup.m_spawners[idx].m_spawner.m_spawnProgress * 100f), m_spawnerGroup.m_spawners[idx].m_spawner.m_spawnProgress);
                    }
                }
            }
            else
            {
                GUILayout.BeginVertical("Spawner Controller", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Reset")))
                {
                    m_spawnerGroup.ResetSpawner();
                }

                if (GUILayout.Button(GetLabel("Spawn")))
                {
                    //Check that they have a single selected terrain
                    if (Gaia.TerrainHelper.GetActiveTerrainCount() != 1)
                    {
                        EditorUtility.DisplayDialog("OOPS!", "You must have only one active terrain in order to use a Spawner Group. Please either add a terrain, or deactivate all but one terrain.", "OK");
                    }
                    else
                    {
                        m_spawnerGroup.RunSpawnerIteration();
                        StartEditorUpdates();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
                GUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Draw a progress bar
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        void ProgressBar(string label, float value)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
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
            { "Reset", "Reset the spawner back to its initial state. Starting spawn again will generate exact same result provided that nothing else in the terrain has changed." },
            { "Spawn", "Run a single spawn interation." },
            { "Flatten", "Flatten the entire terrain - use with care!" },
            { "Smooth", "Smooth the entire terrain - removes jaggies and increases frame rate - run multiple times to increase effect - use with care!" },
            { "Clear Trees", "Clear trees from entire terrain - use with care!" },
            { "Clear Details", "Clear details / grass from entire terrain - use with care!" },
        };
    }
}