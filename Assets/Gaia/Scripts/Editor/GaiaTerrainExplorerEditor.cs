using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gaia
{
    /// <summary>
    /// Some cool terrain modification utilities
    /// </summary>
    public class GaiaTerrainExplorerEditor : EditorWindow
    {
        private EditorUtilsOLD m_editorUtils = new EditorUtilsOLD();
        private GaiaConstants.TerrainOperationType m_operationType = GaiaConstants.TerrainOperationType.QuantizeCurvesFilter;
        private Vector2 m_scrollPosition = Vector2.zero;

        //Erosion related
        private int m_iterations = 20;

        //Thermal erosion
        private float m_talusMin = 0.001f;
        private float m_talusMax = 0.1f;

        //Hydraulic erosion
        private int m_rainFreq = 5;
        private float m_disolveRate = 0.01f;
        private GaiaConstants.ErosionRainType m_rainmapType = GaiaConstants.ErosionRainType.Constant;
        private HeightMap m_rainMap;
        private HeightMap m_hardnessMap;
        private Texture2D m_hardnessMapTexture;

        //Mask1
        private bool m_useErosionMask1 = false;
        private bool m_normalizeErosionMask1 = false;
        private bool m_invertErosionMask1 = false;
        private bool m_flipErosionMask1 = false;
        private float m_erosionMaskStrength1 = 1f;
        private float m_erosionMaskOffset1 = 0f;
        private float m_erosionMaskPowerOf1 = 1f;
        private float m_erosionMaskContrast1 = 1f;
        private float m_quantizeMask1 = 0f;
        private int m_blurMask1 = 0;
        private Texture2D m_erosionMaskTexture1;
        private DateTime m_erosionMaskDT1 = DateTime.MinValue;
        private HeightMap m_erosionMask1;

        //Mask2
        private bool m_useErosionMask2 = false;
        private bool m_normalizeErosionMask2 = false;
        private bool m_invertErosionMask2 = false;
        private bool m_flipErosionMask2 = false;
        private float m_erosionMaskStrength2 = 1f;
        private Texture2D m_erosionMaskTexture2;
        private DateTime m_erosionMaskDT2 = DateTime.MinValue;
        private HeightMap m_erosionMask2;

        //Working Mask
        private HeightMap m_workingMask;

        //Mask merge operation
        private Gaia.GaiaConstants.MaskMergeType m_maskMergeOperation = GaiaConstants.MaskMergeType.AssignMask2IfGreaterThan;

        private float m_powerExponent = 1.3f;
        private float m_contrast = 1.2f;
        private float m_multiplicand = 1.2f;
        private float m_add = 0.02f;
        private float m_subtract = 0.02f;
        List<GaiaWorldManager> m_erosionOperations = new List<GaiaWorldManager>();
        private int m_currentErosionOperation = 0;
        private GaiaConstants.CurvatureType m_curvatureType;
        private bool m_flipExport = false;
        private bool m_invertExport = false;
        private bool m_normalizeExport = false;
        private int m_denoiseRadius = 1;

        //Quantum related
        private float m_quantum = 0.02f;
        private List<float> m_quantumStartHeights = new List<float>();
        private List<AnimationCurve> m_quantumCurves = new List<AnimationCurve>();

        // Grow
        private int m_growthRadius = 1;

        //Shrink filter
        private int m_shrinkRadius = 1;

        //Noise related
        private Gaia.GaiaConstants.NoiseType m_noiseType = GaiaConstants.NoiseType.Perlin;
        private float m_noiseMaskSeed = 0;
        private int m_noiseMaskOctaves = 8;
        private float m_noiseMaskPersistence = 0.25f;
        private float m_noiseMaskFrequency = 1f;
        private float m_noiseMaskLacunarity = 1.5f;
        private float m_noiseZoom = 50f;
        private float m_noiseOffset = 0f;

        //Splat related
        private int m_targetSplatIdx = 0;
        private float[,,] m_splatBackup;

        //Basemap related
        private bool m_basemapFlip = false;
        private Texture2D m_basemapAlphaMask;

        //Terrain height
        private float m_terrainHeight = 1000f;

        //Height to set the terrain to
        private float m_terrainHeightToSet = 0.02f;

        //Aspect type
        private GaiaConstants.AspectType m_aspectType = GaiaConstants.AspectType.Aspect;

        void OnEnable()
        {
            if (m_noiseMaskSeed == 0)
            {
                m_noiseMaskSeed = UnityEngine.Random.Range(0, 65000);
            }

            if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null)
            {
                m_terrainHeight = Terrain.activeTerrain.terrainData.size.y;
            }

            if (m_quantumStartHeights.Count == 0)
            {
                m_quantumStartHeights.Add(0f);
                m_quantumCurves.Add(new AnimationCurve(new Keyframe(0f, 0.0f), new Keyframe(0.7f, 0.1f), new Keyframe(1.0f, 1.0f)));
                m_quantumStartHeights.Add(48f / m_terrainHeight);
                m_quantumCurves.Add(new AnimationCurve(new Keyframe(0f, 0.0f), new Keyframe(0.1f, 0.5f), new Keyframe(1.0f, 1.0f)));
                m_quantumStartHeights.Add(52f / m_terrainHeight);
                m_quantumCurves.Add(new AnimationCurve(new Keyframe(0f, 0.0f), new Keyframe(0.1f, 0.6f), new Keyframe(1.0f, 1.0f)));
                m_quantumStartHeights.Add(90f / m_terrainHeight);
                m_quantumCurves.Add(new AnimationCurve(new Keyframe(0f, 0.0f), new Keyframe(0.1f, 0.35f), new Keyframe(1.0f, 1.0f)));
                m_quantumStartHeights.Add(180f / m_terrainHeight);
                m_quantumCurves.Add(new AnimationCurve(new Keyframe(0f, 0.0f), new Keyframe(0.1f, 0.35f), new Keyframe(1.0f, 1.0f)));
            }
        }

        void OnGUI()
        {
            m_editorUtils.Initialize();
            if (!m_editorUtils.PositionChecked)
            {
                position = m_editorUtils.CheckPosition(position, maximized);
            }

            m_editorUtils.DrawIntro("Gaia Terrain Utilities", "Handy terrain utilities. Click here to see a tutorial.", "http://www.procedural-worlds.com/gaia/tutorials/terrain-utilities/");

            m_operationType = (GaiaConstants.TerrainOperationType)EditorGUILayout.EnumPopup(m_editorUtils.GetContent("Operation", m_tooltips), m_operationType);
            GUILayout.Space(3f);
            m_editorUtils.DrawBodyLine(m_editorUtils.GetContent(""), GUILayout.ExpandWidth(true));
            GUILayout.Space(-15f);

            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);

            m_editorUtils.DrawBody(m_editorUtils.GetContentText(m_operationType.ToString(), m_tooltips));
            GUILayout.Space(5f);

            switch (m_operationType)
            {
                case GaiaConstants.TerrainOperationType.ApplyMaskToSplatmap:
                    if (Terrain.activeTerrain != null)
                    {
                        int numSplats = Terrain.activeTerrain.terrainData.alphamapLayers;
                        if (numSplats > 0)
                        {
                            GUIContent [] splatNames = new GUIContent[numSplats];
                            int [] splatValues = new int[numSplats];
                            for (int idx = 0; idx < numSplats; idx++)
                            {
                                splatValues[idx] = idx;
                                splatNames[idx] = m_editorUtils.GetContent(GaiaSplatPrototype.GetGaiaSplatPrototypes(Terrain.activeTerrain)[idx].texture.name);
                            }
                            m_targetSplatIdx = EditorGUILayout.IntPopup(m_editorUtils.GetContent("Target Splat", m_tooltips), m_targetSplatIdx, splatNames, splatValues);
                        }
                    }
                    break;

                case GaiaConstants.TerrainOperationType.ExportNoiseMap:
                    m_noiseType = (GaiaConstants.NoiseType)EditorGUILayout.EnumPopup(m_editorUtils.GetContent("Noise Type", m_tooltips), m_noiseType);
                    m_noiseMaskSeed = EditorGUILayout.Slider(m_editorUtils.GetContent("Seed", m_tooltips), m_noiseMaskSeed, 0f, 65000f);
                    m_noiseMaskOctaves = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Octaves", m_tooltips), m_noiseMaskOctaves, 1, 12);
                    m_noiseMaskPersistence = EditorGUILayout.Slider(m_editorUtils.GetContent("Persistence", m_tooltips), m_noiseMaskPersistence, 0f, 1f);
                    m_noiseMaskFrequency = EditorGUILayout.Slider(m_editorUtils.GetContent("Frequency", m_tooltips), m_noiseMaskFrequency, 0f, 32f);
                    m_noiseMaskLacunarity = EditorGUILayout.Slider(m_editorUtils.GetContent("Lacunarity", m_tooltips), m_noiseMaskLacunarity, 1.5f, 3.5f);
                    m_noiseOffset = EditorGUILayout.Slider(m_editorUtils.GetContent("Offset", m_tooltips), m_noiseOffset, -1f, 1f);
                    m_noiseZoom = EditorGUILayout.Slider(m_editorUtils.GetContent("Zoom", m_tooltips), m_noiseZoom, 1f, 1000f);
                    break;

                case GaiaConstants.TerrainOperationType.ThermalFilter:
                    m_iterations = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Iterations", m_tooltips), m_iterations, 1, 400);
                    m_hardnessMapTexture = (Texture2D)EditorGUILayout.ObjectField(m_editorUtils.GetContent("Hardness Map", m_tooltips), m_hardnessMapTexture, typeof(Texture2D), false, GUILayout.Height(16f));
                    m_talusMin = EditorGUILayout.Slider(m_editorUtils.GetContent("Erode Above", m_tooltips), m_talusMin, 0f, 1f);
                    m_talusMax = EditorGUILayout.Slider(m_editorUtils.GetContent("Erode Below", m_tooltips), m_talusMax, 0f, 1f);
                    break;

                case GaiaConstants.TerrainOperationType.HydraulicFilter:
                    m_iterations = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Iterations", m_tooltips), m_iterations, 1, 400);
                    m_hardnessMapTexture = (Texture2D)EditorGUILayout.ObjectField(m_editorUtils.GetContent("Hardness Map", m_tooltips), m_hardnessMapTexture, typeof(Texture2D), false, GUILayout.Height(16f));
                    m_rainmapType = (GaiaConstants.ErosionRainType)EditorGUILayout.EnumPopup(m_editorUtils.GetContent("Rainmap Type", m_tooltips), m_rainmapType);
                    m_rainFreq = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Rain Frequency", m_tooltips), m_rainFreq, 1, 100);
                    m_disolveRate = EditorGUILayout.Slider(m_editorUtils.GetContent("Disolve Rate", m_tooltips), m_disolveRate, 0.001f, 1f);
                    break;

                case GaiaConstants.TerrainOperationType.PowerOfFilter:
                    m_powerExponent = EditorGUILayout.Slider(m_editorUtils.GetContent("Exponent", m_tooltips), m_powerExponent, 0.5f, 2f);
                    break;

                case GaiaConstants.TerrainOperationType.ContrastFilter:
                    m_contrast = EditorGUILayout.Slider(m_editorUtils.GetContent("Contrast", m_tooltips), m_contrast, 0.5f, 1.5f);
                    break;

                case GaiaConstants.TerrainOperationType.MultiplyTerrain:
                    m_multiplicand = EditorGUILayout.Slider(m_editorUtils.GetContent("Multiplier", m_tooltips), m_multiplicand, 0.2f, 2f);
                    break;

                case GaiaConstants.TerrainOperationType.AddToTerrain:
                    m_add = EditorGUILayout.Slider(m_editorUtils.GetContent("Amount To Add", m_tooltips), m_add * m_terrainHeight, 0f, m_terrainHeight) / m_terrainHeight;
                    break;

                case GaiaConstants.TerrainOperationType.SetTerrainToHeight:
                    m_terrainHeightToSet = EditorGUILayout.Slider(m_editorUtils.GetContent("Height To Set", m_tooltips), m_terrainHeightToSet * m_terrainHeight, 0f, m_terrainHeight) / m_terrainHeight;
                    break;

                case GaiaConstants.TerrainOperationType.SubtractFromTerrain:
                    m_subtract = EditorGUILayout.Slider(m_editorUtils.GetContent("Amount To Subtract", m_tooltips), m_subtract * m_terrainHeight, 0f, m_terrainHeight) / m_terrainHeight;
                    break;

                case GaiaConstants.TerrainOperationType.QuantizeFilter:
                    m_quantum = EditorGUILayout.Slider(m_editorUtils.GetContent("Quantum", m_tooltips), m_quantum * m_terrainHeight, 0.00001f, m_terrainHeight) / m_terrainHeight;
                    //m_quantumCurve = EditorGUILayout.CurveField(m_editorUtils.GetContent("Quantum Curve", m_tooltips), m_quantumCurve);
                    break;

                case GaiaConstants.TerrainOperationType.QuantizeCurvesFilter:

                    int terraceFilterCount = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Terraces", m_tooltips), m_quantumStartHeights.Count, 1, 15);

                    //remove excess 
                    if (m_quantumStartHeights.Count > terraceFilterCount)
                    {
                        while (m_quantumStartHeights.Count > terraceFilterCount)
                        {
                            m_quantumStartHeights.RemoveAt(m_quantumStartHeights.Count-1);
                            m_quantumCurves.RemoveAt(m_quantumCurves.Count-1);
                        }
                    }

                    //add new
                    if (terraceFilterCount > m_quantumStartHeights.Count)
                    {
                        float lastHeight = m_quantumStartHeights[m_quantumStartHeights.Count - 1];
                        while (terraceFilterCount > m_quantumStartHeights.Count)
                        {
                            lastHeight += 30f / m_terrainHeight;
                            m_quantumStartHeights.Add(lastHeight);
                            m_quantumCurves.Add(new AnimationCurve(new Keyframe(0f, 0.0f), new Keyframe(0.7f, 0.5f), new Keyframe(1.0f, 1.0f)));
                        }
                    }

                    //now allow editing
                    EditorGUI.indentLevel++;
                    for (int filterIdx = 0; filterIdx < m_quantumStartHeights.Count; filterIdx++)
                    {
                        EditorGUILayout.LabelField("Terrace " + (filterIdx + 1));
                        EditorGUI.indentLevel++;
                        float startHeight = EditorGUILayout.Slider(m_editorUtils.GetContent("Start Height", m_tooltips), m_quantumStartHeights[filterIdx] * m_terrainHeight, 0f, m_terrainHeight) / m_terrainHeight;
                        if (filterIdx == 0)
                        {
                            startHeight = 0f;
                        }
                        else
                        {
                            if (startHeight < m_quantumStartHeights[filterIdx - 1])
                            {
                                startHeight = m_quantumStartHeights[filterIdx - 1] + (1f / m_terrainHeight);
                            }
                        }
                        m_quantumStartHeights[filterIdx] = startHeight;
                        m_quantumCurves[filterIdx] = EditorGUILayout.CurveField(m_editorUtils.GetContent("Filter", m_tooltips), m_quantumCurves[filterIdx]);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                    break;

                case GaiaConstants.TerrainOperationType.ExportBaseMap:
                    m_basemapFlip = EditorGUILayout.Toggle(m_editorUtils.GetContent("Flip Export", m_tooltips), m_basemapFlip);
                    m_basemapAlphaMask = (Texture2D)EditorGUILayout.ObjectField(m_editorUtils.GetContent("Bake Mask to Chnl A", m_tooltips), m_basemapAlphaMask, typeof(Texture2D), false, GUILayout.Height(16f));
                    break;

                case GaiaConstants.TerrainOperationType.ExportCurvatureMap:
                    m_curvatureType = (GaiaConstants.CurvatureType)EditorGUILayout.EnumPopup(m_editorUtils.GetContent("Curvature Type", m_tooltips), m_curvatureType);
                    m_flipExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Flip Export", m_tooltips), m_flipExport);
                    m_normalizeExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Normalize Export", m_tooltips), m_normalizeExport);
                    m_invertExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Invert Export", m_tooltips), m_invertExport);
                    break;

                case GaiaConstants.TerrainOperationType.ExportAspectMap:
                    m_aspectType = (GaiaConstants.AspectType)EditorGUILayout.EnumPopup(m_editorUtils.GetContent("Aspect Type", m_tooltips), m_aspectType);
                    m_flipExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Flip Export", m_tooltips), m_flipExport);
                    m_normalizeExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Normalize Export", m_tooltips), m_normalizeExport);
                    m_invertExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Invert Export", m_tooltips), m_invertExport);
                    break;

                case GaiaConstants.TerrainOperationType.ExportHeightMap:
                case GaiaConstants.TerrainOperationType.ExportNormalMap:
                case GaiaConstants.TerrainOperationType.ExportSlopeMap:
                    m_flipExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Flip Export", m_tooltips), m_flipExport);
                    m_normalizeExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Normalize Export", m_tooltips), m_normalizeExport);
                    m_invertExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Invert Export", m_tooltips), m_invertExport);
                    break;

                case GaiaConstants.TerrainOperationType.ExportFlowMap:
                    m_iterations = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Iterations", m_tooltips), m_iterations, 1, 100);
                    m_flipExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Flip Export", m_tooltips), m_flipExport);
                    m_normalizeExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Normalize Export", m_tooltips), m_normalizeExport);
                    m_invertExport = EditorGUILayout.Toggle(m_editorUtils.GetContent("Invert Export", m_tooltips), m_invertExport);
                    break;

                case GaiaConstants.TerrainOperationType.DeNoiseFilter:
                    break;

                case GaiaConstants.TerrainOperationType.GrowFeaturesFilter:
                    m_growthRadius = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Radius", m_tooltips), m_growthRadius, 1, 10);
                    break;

                case GaiaConstants.TerrainOperationType.ShrinkFeaturesFilter:
                    m_shrinkRadius = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Radius", m_tooltips), m_shrinkRadius, 1, 10);
                    break;
            }

            if (m_operationType != GaiaConstants.TerrainOperationType.ExportCurvatureMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportNormalMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportSlopeMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportAspectMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportHeightMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportBaseMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportNoiseMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportFlowMap)
            {
                m_useErosionMask1 = EditorGUILayout.Toggle(m_editorUtils.GetContent("Use Mask 1", m_tooltips), m_useErosionMask1);
                if (m_useErosionMask1)
                {
                    EditorGUI.indentLevel++;
                    Texture2D newMaskTex1 = (Texture2D)EditorGUILayout.ObjectField(m_editorUtils.GetContent("Mask 1", m_tooltips), m_erosionMaskTexture1, typeof(Texture2D), false, GUILayout.Height(16f));
                    if (newMaskTex1 != m_erosionMaskTexture1)
                    {
                        m_erosionMaskTexture1 = newMaskTex1;
                        m_erosionMask1 = null;
                    }
                    if (m_erosionMaskTexture1 != null)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.BeginHorizontal();
                        m_erosionMaskStrength1 = EditorGUILayout.Slider(m_editorUtils.GetContent("Strength", m_tooltips), m_erosionMaskStrength1, 0f, 5f);
                        EditorGUILayout.BeginVertical(GUILayout.Width(21f));
                        GUILayout.Space(2f);
                        if (GUILayout.Button("R", GUILayout.Width(20f), GUILayout.Height(15f)))
                        {
                            m_erosionMaskStrength1 = 1f;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        m_erosionMaskOffset1 = EditorGUILayout.Slider(m_editorUtils.GetContent("Offset", m_tooltips), m_erosionMaskOffset1, -1f, 1f);
                        EditorGUILayout.BeginVertical(GUILayout.Width(21f));
                        GUILayout.Space(2f);
                        if (GUILayout.Button("R", GUILayout.Width(20f), GUILayout.Height(15f)))
                        {
                            m_erosionMaskOffset1 = 0f;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        m_erosionMaskPowerOf1 = EditorGUILayout.Slider(m_editorUtils.GetContent("Power", m_tooltips), m_erosionMaskPowerOf1, 0.001f, 5f);
                        EditorGUILayout.BeginVertical(GUILayout.Width(21f));
                        GUILayout.Space(2f);
                        if (GUILayout.Button("R", GUILayout.Width(20f), GUILayout.Height(15f)))
                        {
                            m_erosionMaskPowerOf1 = 1f;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        m_erosionMaskContrast1 = EditorGUILayout.Slider(m_editorUtils.GetContent("Contrast", m_tooltips), m_erosionMaskContrast1, 0.001f, 5f);
                        EditorGUILayout.BeginVertical(GUILayout.Width(21f));
                        GUILayout.Space(2f);
                        if (GUILayout.Button("R", GUILayout.Width(20f), GUILayout.Height(15f)))
                        {
                            m_erosionMaskContrast1 = 1f;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        m_quantizeMask1 = EditorGUILayout.Slider(m_editorUtils.GetContent("Quantize", m_tooltips), m_quantizeMask1, 0f, 1f);
                        EditorGUILayout.BeginVertical(GUILayout.Width(21f));
                        GUILayout.Space(2f);
                        if (GUILayout.Button("R", GUILayout.Width(20f), GUILayout.Height(15f)))
                        {
                            m_quantizeMask1 = 0f;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        m_blurMask1 = EditorGUILayout.IntSlider(m_editorUtils.GetContent("Blur", m_tooltips), m_blurMask1, 0, 20);
                        EditorGUILayout.BeginVertical(GUILayout.Width(21f));
                        GUILayout.Space(2f);
                        if (GUILayout.Button("R", GUILayout.Width(20f), GUILayout.Height(15f)))
                        {
                            m_blurMask1 = 0;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();

                        m_flipErosionMask1 = EditorGUILayout.Toggle(m_editorUtils.GetContent("Flip", m_tooltips), m_flipErosionMask1);
                        m_invertErosionMask1 = EditorGUILayout.Toggle(m_editorUtils.GetContent("Invert", m_tooltips), m_invertErosionMask1);
                        m_normalizeErosionMask1 = EditorGUILayout.Toggle(m_editorUtils.GetContent("Normalize", m_tooltips), m_normalizeErosionMask1);
                        EditorGUI.indentLevel--;

                    }
                    EditorGUI.indentLevel--;
                }

                if (m_useErosionMask1 && m_erosionMaskTexture1 != null)
                {
                    m_useErosionMask2 = EditorGUILayout.Toggle(m_editorUtils.GetContent("Use Mask 2", m_tooltips), m_useErosionMask2);
                    if (m_useErosionMask2)
                    {
                        EditorGUI.indentLevel++;
                        Texture2D newMaskTex2 = (Texture2D)EditorGUILayout.ObjectField(m_editorUtils.GetContent("Mask 2", m_tooltips), m_erosionMaskTexture2, typeof(Texture2D), false, GUILayout.Height(16f));
                        if (newMaskTex2 != m_erosionMaskTexture2)
                        {
                            m_erosionMaskTexture2 = newMaskTex2;
                            m_erosionMask2 = null;
                        }

                        if (m_erosionMaskTexture2 != null)
                        {
                            EditorGUI.indentLevel++;
                            m_erosionMaskStrength2 = EditorGUILayout.Slider(m_editorUtils.GetContent("Strength", m_tooltips), m_erosionMaskStrength2, 0f, 5f);
                            m_flipErosionMask2 = EditorGUILayout.Toggle(m_editorUtils.GetContent("Flip", m_tooltips), m_flipErosionMask2);
                            m_invertErosionMask2 = EditorGUILayout.Toggle(m_editorUtils.GetContent("Invert", m_tooltips), m_invertErosionMask2);
                            m_normalizeErosionMask2 = EditorGUILayout.Toggle(m_editorUtils.GetContent("Normalize", m_tooltips), m_normalizeErosionMask2);
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.indentLevel--;
                    }
                }

                if (m_erosionMaskTexture1 != null && m_useErosionMask1 && m_erosionMaskTexture2 != null && m_useErosionMask2)
                {
                    m_maskMergeOperation = (GaiaConstants.MaskMergeType)EditorGUILayout.EnumPopup(m_editorUtils.GetContent("Mask Operation", m_tooltips), m_maskMergeOperation);
                }
            }

            //End scroll
            GUILayout.EndScrollView();

            m_editorUtils.DrawBodyLine(m_editorUtils.GetContent(""), GUILayout.ExpandWidth(true));
            GUILayout.Space(-12f);

            EditorGUI.indentLevel++;

            if (m_editorUtils.DrawButton((m_editorUtils.GetContent("Perform Operation", m_tooltips))))
            {
                switch (m_operationType)
                {
                    case GaiaConstants.TerrainOperationType.AddToTerrain:
                    case GaiaConstants.TerrainOperationType.ContrastFilter:
                        ErodeTerrain();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportAspectMap:
                        ExportAspectMap();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportBaseMap:
                        ExportBaseMap();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportCurvatureMap:
                        ExportCuvaturemap();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportFlowMap:
                        ExportFlowmap();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportHeightMap:
                        ExportHeightmap();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportNormalMap:
                        ExportNormalmap();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportNoiseMap:
                        ExportNoisemap();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportSlopeMap:
                        ExportSlopemap();
                        break;
                    case GaiaConstants.TerrainOperationType.ExportMasks:
                        ExportMask();
                        break;
                    case GaiaConstants.TerrainOperationType.ApplyMaskToSplatmap:
                        ApplyMaskToSplatMap();
                        break;
                    case GaiaConstants.TerrainOperationType.HydraulicFilter:
                    case GaiaConstants.TerrainOperationType.MultiplyTerrain:
                    case GaiaConstants.TerrainOperationType.PowerOfFilter:
                    case GaiaConstants.TerrainOperationType.QuantizeFilter:
                    case GaiaConstants.TerrainOperationType.QuantizeCurvesFilter:
                    case GaiaConstants.TerrainOperationType.SubtractFromTerrain:
                    case GaiaConstants.TerrainOperationType.ThermalFilter:
                    case GaiaConstants.TerrainOperationType.DeNoiseFilter:
                    case GaiaConstants.TerrainOperationType.GrowFeaturesFilter:
                    case GaiaConstants.TerrainOperationType.SetTerrainToHeight:
                    case GaiaConstants.TerrainOperationType.ShrinkFeaturesFilter:
                        ErodeTerrain();
                        break;
                }
            }

            GUILayout.Space(5f);

            if (m_operationType == GaiaConstants.TerrainOperationType.ApplyMaskToSplatmap)
            {
                if (m_splatBackup != null)
                {
                    EditorGUI.indentLevel++;
                    if (m_editorUtils.DrawButton(m_editorUtils.GetContent("Undo", m_tooltips)))
                    {
                        UndoApplyMaskToSplatMap();
                    }
                    EditorGUI.indentLevel--;
                }
            }

            if (m_operationType != GaiaConstants.TerrainOperationType.ExportCurvatureMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportBaseMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportNormalMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportSlopeMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportMasks &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportAspectMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportNoiseMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportHeightMap &&
                m_operationType != GaiaConstants.TerrainOperationType.ApplyMaskToSplatmap &&
                m_operationType != GaiaConstants.TerrainOperationType.ExportFlowMap)
            {
                if (m_erosionOperations.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    if (m_currentErosionOperation == 0)
                    {
                        GUI.enabled = false;
                        if (m_editorUtils.DrawButton(m_editorUtils.GetContent(string.Format("Back to Step {0} of {1}",
                            m_currentErosionOperation, m_erosionOperations.Count - 1))))
                        {
                        }

                        GUI.enabled = true;
                    }
                    else
                    {
                        if (m_currentErosionOperation == 1)
                        {
                            if (m_editorUtils.DrawButton(m_editorUtils.GetContent("Back to Start", m_tooltips)))
                            {
                                m_currentErosionOperation--;
                                m_erosionOperations[m_currentErosionOperation].SaveToWorld(true);
                            }
                        }
                        else
                        {
                            if (m_editorUtils.DrawButton(m_editorUtils.GetContent(
                                string.Format("Back to Step {0} of {1}", m_currentErosionOperation - 1,
                                    m_erosionOperations.Count - 1))))
                            {
                                m_currentErosionOperation--;
                                m_erosionOperations[m_currentErosionOperation].SaveToWorld(true);
                            }
                        }
                    }

                    if (m_currentErosionOperation == m_erosionOperations.Count - 1)
                    {
                        GUI.enabled = false;
                        if (m_editorUtils.DrawButton(m_editorUtils.GetContent(string.Format("Fwd to Step {0} of {1}",
                            m_currentErosionOperation, m_erosionOperations.Count - 1))))
                        {
                        }

                        GUI.enabled = true;
                    }
                    else
                    {
                        if (m_editorUtils.DrawButton(m_editorUtils.GetContent(string.Format("Fwd to Step {0} of {1}",
                            m_currentErosionOperation + 1, m_erosionOperations.Count - 1))))
                        {
                            m_currentErosionOperation++;
                            m_erosionOperations[m_currentErosionOperation].SaveToWorld(true);
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Export mask
        /// </summary>
        private void ExportMask()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (Terrain.activeTerrain != null)
            {
                path = Path.Combine(path, GaiaCommon1.Utils.FixFileName(Terrain.activeTerrain.name + "_Mask"));
            }
            else
            {
                path = Path.Combine(path, GaiaCommon1.Utils.FixFileName("Mask"));
            }

            SetupMask();

            if (m_workingMask == null)
            {
                Debug.LogWarning("No masks supplied");
                return;
            }

            GaiaUtils.CompressToMultiChannelFileImage(path, m_workingMask, m_workingMask, m_workingMask, m_workingMask, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
        }

        /// <summary>
        /// Setup masks
        /// </summary>
        private void SetupMask()
        {
            //Wipe working mask
            m_workingMask = null;

            //Grab the mask if supplied
            HeightMap workingMask1 = null;
            if (m_erosionMaskTexture1 == null)
            {
                m_erosionMask1 = null;
            }
            else
            {
                DateTime lastEditDT = File.GetLastWriteTime(AssetDatabase.GetAssetPath(m_erosionMaskTexture1));

                //Only load if necessary
                if (m_erosionMask1 == null || lastEditDT != m_erosionMaskDT1)
                {
                    //Save it
                    m_erosionMaskDT1 = lastEditDT;

                    //Make sure its readable
                    GaiaUtils.MakeTextureReadable(m_erosionMaskTexture1);

                    //Make sure its not compressed
                    GaiaUtils.MakeTextureUncompressed(m_erosionMaskTexture1);

                    //Load the image
                    int width = m_erosionMaskTexture1.width;
                    int height = m_erosionMaskTexture1.height;
                    m_erosionMask1 = new HeightMap(width, height);
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            m_erosionMask1[x, z] = m_erosionMaskTexture1.GetPixel(x, z).grayscale;
                        }
                    }
                }

                //Perform any mask operations
                if (m_useErosionMask1)
                {
                    workingMask1 = m_erosionMask1.Duplicate();
                    if (m_useErosionMask1 && workingMask1 != null)
                    {
                        if (!GaiaCommon1.Utils.Math_ApproximatelyEqual(m_erosionMaskStrength1, 1f))
                        {
                            workingMask1.MultiplyClamped(m_erosionMaskStrength1, 0f, 1f);
                        }
                        if (!GaiaCommon1.Utils.Math_ApproximatelyEqual(m_erosionMaskOffset1, 0f))
                        {
                            workingMask1.AddClamped(m_erosionMaskOffset1, 0f, 1f);
                        }
                        if (!GaiaCommon1.Utils.Math_ApproximatelyEqual(m_erosionMaskPowerOf1, 1f))
                        {
                            workingMask1.Power(m_erosionMaskPowerOf1);
                        }
                        if (!GaiaCommon1.Utils.Math_ApproximatelyEqual(m_erosionMaskContrast1, 1f))
                        {
                            workingMask1.Contrast(m_erosionMaskContrast1);
                        }
                        if (!GaiaCommon1.Utils.Math_ApproximatelyEqual(m_quantizeMask1, 0f))
                        {
                            workingMask1.Quantize(m_quantizeMask1);
                        }
                        if (m_blurMask1 != 0)
                        {
                            workingMask1.Smooth(m_blurMask1);
                        }

                        if (m_flipErosionMask1)
                        {
                            workingMask1.Flip();
                        }

                        //Ensure correct 0..1f ranges
                        workingMask1.AddClamped(0f, 0f, 1f);

                        if (m_invertErosionMask1)
                        {
                            workingMask1.Invert();
                        }
                        if (m_normalizeErosionMask1)
                        {
                            workingMask1.Normalise();
                        }
                    }
                }
            }

            //Grab the mask if supplied
            HeightMap workingMask2 = null;
            if (m_erosionMaskTexture2 == null)
            {
                m_erosionMask2 = null;
            }
            else
            {
                DateTime lastEditDT = File.GetLastWriteTime(AssetDatabase.GetAssetPath(m_erosionMaskTexture2));

                //Only load if necessary
                if (m_erosionMask2 == null || lastEditDT != m_erosionMaskDT2)
                {
                    //Save it
                    m_erosionMaskDT2 = lastEditDT;

                    //Make sure its readable
                    GaiaUtils.MakeTextureReadable(m_erosionMaskTexture2);

                    //Make sure its not compressed
                    GaiaUtils.MakeTextureUncompressed(m_erosionMaskTexture2);

                    //Load the image
                    int width = m_erosionMaskTexture2.width;
                    int height = m_erosionMaskTexture2.height;
                    m_erosionMask2 = new HeightMap(width, height);
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            m_erosionMask2[x, z] = m_erosionMaskTexture2.GetPixel(x, z).grayscale;
                        }
                    }
                }

                //Perform any mask operations
                if (m_useErosionMask2)
                {
                    workingMask2 = m_erosionMask2.Duplicate();
                    if (m_useErosionMask2 && workingMask2 != null)
                    {
                        if (!GaiaCommon1.Utils.Math_ApproximatelyEqual(m_erosionMaskStrength2, 1f))
                        {
                            workingMask2.Multiply(m_erosionMaskStrength2);
                        }
                        if (m_flipErosionMask2)
                        {
                            workingMask2.Flip();
                        }

                        //Ensure correct 0..1f ranges
                        workingMask2.AddClamped(0f, 0f, 1f);

                        if (m_invertErosionMask2)
                        {
                            workingMask2.Invert();
                        }
                        if (m_normalizeErosionMask2)
                        {
                            workingMask2.Normalise();
                        }
                    }
                }
            }

            //Now set up the actual working mask
            if (workingMask1 == null && workingMask2 == null)
            {
                m_workingMask = null;
                return;
            }

            if (workingMask1 != null && workingMask2 == null)
            {
                m_workingMask = workingMask1;
                return;
            }

            if (workingMask1 == null && workingMask2 != null)
            {
                m_workingMask = workingMask2;
                return;
            }

            //Merge them - work off the resolution of mask 1
            switch (m_maskMergeOperation)
            {
                case GaiaConstants.MaskMergeType.AssignMask2IfGreaterThan:
                    workingMask1.Copy(workingMask2, HeightMap.CopyType.CopyIfGreaterThan);
                    break;
                case GaiaConstants.MaskMergeType.AssignMask2IfLessThan:
                    workingMask1.Copy(workingMask2, HeightMap.CopyType.CopyIfLessThan);
                    break;
                case GaiaConstants.MaskMergeType.AddMask2:
                    workingMask1.AddClamped(workingMask2, 0f, 1f);
                    break;
                case GaiaConstants.MaskMergeType.MultiplyByMask2:
                    workingMask1.MultiplyClamped(workingMask2, 0f, 1f);
                    break;
                case GaiaConstants.MaskMergeType.SubtractMask2:
                    workingMask1.SubtractClamped(workingMask2, 0f, 1f);
                    break;
            }

            m_workingMask = workingMask1;
        }

        /// <summary>
        /// Run an operation that modifies the terrain
        /// </summary>
        private void ErodeTerrain()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //Store initial state of terrain
                if (m_erosionOperations.Count == 0)
                {
                    GaiaWorldManager undo = new GaiaWorldManager(Terrain.activeTerrains);
                    undo.LoadFromWorld();
                    m_erosionOperations.Add(undo);
                }
                m_currentErosionOperation++;
                while (m_erosionOperations.Count > m_currentErosionOperation)
                {
                    m_erosionOperations.RemoveAt(m_erosionOperations.Count - 1);
                }

                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Setup mask
                SetupMask();

                //Now iterate and do the work
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        switch (m_operationType)
                        {
                            case GaiaConstants.TerrainOperationType.ThermalFilter:
                                if (m_hardnessMapTexture != null)
                                {
                                    m_hardnessMap = new UnityHeightMap(m_hardnessMapTexture);
                                }
                                else
                                {
                                    m_hardnessMap = new HeightMap(mgr.HeightMapTerrainArray[0, 0].Width(), mgr.HeightMapTerrainArray[0, 0].Depth());
                                }
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().ErodeThermal(m_iterations, m_talusMin, m_talusMax, m_hardnessMap).MultiplyClamped(1f, 0f, 1f);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].ErodeThermal(m_iterations, m_talusMin, m_talusMax, m_hardnessMap).MultiplyClamped(1f, 0f, 1f);
                                }
                                break;

                            case GaiaConstants.TerrainOperationType.HydraulicFilter:
                                HeightMap sedimentMap = new HeightMap(mgr.HeightMapTerrainArray[0, 0].Width(), mgr.HeightMapTerrainArray[0, 0].Depth());
                                if (m_hardnessMapTexture != null)
                                {
                                    m_hardnessMap = new UnityHeightMap(m_hardnessMapTexture);
                                }
                                else
                                {
                                    m_hardnessMap = new HeightMap(mgr.HeightMapTerrainArray[0, 0].Width(), mgr.HeightMapTerrainArray[0, 0].Depth());
                                }
                                switch (m_rainmapType)
                                {
                                    case GaiaConstants.ErosionRainType.Constant:
                                        m_rainMap = mgr.HeightMapTerrainArray[0, 0].Duplicate();
                                        m_rainMap.SetHeight(1f);
                                        break;
                                    case GaiaConstants.ErosionRainType.ErodePeaks:
                                        m_rainMap = mgr.HeightMapTerrainArray[0, 0].Duplicate().Normalise();
                                        break;
                                    case GaiaConstants.ErosionRainType.ErodeValleys:
                                        m_rainMap = mgr.HeightMapTerrainArray[0, 0].Duplicate().Normalise().Invert();
                                        break;
                                    case GaiaConstants.ErosionRainType.ErodeSlopes:
                                        m_rainMap = mgr.HeightMapTerrainArray[0, 0].SlopeMap();
                                        break;
                                }
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().ErodeHydraulic(m_iterations, m_hardnessMap, m_rainMap, m_rainFreq, m_disolveRate, ref sedimentMap).MultiplyClamped(1f, 0f, 1f);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].ErodeHydraulic(m_iterations, m_hardnessMap, m_rainMap, m_rainFreq, m_disolveRate, ref sedimentMap).MultiplyClamped(1f, 0f, 1f);
                                }

                                string fname = path;
                                fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_Sed"));
                                sedimentMap.Normalise();
                                GaiaUtils.CompressToMultiChannelFileImage(fname, sedimentMap, sedimentMap, sedimentMap, sedimentMap, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
                                AssetDatabase.Refresh();

                                break;

                            case GaiaConstants.TerrainOperationType.PowerOfFilter:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().Power(m_powerExponent).MultiplyClamped(1f, 0f, 1f);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].Power(m_powerExponent).MultiplyClamped(1f, 0f, 1f);
                                }
                                break;
                            case GaiaConstants.TerrainOperationType.ContrastFilter:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().Contrast(m_contrast).MultiplyClamped(1f, 0f, 1f);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].Contrast(m_contrast).MultiplyClamped(1f, 0f, 1f);
                                }

                                break;
                            case GaiaConstants.TerrainOperationType.MultiplyTerrain:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().MultiplyClamped(m_multiplicand, 0f, 1f);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].MultiplyClamped(m_multiplicand, 0f, 1f);
                                }
                                break;
                            case GaiaConstants.TerrainOperationType.AddToTerrain:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().AddClamped(m_add, 0f, 1f);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].AddClamped(m_add, 0f, 1f);
                                }
                                break;
                            case GaiaConstants.TerrainOperationType.SetTerrainToHeight:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().SetHeight(m_terrainHeightToSet);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].SetHeight(m_terrainHeightToSet);
                                }
                                break;
                            case GaiaConstants.TerrainOperationType.SubtractFromTerrain:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().SubtractClamped(m_subtract, 0f, 1f);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].SubtractClamped(m_subtract, 0f, 1f);
                                }
                                break;
                            case GaiaConstants.TerrainOperationType.QuantizeFilter:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().Quantize(m_quantum);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].Quantize(m_quantum);
                                }
                                break;

                            case GaiaConstants.TerrainOperationType.QuantizeCurvesFilter:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().Quantize(m_quantumStartHeights.ToArray(), m_quantumCurves.ToArray());
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].Quantize(m_quantumStartHeights.ToArray(), m_quantumCurves.ToArray());
                                }
                                break;

                            case GaiaConstants.TerrainOperationType.DeNoiseFilter:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().DeNoise(m_denoiseRadius);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].DeNoise(m_denoiseRadius);
                                }
                                break;
                            case GaiaConstants.TerrainOperationType.GrowFeaturesFilter:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().GrowEdges(m_growthRadius);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].GrowEdges(m_growthRadius);
                                }
                                break;
                            case GaiaConstants.TerrainOperationType.ShrinkFeaturesFilter:
                                if (m_workingMask != null)
                                {
                                    HeightMap hmNewValues = hmArray[tileX, tileZ].Duplicate().ShrinkEdges(m_shrinkRadius);
                                    hmArray[tileX, tileZ].Lerp(hmNewValues, m_workingMask);
                                }
                                else
                                {
                                    hmArray[tileX, tileZ].ShrinkEdges(m_shrinkRadius);
                                }
                                break;
