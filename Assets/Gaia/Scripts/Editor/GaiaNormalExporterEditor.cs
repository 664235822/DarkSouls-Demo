using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gaia
{
    /// <summary>
    /// Class to export terrain normal mask as a texture
    /// </summary>
    public class GaiaNormalExporterEditor : EditorWindow
    {
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private string m_maskName;

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
            GUILayout.BeginVertical("Gaia Terrain Normal Exporter", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The Gaia terrain normal exporter allows you to export your terrain normals as a texture.", m_wrapStyle);
            GUILayout.EndVertical();

            if (string.IsNullOrEmpty(m_maskName))
            {
                m_maskName = string.Format("Terrain-Normals-{0:yyyyMMdd-HHmmss}", DateTime.Now);
            }
            m_maskName = EditorGUILayout.TextField(GetLabel("Texture Name"), m_maskName);

            GUILayout.Space(5);

            EditorGUI.indentLevel++;
            if (DisplayButton(GetLabel("Export Normal Map")))
            {
                ExportNormal();
            }
            EditorGUI.indentLevel--;

        }

        private void ExportNormal()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                string path = "Assets/GaiaMasks/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                mgr.LoadFromWorld();
                path = Path.Combine(path, GaiaCommon1.Utils.FixFileName(m_maskName));
                mgr.ExportNormalmapAsPng(path);
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Export complete", " Your normal map has been saved to : " + path, "OK");
            }
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