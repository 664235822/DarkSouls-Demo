using System;
using System.Text;
using Gaia.FullSerializer;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// This object stores Gaia defaults. There should be only one of these in your project. Gaia will automatically pick these up and use them.
    /// </summary>
    public class GaiaDefaults : ScriptableObject
    {
        [Tooltip("Unique identifier for these defaults."), HideInInspector]
        public string m_defaultsID = Guid.NewGuid().ToString();

        [Tooltip("The absolute height of the sea or water table in meters. All spawn criteria heights are calculated relative to this. Used to populate initial sea level in new resources files.")]
        public float m_seaLevel = 50f;

        [Tooltip("The beach height in meters. Beaches are spawned at sea level and are extended for this height above sea level. This is used when creating default spawn rules in order to create a beach in the zone between water and land. Only used to populate initial beach height in new resources files.")]
        public float m_beachHeight = 5f;

        [Range(1, 20),Tooltip("Number of tiles in X direction."), HideInInspector]
        public int m_tilesX = 1;

        [Range(1, 20), Tooltip("Number of tiles in Z direction."), HideInInspector]
        public int m_tilesZ = 1;

        [Header("Base Terrain:")]
        [Space(5)]
        [Tooltip("The accuracy of the mapping between the terrain maps (heightmap, textures, etc) and the generated terrain; higher values indicate lower accuracy but lower rendering overhead.")]
        [Range(1, 200)]
        public int m_pixelError = 5;
        [Tooltip("The maximum distance at which terrain textures will be displayed at full resolution. Beyond this distance, a lower resolution composite image will be used for efficiency.")]
        [Range(0, 2000)]
        public int m_baseMapDist = 1024;
#if UNITY_2019_1_OR_NEWER
        [Tooltip("The shadow casting mode for the terrain.")]
        public UnityEngine.Rendering.ShadowCastingMode m_shaodwCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
#else
        [Tooltip("Whether or not the terrain casts shadows.")]
        public bool m_castShadows = true;
#endif
        [Tooltip("The material used to render the terrain. This should use a suitable shader, for example Nature/Terrain/Diffuse. The default terrain shader is used if no material is supplied.")]
        public Material m_material;
        [Tooltip("The Physic Material used for the terrain surface to specify its friction and bounce.")]
        public PhysicMaterial m_physicsMaterial;

        [Header("Tree & Detail Objects:")]
        [Space(5)]
        [Tooltip("Draw trees, grass & details.")]
        public bool m_draw = true;
        [Tooltip("The distance (from camera) beyond which details will be culled.")]
        [Range(0, 250)]
        public int m_detailDistance = 120;
        [Tooltip("The number of detail/grass objects in a given unit of area. The value can be set lower to reduce rendering overhead.")]
        [Range(0f, 1f)]
        public float m_detailDensity = 1.0f;
        [Tooltip("The distance (from camera) beyond which trees will be culled.")]
        [Range(0, 2000)]
        public int m_treeDistance = 500;
        [Tooltip("The distance (from camera) at which 3D tree objects will be replaced by billboard images.")]
        [Range(5, 2000)]
        public int m_billboardStart = 50;
        [Tooltip("Distance over which trees will transition between 3D objects and billboards.There is often a rotation effect as this kicks in.")]
        [Range(0, 200)]
        public int m_fadeLength = 20;
        [Tooltip("The maximum number of visible trees that will be represented as solid 3D meshes. Beyond this limit, trees will be replaced with billboards.")]
        [Range(0, 10000)]
        public int m_maxMeshTrees = 50;

        [Header("Wind Settings:")]
        [Space(5)]
        [Tooltip("The speed of the wind as it blows grass.")]
        [Range(0f, 1f)]
        public float m_speed = 0.35f;
        [Tooltip("The size of the “ripples” on grassy areas as the wind blows over them.")]
        [Range(0f, 1f)]
        public float m_size = 0.12f;
        [Tooltip("The degree to which grass objects are bent over by the wind.")]
        [Range(0f, 1f)]
        public float m_bending = 0.1f;
        [Tooltip("Overall color tint applied to grass objects.")]
        public Color m_grassTint = new Color(180f/255f, 180f/255f, 180f/255f, 1f);

        [Header("Resolution Settings:")]
        [Space(5)]
        [Tooltip("The size of terrain tile in X & Z axis (in world units).")]
        public int m_terrainSize = 2048;
        [Tooltip("The height of the terrain in world unit meters")]
        public int m_terrainHeight = 700;
        [Tooltip("Pixel resolution of the terrain’s heightmap (should be a power of two plus one e.g. 513 = 512 + 1). Higher resolutions allow for more detailed terrain features, at the cost of poorer performance.")]
        public int m_heightmapResolution = 1025;
        [Tooltip("Resolution of the map that determines the separate patches of details/grass. Higher resolution gives smaller and more detailed patches.")]
        public int m_detailResolution = 1024;
        [Tooltip("Length/width of the square of patches rendered with a single draw call.")]
        public int m_detailResolutionPerPatch = 8;
        [Tooltip("Resolution of the “splatmap” that controls the blending of the different terrain textures. Higher resolutions consumer more memory, but provide more accurate texturing.")]
        public int m_controlTextureResolution = 1024;
        [Tooltip("Resolution of the composite texture used on the terrain when viewed from a distance greater than the Basemap Distance (see above).")]
        public int m_baseMapSize = 1024;


        /// <summary>
        /// Create the terrain defined by these settings
        /// </summary>
        public void CreateTerrain()
        {
            Terrain[,] world;

            //Update the session
            GaiaSessionManager sessionMgr = GaiaSessionManager.GetSessionManager();
            if (sessionMgr != null && sessionMgr.IsLocked() != true)
            {
                //Update terrain settings in session
                sessionMgr.m_session.m_terrainWidth = m_tilesX * m_terrainSize;
                sessionMgr.m_session.m_terrainDepth = m_tilesZ * m_terrainSize;
                sessionMgr.m_session.m_terrainHeight = m_terrainHeight;
                sessionMgr.AddDefaults(this);

                sessionMgr.SetSeaLevel(m_seaLevel);

                //Then add the operation
                GaiaOperation op = new GaiaOperation();
                op.m_description = "Creating terrain";
                op.m_generatedByID = m_defaultsID;
                op.m_generatedByName = this.name;
                op.m_generatedByType = this.GetType().ToString();
                op.m_isActive = true;
                op.m_operationDateTime = DateTime.Now.ToString();
                op.m_operationType = GaiaOperation.OperationType.CreateTerrain;
                sessionMgr.AddOperation(op);
            }

            //Create the terrains array
            world = new Terrain[m_tilesX, m_tilesZ];

            //And iterate through and create each terrain
            for (int x = 0; x < m_tilesX; x++)
            {
                for (int z = 0; z < m_tilesZ; z++)
                {
                    CreateTile(x, z, ref world, null);
                }
            }

            //Now join them together and remove their seams
            RemoveWorldSeams(ref world);
        }

        /// <summary>
        /// Update the defaults if possible from the currently selected terrain
        /// </summary>
        public void UpdateFromTerrain()
        {
            Terrain terrain = GetActiveTerrain();
            if (terrain == null)
            {
                Debug.Log("Could not update from active terrain - no current active terrain");
                return;
            }

            m_baseMapDist = (int)terrain.basemapDistance;

#if UNITY_2019_1_OR_NEWER
            m_shaodwCastingMode = terrain.shadowCastingMode;
#else
            m_castShadows = terrain.castShadows;
#endif
            m_detailDensity = terrain.detailObjectDensity;
            m_detailDistance = (int)terrain.detailObjectDistance;
            m_pixelError = (int)terrain.heightmapPixelError;
            m_billboardStart = (int)terrain.treeBillboardDistance;
            m_fadeLength = (int)terrain.treeCrossFadeLength;
            m_treeDistance = (int)terrain.treeDistance;
            m_maxMeshTrees = terrain.treeMaximumFullLODCount;
            if (terrain.materialType == Terrain.MaterialType.Custom)
            {
                m_material = terrain.materialTemplate;
            }
            TerrainCollider collider = terrain.GetComponent<TerrainCollider>();
            if (collider != null)
            {
                m_physicsMaterial = collider.material;
            }

            TerrainData terrainData = terrain.terrainData;
            m_controlTextureResolution = terrainData.alphamapResolution;
            m_baseMapSize = terrainData.baseMapResolution;
            m_detailResolution = terrainData.detailResolution;
            //m_detailResolutionPerPatch = terrainData.
            m_heightmapResolution = terrainData.heightmapResolution;
            m_bending = terrainData.wavingGrassAmount;
            m_size = terrainData.wavingGrassSpeed;
            m_speed = terrainData.wavingGrassStrength;
            m_grassTint = terrainData.wavingGrassTint;
            m_terrainSize = (int)terrainData.size.x;
            m_terrainHeight = (int)terrainData.size.y;
        }


        /// <summary>
        /// Create the terrain defined by these settings - and apply the resources to it
        /// </summary>
        public void CreateTerrain(GaiaResource resources)
        {
            Terrain[,] world;

            //Update the resouces ny associating them with their assets
            resources.AssociateAssets();

            //Update the session
            GaiaSessionManager sessionMgr = GaiaSessionManager.GetSessionManager();
            if (sessionMgr != null && sessionMgr.IsLocked() != true)
            {
                //Update terrain settings in session
                sessionMgr.m_session.m_terrainWidth = m_tilesX * m_terrainSize;
                sessionMgr.m_session.m_terrainDepth = m_tilesZ * m_terrainSize;
                sessionMgr.m_session.m_terrainHeight = m_terrainHeight;

                //Add the defaults
                sessionMgr.AddDefaults(this);

                //Set the sea level - but only if it is zero - if not zero then its been deliberately set
                if (Gaia.GaiaUtils.Math_ApproximatelyEqual(sessionMgr.m_session.m_seaLevel, 0f))
                {
                    sessionMgr.SetSeaLevel(m_seaLevel);
                }

                //Grab the resources scriptable object
                sessionMgr.AddResource(resources);

                //Adjust them if they are different to the defaults
                resources.ChangeSeaLevel(sessionMgr.m_session.m_seaLevel);

                //Then add the operation
                sessionMgr.AddOperation(GetTerrainCreationOperation(resources));
            }

            //Create the terrains array
            world = new Terrain[m_tilesX, m_tilesZ];

            //And iterate through and create each terrain
            for (int x = 0; x < m_tilesX; x++)
            {
                for (int z = 0; z < m_tilesZ; z++)
                {
                    CreateTile(x, z, ref world, resources);
                }
            }

            //Now join them together and remove their seams
            RemoveWorldSeams(ref world);
        }

        public GaiaOperation GetTerrainCreationOperation(GaiaResource resources)
        {
            //Then add the operation
            GaiaOperation op = new GaiaOperation();
            op.m_description = "Creating terrain";
            op.m_generatedByID = m_defaultsID;
            op.m_generatedByName = this.name;
            op.m_generatedByType = this.GetType().ToString();
            op.m_isActive = true;
            op.m_operationDateTime = DateTime.Now.ToString();
            op.m_operationType = GaiaOperation.OperationType.CreateTerrain;
#if UNITY_EDITOR
            op.m_operationDataJson = new string[2];
            string ap = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(ap))
            {
                op.m_operationDataJson[0] = "GaiaDefaults.asset";
            }
            else
            {
                op.m_operationDataJson[0] = AssetDatabase.GetAssetPath(this);
            }
            ap = AssetDatabase.GetAssetPath(resources);
            if (string.IsNullOrEmpty(ap))
            {
                op.m_operationDataJson[1] = "GaiaResources.asset";
            }
            else
            {
                op.m_operationDataJson[1] = AssetDatabase.GetAssetPath(resources);
            }
#endif

            return op;
        }

        /// <summary>
        /// Get the currently active terrain - or any terrain
        /// </summary>
        /// <returns>A terrain if there is one</returns>
        public static Terrain GetActiveTerrain()
        {
            //Grab active terrain if we can
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null && terrain.isActiveAndEnabled)
            {
                return terrain;
            }

            //Then check rest of terrains
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                if (terrain != null && terrain.isActiveAndEnabled)
                {
                    return terrain;
                }
            }

            if (terrain == null)
            {
                Terrain findTerrainComponent = FindObjectOfType<Terrain>();
                if (findTerrainComponent != null && findTerrainComponent.isActiveAndEnabled)
                {
                    terrain = findTerrainComponent;
                    return terrain;
                }
            }

            return null;
        }

        /// <summary>
        /// Create a terrain tile based on these settings
        /// </summary>
        /// <param name="tx">X location</param>
        /// <param name="tz">Z location</param>
        /// <param name="world">The array managing it</param>
        private void CreateTile(int tx, int tz, ref Terrain[,] world, GaiaResource resources)
        {
            if (tx < 0 || tx >= m_tilesX)
            {
                Debug.LogError("X value out of bounds");
                return;
            }

            if (tz < 0 || tz >= m_tilesZ)
            {
                Debug.LogError("Z value out of bounds");
                return;
            }

            //Look for issues in the terrain settings and fix them
            GetAndFixDefaults();

            //this will center terrain at origin
            Vector2 m_offset = new Vector2(-m_terrainSize * m_tilesX * 0.5f, -m_terrainSize * m_tilesZ * 0.5f);

            //create the terrains if they dont already exist
            if (world.Length < m_tilesX)
            {
                world = new Terrain[m_tilesX, m_tilesZ];
            }

            //Create the terrain
            Terrain terrain;
            TerrainData terrainData = new TerrainData();

            terrainData.name = string.Format("Terrain_{0}_{1}-{2:yyyyMMdd-HHmmss}", tx, tz, DateTime.Now);
            terrainData.alphamapResolution = m_controlTextureResolution;
            terrainData.baseMapResolution = m_baseMapSize;
            terrainData.SetDetailResolution(m_detailResolution, m_detailResolutionPerPatch);
            terrainData.heightmapResolution = m_heightmapResolution;
            //terrainData.physicsMaterial = m_physicsMaterial;
            terrainData.wavingGrassAmount = m_bending;
            terrainData.wavingGrassSpeed = m_size;
            terrainData.wavingGrassStrength = m_speed;
            terrainData.wavingGrassTint = m_grassTint;
            terrainData.size = new Vector3(m_terrainSize, m_terrainHeight, m_terrainSize);

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(terrainData, string.Format("Assets/{0}.asset", terrainData.name));
#endif

            terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
            terrain.name = terrainData.name;
            terrain.transform.position =
                new Vector3(m_terrainSize * tx + m_offset.x, 0, m_terrainSize * tz + m_offset.y);
            terrain.basemapDistance = m_baseMapDist;
#if UNITY_2019_1_OR_NEWER
            terrain.shadowCastingMode = m_shaodwCastingMode;
#else
            terrain.castShadows = m_castShadows;
#endif
            terrain.detailObjectDensity = m_detailDensity;
            terrain.detailObjectDistance = m_detailDistance;
            terrain.heightmapPixelError = m_pixelError;
            terrain.treeBillboardDistance = m_billboardStart;
            terrain.treeCrossFadeLength = m_fadeLength;
            terrain.treeDistance = m_treeDistance;
            terrain.treeMaximumFullLODCount = m_maxMeshTrees;
#if UNITY_EDITOR
            GameObjectUtility.SetStaticEditorFlags(terrain.gameObject,
                StaticEditorFlags.BatchingStatic | StaticEditorFlags.NavigationStatic |
                StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic |
                StaticEditorFlags.OffMeshLinkGeneration | StaticEditorFlags.ReflectionProbeStatic | StaticEditorFlags.ContributeGI
                );
            terrain.bakeLightProbesForTrees = false;
#if UNITY_2018_3_OR_NEWER
            terrain.drawInstanced = true;
#endif
#endif

            GaiaConstants.EnvironmentRenderer rendererType = GaiaConstants.EnvironmentRenderer.BuiltIn;
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (gaiaSettings != null)
            {
                rendererType = gaiaSettings.m_currentRenderer;
#if !UNITY_2018_1_OR_NEWER
                rendererType = GaiaConstants.EnvironmentRenderer.BuiltIn;
#endif
            }

            if (rendererType == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                if (m_material != null)
                {
#if UNITY_EDITOR
                    GaiaPipelineUtils.SetupPipeline(rendererType, null, null, null, "Procedural Worlds/Simple Water", false);
#endif
                }
            }
            else
            {
                terrain.materialType = Terrain.MaterialType.Custom;
                if (rendererType == GaiaConstants.EnvironmentRenderer.LightWeight2018x)
                {
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
                    GaiaPipelineUtils.SetupPipeline(rendererType, "Procedural Worlds Lightweight Pipeline Profile", "Pipeline Terrain Material", "Lightweight Render Pipeline/Terrain/Lit", "Procedural Worlds/Simple Water LW", false);
#else
                    Debug.LogWarning("Lightweight Pipeline is only supposted in 2018.3 or newer");
#endif
                }
                else
                {
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
                    GaiaPipelineUtils.SetupPipeline(rendererType, "Procedural Worlds HDRenderPipelineAsset", "Pipeline Terrain Material", "HDRP/TerrainLit", "Procedural Worlds/Simple Water HD", false);
#else
                    Debug.LogWarning("Lightweight Pipeline is only supposted in 2018.3 or newer");
#endif
                }
            }

            if (m_physicsMaterial != null)
                {
                    TerrainCollider collider = terrain.GetComponent<TerrainCollider>();
                    if (collider != null)
                    {
                        collider.material = m_physicsMaterial;
                    }
                    else
                    {
                        Debug.LogWarning("Unable to assign physics material to terrain!");
                    }
                }

            //Assign prototypes
            if (resources != null)
            {
                resources.ApplyPrototypesToTerrain(terrain);
            }
            else
            {
                terrain.Flush();
            }

            //Save the new tile
            world[tx, tz] = terrain;

            //Parent it to the environment
            GameObject gaiaObj = GameObject.Find("Gaia Environment");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia Environment");
            }
            terrain.transform.parent = gaiaObj.transform;
        }

        /// <summary>
        /// Update the terrain tiles to relate properly to each other
        /// </summary>
        /// <param name="tiles"></param>
        private void RemoveWorldSeams(ref Terrain [,] world)
        {
            //Set the neighbours of terrain to remove seams.
            for (int x = 0; x < m_tilesX; x++)
            {
                for (int z = 0; z < m_tilesZ; z++)
                {
                    Terrain right = null;
                    Terrain left = null;
                    Terrain bottom = null;
                    Terrain top = null;

                    if (x > 0) left = world[(x - 1), z];
                    if (x < m_tilesX - 1) right = world[(x + 1), z];

                    if (z > 0) bottom = world[x, (z - 1)];
                    if (z < m_tilesZ - 1) top = world[x, (z + 1)];

                    world[x, z].SetNeighbors(left, top, right, bottom);
                }
            }
        }


        /// <summary>
        /// Report on and fix the values put into the defaults
        /// </summary>
        /// <returns></returns>
        public string GetAndFixDefaults()
        {
            StringBuilder defStr = new StringBuilder();

            if (!Mathf.IsPowerOfTwo(m_terrainSize))
            {
                defStr.AppendFormat("Terrain size must be power of 2! {0} was changed to {1}.\n", m_terrainSize, Mathf.ClosestPowerOfTwo(m_terrainSize));
                m_terrainSize = Mathf.ClosestPowerOfTwo(m_terrainSize);
            }

            if (!Mathf.IsPowerOfTwo(m_heightmapResolution - 1))
            {
                defStr.AppendFormat("Height map size must be power of 2 + 1 number! {0} was changed to {1}.\n", m_heightmapResolution, Mathf.ClosestPowerOfTwo(m_heightmapResolution) + 1);
                m_heightmapResolution = Mathf.ClosestPowerOfTwo(m_heightmapResolution) + 1;
            }

            if (!Mathf.IsPowerOfTwo(m_controlTextureResolution))
            {
                defStr.AppendFormat("Control texture resolution must be power of 2! {0} was changed to {1}.\n", m_controlTextureResolution, Mathf.ClosestPowerOfTwo(m_controlTextureResolution));
                m_controlTextureResolution = Mathf.ClosestPowerOfTwo(m_controlTextureResolution);
            }

            if (m_controlTextureResolution > 2048)
            {
                defStr.AppendFormat("Control texture resolution must be <= 2048! {0} was changed to {1}.\n", m_controlTextureResolution, 2048);
                m_controlTextureResolution = 2048;
            }

            if (!Mathf.IsPowerOfTwo(m_baseMapSize))
            {
                defStr.AppendFormat("Basemap size must be power of 2! {0} was changed to {1}.\n", m_baseMapSize, Mathf.ClosestPowerOfTwo(m_baseMapSize));
                m_baseMapSize = Mathf.ClosestPowerOfTwo(m_baseMapSize);
            }

            if (m_baseMapSize > 2048)
            {
                defStr.AppendFormat("Basemap size must be <= 2048! {0} was changed to {1}.\n", m_baseMapSize, 2048);
                m_baseMapSize = 2048;
            }

            if (!Mathf.IsPowerOfTwo(m_detailResolution))
            {
                defStr.AppendFormat("Detail map size must be power of 2! {0} was changed to {1}.\n", m_detailResolution, Mathf.ClosestPowerOfTwo(m_detailResolution));
                m_detailResolution = Mathf.ClosestPowerOfTwo(m_detailResolution);
            }

            if (m_detailResolutionPerPatch < 8)
            {
                defStr.AppendFormat("Detail resolution per patch must be >= 8! {0} was changed to {1}.\n", m_detailResolutionPerPatch, 8);
                m_detailResolutionPerPatch = 8;
            }

            return defStr.ToString();
        }

        /// <summary>
        /// Serialise this as json
        /// </summary>
        /// <returns></returns>
        public string SerialiseJson()
        {
            fsData data;
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(this, out data);
            return fsJsonPrinter.CompressedJson(data);
        }

        /// <summary>
        /// Deserialise the suplied json into this object
        /// </summary>
        /// <param name="json">Source json</param>
        public void DeSerialiseJson(string json)
        {
            fsData data = fsJsonParser.Parse(json);
            fsSerializer serializer = new fsSerializer();
            var defaults = this;
            serializer.TryDeserialize<GaiaDefaults>(data, ref defaults);
        }
    }
}