//                            case GaiaConstants.TerrainOperationType.ZTestFilter:
//                                float[,] kernel = {{0f, -1f, 0f}, {-1f, 4f, -1f}, {0f, -1f, 0f}};
//                                //float[,] kernel = { { 1f, 1f, 1f }, { 1f, 1f, 1f }, { 1f, 1f, 1f } };
//                                //float[,] kernel = { { 1f, 2f, 1f }, { 2f, 4f, 2f }, { 1f, 2f, 1f } };
//                                hmArray[tileX, tileZ].Convolve(kernel);
//                                break;
                        }
                    }
                }

                mgr.SaveToWorld();
                m_erosionOperations.Add(mgr);
            }
            mgr.SaveToWorld();
        }

        /// <summary>
        /// Export terrain heightmaps
        /// </summary>
        private void ExportHeightmap()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //path = Path.Combine(path, PWCommon.Utils.FixFileName());

                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Now iterate and do the work
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        HeightMap exportHm = hmArray[tileX, tileZ].Duplicate();
                        if (m_flipExport)
                        {
                            exportHm.Flip();
                        }
                        if (m_invertExport)
                        {
                            exportHm.Invert();
                        }
                        if (m_normalizeExport)
                        {
                            exportHm.Normalise();
                        }
                        string fname = path;
                        fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_HeightMap"));
                        GaiaUtils.CompressToMultiChannelFileImage(fname, exportHm, exportHm, exportHm, exportHm, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
                    }
                }
            }
        }

        /// <summary>
        /// Export terrain flowmaps
        /// </summary>
        private void ExportFlowmap()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //path = Path.Combine(path, PWCommon.Utils.FixFileName());

                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Now iterate and do the work
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        string fname = path;
                        fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_FlowMap"));
                        HeightMap exportMap = hmArray[tileX, tileZ].FlowMap(m_iterations);
                        exportMap.Flip();
                        if (m_flipExport)
                        {
                            exportMap.Flip();
                        }
                        if (m_invertExport)
                        {
                            exportMap.Invert();
                        }
                        if (m_normalizeExport)
                        {
                            exportMap.Normalise();
                        }
                        GaiaUtils.CompressToMultiChannelFileImage(fname, exportMap, exportMap, exportMap, exportMap, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
                    }
                }
            }
        }

        /// <summary>
        /// Export terrain curvature maps
        /// </summary>
        private void ExportCuvaturemap()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Now iterate and do the work
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        string fname = path;
                        fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_CurvatureMap"));
                        HeightMap exportMap = hmArray[tileX, tileZ].CurvatureMap(m_curvatureType);
                        if (m_flipExport)
                        {
                            exportMap.Flip();
                        }
                        if (m_invertExport)
                        {
                            exportMap.Invert();
                        }
                        if (m_normalizeExport)
                        {
                            exportMap.Normalise();
                        }
                        GaiaUtils.CompressToMultiChannelFileImage(fname, exportMap, exportMap, exportMap, exportMap, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
                    }
                }
            }
        }

        /// <summary>
        /// Export terrain slope maps
        /// </summary>
        private void ExportSlopemap()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Now iterate and do the work
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        string fname = path;
                        fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_SlopeMap"));
                        HeightMap exportMap = hmArray[tileX, tileZ].SlopeMap();
                        if (m_flipExport)
                        {
                            exportMap.Flip();
                        }
                        if (m_invertExport)
                        {
                            exportMap.Invert();
                        }
                        if (m_normalizeExport)
                        {
                            exportMap.Normalise();
                        }
                        GaiaUtils.CompressToMultiChannelFileImage(fname, exportMap, exportMap, exportMap, exportMap, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
                    }
                }
            }
        }

        /// <summary>
        /// Export terrain aspect maps
        /// </summary>
        private void ExportAspectMap()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Now iterate and do the work
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        string fname = path;
                        fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_AspectMap"));
                        HeightMap exportMap = hmArray[tileX, tileZ].Aspect(m_aspectType);
                        if (m_flipExport)
                        {
                            exportMap.Flip();
                        }
                        if (m_invertExport)
                        {
                            exportMap.Invert();
                        }
                        if (m_normalizeExport)
                        {
                            exportMap.Normalise();
                        }
                        GaiaUtils.CompressToMultiChannelFileImage(fname, exportMap, exportMap, exportMap, exportMap, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
                    }
                }
            }
        }

        /// <summary>
        /// Export terrain normal maps
        /// </summary>
        private void ExportNormalmap()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Now iterate and do the work
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        string fname = path;
                        fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_NormalMap"));
                        Texture2D nrmTexture = hmArray[tileX, tileZ].CalculateNormals();
                        byte[] exrBytes = nrmTexture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
                        GaiaCommon1.Utils.WriteAllBytes(fname+".exr", exrBytes);
                        MonoBehaviour.DestroyImmediate(nrmTexture);

                        AssetDatabase.Refresh();
                        Texture2D normalTex = GaiaCommon1.AssetUtils.GetAsset(fname, typeof(Texture2D)) as Texture2D;
                        GaiaUtils.MakeTextureNormal(normalTex);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate and export some noise
        /// </summary>
        private void ExportNoisemap()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Get our fractal generator
                Gaia.FractalGenerator noiseGenerator = null;
                switch (m_noiseType)
                {
                    case GaiaConstants.NoiseType.None:
                    case GaiaConstants.NoiseType.Perlin:
                        noiseGenerator = new FractalGenerator(m_noiseMaskFrequency, m_noiseMaskLacunarity, m_noiseMaskOctaves, m_noiseMaskPersistence, m_noiseMaskSeed, FractalGenerator.Fractals.Perlin);
                        break;
                    case GaiaConstants.NoiseType.Billow:
                        noiseGenerator = new FractalGenerator(m_noiseMaskFrequency, m_noiseMaskLacunarity, m_noiseMaskOctaves, m_noiseMaskPersistence, m_noiseMaskSeed, FractalGenerator.Fractals.Billow);
                        break;
                    case GaiaConstants.NoiseType.Ridged:
                        noiseGenerator = new FractalGenerator(m_noiseMaskFrequency, m_noiseMaskLacunarity, m_noiseMaskOctaves, m_noiseMaskPersistence, m_noiseMaskSeed, FractalGenerator.Fractals.RidgeMulti);
                        break;
                }

                noiseGenerator.YOffset = m_noiseOffset;

                //Now iterate and do the work
                float zoom = 1f / m_noiseZoom;
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        string fname = path;
                        fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_NoiseMap"));
                        HeightMap exportMap = hmArray[tileX, tileZ].Duplicate();

                        int width = exportMap.Width();
                        int height = exportMap.Depth();

                        for (int x = 0; x < width; x++)
                        {
                            for (int z = 0; z < height; z++)
                            {
                                exportMap[x, z] = noiseGenerator.GetNormalisedValue((tileX * width + x) * zoom, (tileZ * height + z) * zoom);
                            }
                        }
                        GaiaUtils.CompressToMultiChannelFileImage(fname, exportMap, exportMap, exportMap, exportMap, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Export terrain base maps
        /// </summary>
        private void ExportBaseMap()
        {
            if (Terrain.activeTerrain == null)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have a valid terrain in your scene!!", "OK");
                return;
            }

            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            UnityHeightMap alphaMask = null;
            if (m_basemapAlphaMask != null)
            {
                alphaMask = new UnityHeightMap(m_basemapAlphaMask);
                if (m_basemapFlip)
                {
                    alphaMask.Flip();
                }
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Now iterate and do the work
                //float zoom = 1f / m_noiseZoom;
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        string fname = path;
                        fname = Path.Combine(path, GaiaCommon1.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_BaseMap"));

                        Texture2D[] terrainSplats = mgr.PhysicalTerrainArray[tileX, tileZ].terrainData.alphamapTextures;

                        GaiaSplatPrototype[] terrainSplatPrototypes = GaiaSplatPrototype.GetGaiaSplatPrototypes(mgr.PhysicalTerrainArray[tileX, tileZ]);
                        int width = terrainSplats[0].width;
                        int height = terrainSplats[0].height;
                        float dimensions = width * height;

                        //Get the average colours of the terrain textures by using the highest mip
                        Color[] averageSplatColors = new Color[terrainSplatPrototypes.Length];
                        for (int protoIdx = 0; protoIdx < terrainSplatPrototypes.Length; protoIdx++)
                        {
                            GaiaSplatPrototype proto = terrainSplatPrototypes[protoIdx];
                            Texture2D tmpTerrainTex = ResizeTexture(proto.texture, TextureFormat.ARGB32, 8, width, height, true, false, false);
                            Color[] maxMipColors = tmpTerrainTex.GetPixels(tmpTerrainTex.mipmapCount - 1);
                            averageSplatColors[protoIdx] = new Color(maxMipColors[0].r, maxMipColors[0].g, maxMipColors[0].b, maxMipColors[0].a);
                        }


                        //Create the new texture
                        Texture2D colorTex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
                        colorTex.name = mgr.PhysicalTerrainArray[tileX, tileZ].name + "_BaseMap";
                        colorTex.wrapMode = TextureWrapMode.Repeat;
                        colorTex.filterMode = FilterMode.Bilinear;
                        colorTex.anisoLevel = 8;
                        float xInv = 1f / width;
                        float zInv = 1f / height;
                        for (int x = 0; x < width; x++)
                        {
                            if (x % 250 == 0)
                            {
                                EditorUtility.DisplayProgressBar("Baking Textures", "Ingesting terrain basemap : " + mgr.PhysicalTerrainArray[tileX, tileZ].name + "..", (float)(x * width) / dimensions);
                            }

                            for (int z = 0; z < height; z++)
                            {
                                int splatColorIdx = 0;
                                Color mapColor = Color.black;
                                for (int splatIdx = 0; splatIdx < terrainSplats.Length; splatIdx++)
                                {
                                    Texture2D terrainSplat = terrainSplats[splatIdx];
                                    Color splatColor;
                                    if (m_basemapFlip)
                                    {
                                        splatColor = terrainSplat.GetPixel(z, x);
                                    }
                                    else
                                    {
                                        splatColor = terrainSplat.GetPixel(x, z);
                                    }

                                    if (splatColorIdx < averageSplatColors.Length)
                                    {
                                        mapColor = Color.Lerp(mapColor, averageSplatColors[splatColorIdx++], splatColor.r);
                                    }
                                    if (splatColorIdx < averageSplatColors.Length)
                                    {
                                        mapColor = Color.Lerp(mapColor, averageSplatColors[splatColorIdx++], splatColor.g);
                                    }
                                    if (splatColorIdx < averageSplatColors.Length)
                                    {
                                        mapColor = Color.Lerp(mapColor, averageSplatColors[splatColorIdx++], splatColor.b);
                                    }
                                    if (splatColorIdx < averageSplatColors.Length)
                                    {
                                        mapColor = Color.Lerp(mapColor, averageSplatColors[splatColorIdx++], splatColor.a);
                                    }
                                    if (alphaMask != null)
                                    {
                                        mapColor.a = alphaMask[xInv * x, zInv * z];
                                    }
                                    else
                                    {
                                        mapColor.a = 1f;
                                    }
                                }
                                colorTex.SetPixel(x, z, mapColor);
                            }
                        }
                        colorTex.Apply();

                        EditorUtility.DisplayProgressBar("Baking Textures", "Encoding terrain basemap : " + mgr.PhysicalTerrainArray[tileX, tileZ].name + "..", 0f);

                        //Save it
                        byte[] content = colorTex.EncodeToPNG();
                        File.WriteAllBytes(fname + ".png", content);

                        //Shut it up
                        EditorUtility.ClearProgressBar();
                    }
                }
            }
            
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Resize the supplied texture, also handles non rw textures and makes them rm
        /// </summary>
        /// <param name="texture">Source texture</param>
        /// <param name="width">Width of new texture</param>
        /// <param name="height">Height of new texture</param>
        /// <param name="mipmap">Generate mipmaps</param>
        /// <param name="linear">Use linear colour conversion</param>
        /// <returns>New texture</returns>
        public static Texture2D ResizeTexture(Texture2D texture, TextureFormat format, int aniso, int width, int height, bool mipmap, bool linear, bool compress)
        {
            RenderTexture rt;
            if (linear)
            {
                rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            }
            else
            {
                rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
            }
            bool prevRgbConversionState = GL.sRGBWrite;
            if (linear)
            {
                GL.sRGBWrite = false;
            }
            else
            {
                GL.sRGBWrite = true;
            }
            Graphics.Blit(texture, rt);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D newTexture = new Texture2D(width, height, format, mipmap, linear);
            newTexture.name = texture.name + " X";
            newTexture.anisoLevel = aniso;
            newTexture.filterMode = texture.filterMode;
            newTexture.wrapMode = texture.wrapMode;
            newTexture.mipMapBias = texture.mipMapBias;
            newTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            newTexture.Apply(true);

            if (compress)
            {
                newTexture.Compress(true);
                newTexture.Apply(true);
            }

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
            GL.sRGBWrite = prevRgbConversionState;
            return newTexture;
        }

        /// <summary>
        /// Apply the mask to the splatmap
        /// </summary>
        private void ApplyMaskToSplatMap()
        {
            SetupMask();

            if (m_workingMask == null)
            {
                Debug.LogWarning("No mask was supplied!");
                return;
            }

            if (Terrain.activeTerrain == null)
            {
                Debug.LogWarning("Must have active terrain");
                return;
            }

            m_splatBackup = Terrain.activeTerrain.terrainData.GetAlphamaps(0, 0,
                Terrain.activeTerrain.terrainData.alphamapWidth, Terrain.activeTerrain.terrainData.alphamapHeight);

            float[,,] splatMaps = Terrain.activeTerrain.terrainData.GetAlphamaps(0, 0,
                Terrain.activeTerrain.terrainData.alphamapWidth, Terrain.activeTerrain.terrainData.alphamapHeight);

            int width = splatMaps.GetLength(0);
            int height = splatMaps.GetLength(1);
            int numSplats = splatMaps.GetLength(2);

            if (m_targetSplatIdx >= numSplats)
            {
                Debug.LogWarning("Selected splat is out of bounds!");
                return;
            }

            float invX = 1f / (width - 1f);
            float invY = 1f / (height - 1f);
            float maskValue = 0f;
            float splatValue = 0f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    maskValue = m_workingMask[invX * x, invY * y];
                    splatValue = splatMaps[x, y, m_targetSplatIdx];

                    //Only care if we are increasing values
                    if (maskValue > splatValue)
                    {
                        float delta = maskValue - splatValue;
                        float theRest = 1f - splatValue;
                        float adjustment = 0f;
                        if (!GaiaCommon1.Utils.Math_ApproximatelyEqual(theRest, 0f))
                        {
                            adjustment = 1f - (delta / theRest);
                        }

                        for (int idx = 0; idx < numSplats; idx++)
                        {
                            if (idx == m_targetSplatIdx)
                            {
                                splatMaps[x, y, idx] = maskValue;
                            }
                            else
                            {
                                splatMaps[x, y, idx] *= adjustment;
                            }
                        }
                    }
                }
            }

            Terrain.activeTerrain.terrainData.SetAlphamaps(0,0, splatMaps);
        }

        /// <summary>
        /// Undo splatmap mod
        /// </summary>
        private void UndoApplyMaskToSplatMap()
        {
            if (m_splatBackup != null)
            {
                Terrain.activeTerrain.terrainData.SetAlphamaps(0, 0, m_splatBackup);
                m_splatBackup = null;
            }
        }

        private void ZTest()
        {
            string path = "Assets/GaiaExports/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //            WoodTexture mtex = new WoodTexture();
            //            HeightMap exportMap = new HeightMap(mtex.Generate(1024, 1024));
            //            string fname = path + "Wood";
            //            GaiaUtils.CompressToMultiChannelFileImage(fname, exportMap, exportMap, exportMap, exportMap, TextureFormat.RGBAFloat, GaiaConstants.ImageFileType.Exr);
            //            AssetDatabase.Refresh();


            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                //Load in the terrains from the world
                mgr.LoadFromWorld();

                //Grab the terrains from the manager
                UnityHeightMap[,] hmArray = mgr.HeightMapTerrainArray;
                int tilesX = hmArray.GetLength(0);
                int tilesZ = hmArray.GetLength(1);

                //Now iterate and do the work
                for (int tileX = 0; tileX < tilesX; tileX++)
                {
                    for (int tileZ = 0; tileZ < tilesZ; tileZ++)
                    {
                        //string fname = path;
                        //fname = Path.Combine(path, PWCommon.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_Z"));

//                        AdaptiveSmoothing filter = new AdaptiveSmoothing();
//                        System.Drawing.Bitmap bm = hmArray[tileX, tileZ].SaveToBitmap(PixelFormat.Format16bppGrayScale);
//                        filter.ApplyInPlace(bm);
//                        hmArray[tileX, tileZ].LoadFromBitmap(bm);
                    }
                }
            }
        }

        /// <summary>
        /// The tooltips
        /// </summary>
        private static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        {
            {"Operation", "The operation to perform."},
            {"Perform Operation", "Perform the operation."},
            {"Iterations", "The number of iterations to run the operation for. Large numbers of iterations can result in long wait times as some calculations are very cpu intensive."},
            {"Amount To Add", "The amount of height to add in meters."},
            {"Use Mask 1", "Use the first mask if the mask texture has been supplied."},
            {"Mask 1", "The texture to source the first mask from. Use 16 & 32 bit EXR based masks for best resolution. JPG & PNG are 8 bits per channel internally and will show artefacts in most operations."},
            {"Use Mask 2", "Use the second mask if the mask texture has been supplied."},
            {"Mask 2", "The texture to source the second mask from. Use 16 & 32 bit EXR based masks for best resolution. JPG & PNG are 8 bits per channel internally and will show artefacts in most operations."},
            {"Strength", "Mask is multiplied (scaled) by this value. Hitting R button disables it."},
            //{"Offset", "This offset is added to the mask. Hitting R button disables it."},
            {"Power", "The mask is modifed by this exponent. Hitting R button disables it."},
            {"Quantize", "Mask is quantized by this value. Hitting R button disables it."},
            {"Blur", "Mask is blurred this many times. Hitting R button disables it."},
            {"Flip", "Flip - often useful when dealing with terrain it is flipped internally."},
            {"Invert", "Invert - high values (white) becomes low values (black) and vice versa."},
            {"Normalize", "Scale between minimum and maximum values"},
            {"Flip Export", "Flip - often useful when dealing with terrain it is flipped internally."},
            {"Invert Export", "Invert - high values (white) becomes low values (black) and vice versa."},
            {"Normalize Export", "Scale between minimum and maximum values"},
            {"Target Splat", "The terrain texure that the mask will be applied to."},
            {"Mask Operation", "The way in which the masks will be merged."},
            {"Contrast", "The amount of contrast that will be applied."},
            {"Radius", "The number of pixels around the original that the operation will calculate its values from. Larger values will take longer to calculate."},
            {"Aspect Type", "The type of aspect to export."},
            {"Bake Mask to Chnl A", "If supplied, the mask will be baked into the A channel of the exported color map."},
            {"Curvature Type", "The type of curvature to export."},
            {"Noise Type", "The type of noise to be generated."},
            {"Seed", "The seed value for the noise function - the same seed will always generate the same noise for a given set of parameters."},
            {"Octaves", "The amount of detail in the noise - more octaves mean more detail and longer calculation time."},
            {"Persistence", "The roughness of the noise. Controls how quickly amplitudes diminish for successive octaves. 0..1."},
            {"Frequency", "The frequency of the first octave."},
            {"Lacunarity", "The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5."},
            {"Offset", "The vertical / strength offset to be applied to the noise. Can be used to create noise islands"},
            {"Zoom", "The zoom level of the noise."},
            {"Hardness Map", "A map that represents the hardness of a location on the terrain."},
            {"Rainmap Type", "A map that represents where the rain falls on the terrain. Areas with higher rainfall will erode more."},
            {"Rain Frequency", "The amount of iterations that it takes for the rain to evaporate. The rain map is applied again when this happens."},
            {"Disolve Rate", "The impact of the rain on the terrain in terms of disolving and picking up sediment."},
            {"Multiplier", "The amount to multiply the terrain by."},
            {"Exponent", "The exponent used in the power function applied to the terrain."},
            {"Quantum", "The height at which the terrain will be quantized in meters."},
            {"Transform Height","Pre-process and modify the stamp height. Can be used to further refine stamp shapes."},
            {"AddToTerrain","Add To Terrain : This filter will add the specified height uniformly across the terrain."},
            {"ContrastFilter","Contrast Filter : The contrast filter increases the relative contrast of the terrain."},
            {"ApplyMaskToSplatmap","Apply Mask To Splatmap : This utility will take the supplied mask and paint it into the splat map (terrain texture) specified. It will only paint the texture at a location if its strength is stronger than what it was before."},
            {"GrowFeaturesFilter","Grow Features Filter : This filter will grow the features on the terrain. Increasing the radius will result in the filter sampling a larger area of the terrain around it."},
            {"DeNoiseFilter","De Noise Filter : This filter will remove small lumps and holes. Its a great way to reduce the poly count of your terrain without degrading its visual appearance."},
            {"HydraulicFilter","Hydraulic Filter : This filter will apply hydraulic erosion to your terrain, and export the erosion sediment map into your GaiaExports directory. You can ad some nice interest to your terrain by then applying that sediment map as a mask to back on your sand or grass splatmap. Be patient with large numbers of iterations as it can take a long time to calculate."},
            {"MultiplyTerrain","Multiply Terrain Filter: This filter will multiply the terrain heights by the value you supply."},
            {"PowerOfFilter","Power Of Filter: This filter raises the terrain height to the power of the exponent you supply."},
            {"QuantizeFilter","Quantize Filter: This filter quantizes the terrain to the quantum value you supply."},
            {"QuantizeCurvesFilter","Quantize Curves Filter (Terrace): This filter applies a curve function to the terrain between the values you supply. Curves are applied between the start height you assign and the next start height. The height of the curve will control the extent to which the original terrain is modified by the filter. Zero will lock terrain to start height, one will pass the terrain through unmodified."},
            {"SetTerrainToHeight","Set Terrain To Height: This filter will set the terrain to the height you supply."},
            {"ShrinkFeaturesFilter","Shink Features Filter : This filter will shink the more prominent features of the terrain. It can be uses as a type of smoothing filter. The Radius determines the distance the surrounding terrain sampled when shrinking the feature."},
            {"SubtractFromTerrain","Subtract From Terrain : This filter subtract the height supplied from the entire terrain."},
            {"ThermalFilter","Thermal Erosion Filter: This filter will apply thermal erosion to the terrain. Experiment with very small values to create interesting terracing effects. Be patient with large numbers of iterations as it can take a long time to calculate."},
            {"ExportAspectMap","Export Aspect Map: Exports an apect map of your terrain into the GaiaExports directory."},
            {"ExportBaseMap","Export Base Map: Exports the base map (colours) of your terrain into the GaiaExports directory. You can also inject a mask (map) into the alpha channel of the map for use with CTS. Export your terrain as a low poly mesh (see advanced menu), a normal map and a base map, and you have a nice low poly feature to use in the distance!"},
            {"ExportCurvatureMap","Export Curvature Map: Exports a curvature map of your terrain into the GaiaExports directory."},
            {"ExportFlowMap","Export Flow Map: Exports a map of your terrain showing where water would flow into the GaiaExports directory. Use this as a mask with the subtract function to create a quick erosion effect."},
            {"ExportHeightMap","Export Height Map: Exports a height map of your terraininto the GaiaExports directory. Turn these into new stamps with the scanner (see advanced menu) and its also a very handy mask."},
            {"ExportNoiseMap","Export Noise Map: Exports a noise map into the GaiaExports directory. These make great masks to add variation to the other filters."},
            {"ExportNormalMap","Export Normal Map: Exports a normal map into the GaiaExports directory. Tip: Export your terrain as a low poly mesh (see advanced menu), a normal map and a base map, and you have a nice low poly feature to use in the distance!"},
            {"ExportMasks","Export Masks: Combine your exported maps together to create new maps in your GaiaExports directory to use as masks for other filters!"},
            {"ExportSlopeMap","Export Slope Map: Combine your terrains slope into your GaiaExports directory to use as masks for other filters!"},
        };
    }
}