using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gaia
{
    public class GaiaTerrainHeightAdjuster : EditorWindow
    {

        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private float m_terrainHeight = 0f;
        private float m_maxTerrainHeight = 1000f;

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

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.fontStyle = FontStyle.Normal;
                m_wrapStyle.wordWrap = true;
            }

            //Text intro
            GUILayout.BeginVertical("Gaia Terrain Height Adjuster", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The terrain height adjuster allows you to set the height of the terrains in your scene to the given height.", m_wrapStyle);
            GUILayout.EndVertical();

            Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
            if (terrain != null)
            {
                m_maxTerrainHeight = terrain.terrainData.size.y;
            }
            
            m_terrainHeight = EditorGUILayout.Slider(GetLabel("New Terrain Height (m)"), m_terrainHeight, 0f, m_maxTerrainHeight);

            GUILayout.Space(5);

            EditorGUI.indentLevel++;
            if (DisplayButton(GetLabel("Adjust Terrain Heights")))
            {
                if (terrain == null)
                {
                    EditorUtility.DisplayDialog("OOPS!", "You must have at least one active terrain in your scene to use this feature!", "OK");
                    return;
                }

                GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
                mgr.LoadFromWorld();
                mgr.SetHeightWU(m_terrainHeight);
                mgr.SaveToWorld();
            }
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Display a button that takes editor indentation into account
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        bool DisplayButton(GUIContent content)
        {
            TextAnchor oldalignment = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            Rect btnR = EditorGUILayout.BeginHorizontal();
            btnR.xMin += (EditorGUI.indentLevel * 18f);
            btnR.height += 20f;
            btnR.width -= 4f;
            bool result = GUI.Button(btnR, content);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(22);
            GUI.skin.button.alignment = oldalignment;
            return result;
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
            { "Execution Mode", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
        };


    }
}
