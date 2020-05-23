using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;

namespace Gaia
{

    /// <summary>
    /// Editor for shoreline masker
    /// </summary>
    public class ShorelineMaskerEditor : EditorWindow
    {

        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        public float m_seaLevel = 50f;
        public int m_maskSize = 20;
        public float m_aboveWaterStrength = 1f;

        void OnEnable()
        {
        }

        public void OnGUI()
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
                m_wrapStyle.wordWrap = true;
            }

            //Create a nice text intro
            GUILayout.BeginVertical("Shoreline Masker", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Use this tool to make a mask of the active terrain at the height provided. The mask will be created in the assets directory.", m_wrapStyle);
            GUILayout.EndVertical();

            m_seaLevel = EditorGUILayout.FloatField("Sea Level", m_seaLevel);
            m_maskSize = EditorGUILayout.IntField("Mask Size", m_maskSize);
            m_aboveWaterStrength = EditorGUILayout.Slider("Above Water Value", m_aboveWaterStrength, 0f, 1f);

            if (GUILayout.Button("Create ShoreLine Mask"))
            {
                CreateMask();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
            }
        }


        /// <summary>
        /// Create the mask and put it in the root assets directory
        /// </summary>
        public void CreateMask()
        {
            // Check to see if we have an active terrain
            if (Terrain.activeTerrain == null)
            {
                Debug.LogError("You need an active terrain!");
                return;
            }

            // Get the terrain
            UnityHeightMap hm = new UnityHeightMap(Terrain.activeTerrain);

            // Create a heightmap for the mask
            HeightMap maskHm = new HeightMap(hm.Width(), hm.Depth());

            //Iterate though our terrain, at the resolution of the heightmap and pick up the height
            float testHeight;
            float nrmSeaLevel = m_seaLevel / Terrain.activeTerrain.terrainData.size.y;

            for (int x = 0; x < hm.Width(); x++)
            {
                for (int z = 0; z < hm.Depth(); z++)
                {
                    testHeight = hm[x, z];
                    if (testHeight < nrmSeaLevel)
                    {
                        continue;
                    }
                    else if (Gaia.GaiaUtils.Math_ApproximatelyEqual(testHeight, nrmSeaLevel))
                    {
                        //We have a shoreline, lets mask it
                        maskHm[x, z] = m_aboveWaterStrength;
                        MakeMask(x, z, nrmSeaLevel, m_maskSize, hm, maskHm);
                    }
                    else
                    {
                        maskHm[x, z] = m_aboveWaterStrength;
                    }
                }
            }

            maskHm.Flip();
            //maskHm.Smooth(1);


            string path = "Assets/GaiaMasks/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, GaiaCommon1.Utils.FixFileName(string.Format("TerrainShoreLineMask-{0:yyyyMMdd-HHmmss}", DateTime.Now)));
            Gaia.GaiaUtils.CompressToSingleChannelFileImage(maskHm.Heights(), path, TextureFormat.RGBA32, false, true);

            EditorUtility.DisplayDialog("Shoreline Masker", "Your shorline mask has been created at " + path, "OK");
        }


        /// <summary>
        /// Make a mask by iterating out from this location 
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startZ"></param>
        /// <param name="waterHeight"></param>
        /// <param name="maskSize"></param>
        /// <param name="hm"></param>
        /// <param name="maskHm"></param>
        private void MakeMask(int startX, int startZ, float waterHeight, int maskSize, HeightMap hm, HeightMap maskHm)
        {
            int width = hm.Width();
            int depth = hm.Depth();
            int minX = startX - maskSize;
            int maxX = startX + maskSize;
            int minZ = startZ - maskSize;
            int maxZ = startZ + maskSize;
            float strength;

            if (minX < 0)
            {
                minX = 0;
            }
            if (maxX >= width)
            {
                maxX = width;
            }
            if (minZ < 0)
            {
                minZ = 0;
            }
            if (maxZ >= depth)
            {
                maxZ = depth;
            }

            //Make the mask if height is below sea level
            for (int x = minX; x < maxX; x++)
            {
                for (int z = minZ; z < maxZ; z++)
                {
                    if (hm[x, z] <= waterHeight)
                    {
                        strength = Gaia.GaiaUtils.Math_Distance(startX, startZ, x, z) / (float)maskSize;
                        if (strength <= 1f)
                        {
                            strength = 1f - strength;
                            if (strength > maskHm[x, z])
                            {
                                maskHm[x, z] = strength;
                            }
                        }
                    }
                }
            }
        }
    }
}