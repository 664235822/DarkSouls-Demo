
using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// Wrapper to allow unified access to splat prototype data across different unity terrain APIs (pre and post 2018.3)
    /// </summary>
    public struct GaiaSplatPrototype
    {
        public float metallic;
        public Texture2D normalMap;
        public float smoothness;
        public Texture2D texture;
        public Vector2 tileOffset;
        public Vector2 tileSize;

#if UNITY_2018_3_OR_NEWER

        public GaiaSplatPrototype(TerrainLayer terrainLayer)
        {
            //Initialize empty
            metallic = 0f;
            normalMap = null;
            smoothness = 0f;
            texture = null;
            tileOffset = Vector2.zero;
            tileSize = Vector2.zero;

            if (terrainLayer != null)
            {
                metallic = terrainLayer.metallic;
                normalMap = terrainLayer.normalMapTexture;
                smoothness = terrainLayer.smoothness;
                texture = terrainLayer.diffuseTexture;
                tileOffset = terrainLayer.tileOffset;
                tileSize = terrainLayer.tileSize;
            }
        }

        /// <summary>
        /// Returns the contained data as a TerrainLayer object.
        /// </summary>
        /// <returns>A TerrainLayer object.</returns>
        public TerrainLayer Convert()
        {
            return new TerrainLayer
            {
                metallic = metallic,
                normalMapTexture = normalMap,
                smoothness = smoothness,
                diffuseTexture = texture,
                tileOffset = tileOffset,
                tileSize = tileSize
            };
        }

#else

        public GaiaSplatPrototype(SplatPrototype splatPrototype)
        {
            //Initialize empty
            metallic = 0f;
            normalMap = null;
            smoothness = 0f;
            texture = null;
            tileOffset = Vector2.zero;
            tileSize = Vector2.zero;

            if (splatPrototype != null)
            {
                metallic = splatPrototype.metallic;
                normalMap = splatPrototype.normalMap;
                smoothness = splatPrototype.smoothness;
                texture = splatPrototype.texture;
                tileOffset = splatPrototype.tileOffset;
                tileSize = splatPrototype.tileSize;
            }
        }

        /// <summary>
        /// Returns the contained data as a SplatPrototype object.
        /// </summary>
        /// <returns>A SplatPrototype object.</returns>
        public SplatPrototype Convert()
        {
            return new SplatPrototype {
                                        metallic = metallic,
                                        normalMap = normalMap,
                                        smoothness = smoothness,
                                        texture = texture,
                                        tileOffset = tileOffset,
                                        tileSize = tileSize
                                    };
        }

#endif

        /// <summary>
        /// Gets all splat prototypes from a terrain. Uses the correct terrain API for pre and post Unity 2018.3.
        /// </summary>
        /// <param name="terrain">The terrain containing the splat prototype data.</param>
        /// <returns>Null if invalid terrain data. An empty GaiaSplatPrototype array if no splat prototypes in terrain data. A filled GaiaSplatPrototype array if splat prototypes present.</returns>
        public static GaiaSplatPrototype[] GetGaiaSplatPrototypes(Terrain terrain)
        {
            if (terrain == null || terrain.terrainData == null)
            {
                return null;
            }
            TerrainData terrainData = terrain.terrainData;

#if UNITY_2018_3_OR_NEWER

            if (terrainData.terrainLayers == null || terrainData.terrainLayers.Length == 0)
            {
                return new GaiaSplatPrototype[0];
            }

            GaiaSplatPrototype[] splatPrototypes = new GaiaSplatPrototype[terrainData.terrainLayers.Length];

            for (int i = 0; i < terrainData.terrainLayers.Length; i++)
            {
                splatPrototypes[i] = new GaiaSplatPrototype(terrainData.terrainLayers[i]);
            }
            return splatPrototypes;
#else
            if (terrainData.splatPrototypes == null ||terrainData.splatPrototypes.Length == 0)
            {
                return new GaiaSplatPrototype[0];
            }

            GaiaSplatPrototype[] splatPrototypes = new GaiaSplatPrototype[terrainData.splatPrototypes.Length];

            for (int i = 0; i < terrainData.splatPrototypes.Length; i++)
            {
                splatPrototypes[i] = new GaiaSplatPrototype(terrainData.splatPrototypes[i]);
            }
            return splatPrototypes;
#endif

        }

        /// <summary>
        /// Applies an array of Gaia Splat prototypes to a terrain. Uses the correct terrain API for pre and post Unity 2018.3.
        /// </summary>
        /// <param name="terrain">The terrain to assign the splat prototypes to.</param>
        /// <param name="splats">Array of GaiaSplatPrototypes to assign to the terrain.</param>
        /// <param name="terrainName">The current Gaia profile. Used for terrain layer asset filenames.</param>
        public static void SetGaiaSplatPrototypes(Terrain terrain, GaiaSplatPrototype[] splats, string terrainName)
        {
            if (terrain != null && splats != null)
            {
#if UNITY_2018_3_OR_NEWER

                TerrainLayer[] terrainLayers = new TerrainLayer[splats.Length];

                for (int i = 0; i < splats.Length; i++)
                {
                    terrainLayers[i] = splats[i].Convert();
                }

                //completely remove all old splat prototypes first to prevent build-up of abandoned files
                RemoveTerrainLayerAssetFiles(terrainName);

                //Permanently save the new layers as asset files & get a reference, else they will not work properly in the terrain
                for (int i = 0; i < terrainLayers.Length; i++)
                {
                    terrainLayers[i] = SaveTerrainLayerAsAsset(terrainName, i.ToString(), terrainLayers[i]);
                }
                terrain.terrainData.terrainLayers = terrainLayers;

#else

                SplatPrototype[] splatPrototypes = new SplatPrototype[splats.Length];

                for (int i = 0; i < splats.Length; i++)
                {
                    splatPrototypes[i] = splats[i].Convert();
                }
                terrain.terrainData.splatPrototypes = splatPrototypes;

#endif
            }
        }
#if UNITY_2018_3_OR_NEWER
        /// <summary>
        /// Looks up terrain layer asset files matching a Gaia terrain, and returns them as an array.
        /// </summary>
        /// <param name="terrainName">The Gaia terrain name to look up terrain layer asset files for.</param>
        /// <returns></returns>
        private static TerrainLayer[] LookupTerrainLayerAssetFiles(string terrainName)
        {
#if UNITY_EDITOR

            string gaiaDirectory = "";
            string terrainLayerDirectory = gaiaDirectory + "Profiles/TerrainLayers";
            DirectoryInfo info = new DirectoryInfo(terrainLayerDirectory);
            FileInfo[] fileInfo = info.GetFiles(terrainName + "*.asset");

            TerrainLayer[] returnArray = new TerrainLayer[fileInfo.Length];

            for (int i = 0; i < fileInfo.Length; i++)
            {
                returnArray[i] = (TerrainLayer)AssetDatabase.LoadAssetAtPath("Assets" + fileInfo[i].FullName.Substring(Application.dataPath.Length), typeof(TerrainLayer));
            }
            return returnArray;
#else
            Debug.LogError("Runtime Gaia operation is not supported");
            return new TerrainLayer[0];
#endif
        }

        /// <summary>
        /// Saves a unity terrain layer as asset file and returns a reference to the newly created Terrain Layerfile.
        /// </summary>
        /// <param name="terrainName">The name of the current Gaia terrain (for the filename).</param>
        /// <param name="layerId">The layer ID of the layer that is to be saved (for the filename).</param>
        /// <param name="terrainLayer">The terrain layer object to save.</param>
        /// <returns>Reference to the created TerrainLayer</returns>
        private static TerrainLayer SaveTerrainLayerAsAsset(string terrainName, string layerId, TerrainLayer terrainLayer)
        {
#if UNITY_EDITOR

            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();

            if (gaiaSettings.m_currentTerrainLayerStoragePath == "")
            {
                Debug.LogError("Current Terrain Layer Storage Path is empty, please check the Gaia Settings file!");
                return null;
            }

            
            Directory.CreateDirectory(gaiaSettings.m_currentTerrainLayerStoragePath);

            //The combination of terrain name and layer id should be unique enough so that users don't overwrite layers between terrains. 
            string path = gaiaSettings.m_currentTerrainLayerStoragePath + "/" + terrainName + "_" + layerId + ".asset";

            AssetDatabase.CreateAsset(terrainLayer, path);
            AssetDatabase.SaveAssets();

            return AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);

#else
            Debug.LogError("Runtime Gaia operation is not supported");
            return new TerrainLayer();
#endif
        }

        /// <summary>
        /// Removes all Terrain Layer Asset Files for a given terrain
        /// </summary>
        /// <param name="terrainName"></param>
        private static void RemoveTerrainLayerAssetFiles(string terrainName)
        {

#if UNITY_EDITOR
            string gaiaDirectory = "";
            string terrainLayerDirectory = gaiaDirectory + "Profiles/TerrainLayers";
            DirectoryInfo info = new DirectoryInfo(terrainLayerDirectory);
            //only read in files if the directory exists.
            //The save function will create the directory in case it is missing.
            if (info.Exists)
            {
                FileInfo[] fileInfo = info.GetFiles(terrainName + "*.asset");

                for (int i = 0; i < fileInfo.Length; i++)
                {
                    File.Delete(fileInfo[i].FullName);
                }
            }
#else
            Debug.LogError("Runtime Gaia operation is not supported");
#endif

        }
#endif

    }

}
