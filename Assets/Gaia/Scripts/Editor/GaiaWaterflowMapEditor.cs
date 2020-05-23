using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace Gaia
{
    /// <summary>
    /// Export current terrain as a water flow map
    /// </summary>
    public class GaiaWaterflowMapEditor : EditorWindow
    {
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private string m_maskName;
        private WaterFlowMap m_waterFlowMap = new WaterFlowMap();

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
            GUILayout.BeginVertical("Gaia WaterFlow Mask Exporter", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The Gaia waterflow exporter allows you to calculate and export a water flow mask from your terrain.", m_wrapStyle);
            GUILayout.EndVertical();

            if (string.IsNullOrEmpty(m_maskName))
            {
                m_maskName = string.Format("TerrainWaterFlow-{0:yyyyMMdd-HHmmss}", DateTime.Now);
            }
            m_maskName = EditorGUILayout.TextField(GetLabel("Mask Name"), m_maskName);

            m_waterFlowMap.m_dropletVolume = EditorGUILayout.Slider(GetLabel("Droplet Volume"), m_waterFlowMap.m_dropletVolume, 0.1f, 2f);
            m_waterFlowMap.m_dropletAbsorbtionRate = EditorGUILayout.Slider(GetLabel("Droplet Absorbtion Rate"), m_waterFlowMap.m_dropletAbsorbtionRate, 0.01f, 1f);
            m_waterFlowMap.m_waterflowSmoothIterations = EditorGUILayout.IntSlider(GetLabel("Smooth Iterations"), m_waterFlowMap.m_waterflowSmoothIterations, 0, 10);

            GUILayout.Space(5);

            EditorGUI.indentLevel++;
            if (DisplayButton(GetLabel("Create Mask")))
            {
                Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
                if (terrain == null)
                {
                    EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain!!", "OK");
                    return;
                }

                string path = "Assets/GaiaMasks/";
			    if (!Directory.Exists(path))
			    {
				    Directory.CreateDirectory(path);
			    }
			
        		path = Path.Combine(path, GaiaCommon1.Utils.FixFileName(m_maskName));

                m_waterFlowMap.CreateWaterFlowMap(terrain);
                m_waterFlowMap.ExportWaterMapToPath(path);


                GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
                mgr.LoadFromWorld();

                path += "WaterFlow";
                mgr.ExportWaterflowMapAsPng(m_waterFlowMap.m_waterflowSmoothIterations, path);

                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Done!", "Your mask is available at " + path, "OK");
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