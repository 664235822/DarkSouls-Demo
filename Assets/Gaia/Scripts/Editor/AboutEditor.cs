using UnityEngine;
using UnityEditor;
using Gaia.Internal;

namespace Gaia
{
    /// <summary>
    /// Handy helper for all things Gaia
    /// </summary>
    public class AboutEditor : EditorWindow
    {
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;

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
            GUILayout.BeginVertical("About Gaia", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Gaia by Procedural Worlds automates the majority of the more tedious work in creating gorgeous environments. It was crafted with much love. Enjoy!", m_wrapStyle);
            if (GUILayout.Button("Visit GAIA Forums"))
            {
                Application.OpenURL("http://forum.unity3d.com/threads/gaia-aaa-terrain-generator-procedural-texturing-planting-and-scene-creation.327342/");
            }
            if (GUILayout.Button("Visit Procedural Worlds"))
            {
                Application.OpenURL("http://www.procedural-worlds.com");
            }
            if (GUILayout.Button("Like GAIA? Support with a Review :)"))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/42618");
            }
            EditorGUILayout.LabelField(string.Format("Version: {0}", PWApp.CONF.Version), m_wrapStyle);
            GUILayout.EndVertical();
        }
    }
}
