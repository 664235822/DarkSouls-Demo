using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using Gaia.FullSerializer;


namespace Gaia
{
    /// <summary>
    /// Class to apply stamps to the terrain
    /// </summary>
    [ExecuteInEditMode]
    public class Stamper : MonoBehaviour
    {
        #region Basic stamp control

        /// <summary>
        /// The stamp ID
        /// </summary>
        public string m_stampID = Guid.NewGuid().ToString();

        /// <summary>
        /// The preview texture for the stamp - used to load the stamp - stamp data is in Data directory under the preview
        /// </summary>
        public Texture2D m_stampPreviewImage;

        /// <summary>
        /// Stamp x location - done this way to expose in the editor as a simple slider
        /// </summary>
        public float m_x = 0f;

        /// <summary>
        /// Stamp y location - done this way to expose in the editor as a simple slider
        /// </summary>
        public float m_y = 50f;

        /// <summary>
        /// Stamp z location - done this way to expose in the editor as a simple slider
        /// </summary>
        public float m_z = 0f;

        /// <summary>
        /// Stamp width - this is the horizontal scaling factor - applied to both x & z
        /// </summary>
        public float m_width = 10f;

        /// <summary>
        /// Stamp height - this is the vertical scaling factor
        /// </summary>
        public float m_height = 10f;

        /// <summary>
        /// Stamp rotation
        /// </summary>
        public float m_rotation = 0f;

        /// <summary>
        /// Used to stick the stamp to the ground - makes height changes more understandable
        /// </summary>
        public bool m_stickBaseToGround = true;

        /// <summary>
        /// The resources that apply to this stamp - changes in sea level in the stamp are reflected back to the resources
        /// so that spawners will adapt correctly.
        /// </summary>
        [fsIgnore]
        public Gaia.GaiaResource m_resources;

        /// <summary>
        /// Of the resoucces are missing then base sea level off this instead
        /// </summary>
        [fsIgnore]
        public float m_seaLevel = 0f;

        /// <summary>
        /// The path this resources file came from
        /// </summary>
        public string m_resourcesPath;

        #endregion

        #region Stamp variables

        /// <summary>
        /// Toggling this value will toggle the inversion status on the stamp - preset to have the stamp inverted when it is loaded
        /// </summary>
        public bool m_invertStamp = false;

        /// <summary>
        /// Toggling this value will toggle the normalisation status of the stamp - preset to have the stamp normalised when loaded
        /// </summary>
        public bool m_normaliseStamp = false;

        /// <summary>
        /// The ground / base level - value in 0..1 that determines where the base of the stamp is as a % pf overall stamp height. 
        /// Initially loaded from scanned value stored in the stamp, but can be overridden.
        /// </summary>
        public float m_baseLevel = 0f;

        /// <summary>
        /// Whether or not to draw any portion of the stamp below the base level of the stamp
        /// </summary>
        public bool m_drawStampBase = true;

        /// <summary>
        /// Stamp operation type - determines how the stamp will be applied
        /// </summary>
        public GaiaConstants.FeatureOperation m_stampOperation = GaiaConstants.FeatureOperation.RaiseHeight;

        /// <summary>
        /// Stamp smooth iterations - the number of smoothing iterations to be applied to a stamp before stamping - use to clean up noisy stamps
        /// without affecting rest of the terrain.
        /// </summary>
        public int m_smoothIterations = 0;

        /// <summary>
        /// The blend strength to use 0.. original terrain... 1.. new stamp - used only for Constants.FeatureOperation.BlendHeight
        /// </summary>
        public float m_blendStrength = 0.5f; //The strength of the stamp if blending

        /// <summary>
        /// The physical stencil height in meters - adds or subtracts to that height based on the stamp height 0.. no impact... 1.. full impact.
        /// Most normalise stamp first for this to be accurate. Only used for Constants.FeatureOperation.StencilHeight
        /// </summary>
        public float m_stencilHeight = 1f;

        /// <summary>
        /// A curve that influences adjusts the height of the stamp
        /// </summary>
        public AnimationCurve m_heightModifier = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        /// <summary>
        /// A curve that influences the strength of the stamp over distance from centre. LHS is centre of stamp, RHS is outer edge of stamp. 
        /// </summary>
        public AnimationCurve m_distanceMask = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        /// <summary>
        /// The area mask to apply
        /// </summary>
        public GaiaConstants.ImageFitnessFilterMode m_areaMaskMode = GaiaConstants.ImageFitnessFilterMode.None;

        /// <summary>
        /// The source testure used for area based filters, can be used in conjunction with the distance mask. Values range in 0..1.
        /// </summary>
        public Texture2D m_imageMask;

        /// <summary>
        /// This is used to invert the strengh supplied image mask texture
        /// </summary>
        public bool m_imageMaskInvert = false;

        /// <summary>
        /// This is used to invert the strengh supplied image mask texture
        /// </summary>
        public bool m_imageMaskNormalise = false;

        /// <summary>
        /// Flip the x, z of the image texture - sometimes required to match source with unity terrain
        /// </summary>
        public bool m_imageMaskFlip = false;

        /// <summary>
        /// This is used to smooth the supplied image mask texture
        /// </summary>
        public int m_imageMaskSmoothIterations = 3;

        /// <summary>
        /// The heightmap for the image filter
        /// </summary>
        [fsIgnore]
        public HeightMap m_imageMaskHM;

        /// <summary>
        /// Seed for noise based fractal
        /// </summary>
        public float m_noiseMaskSeed = 0;

        /// <summary>
        /// The amount of detail in the fractal - more octaves mean more detail and longer calc time.
        /// </summary>
        public int m_noiseMaskOctaves = 8;

        /// <summary>
        /// The roughness of the fractal noise. Controls how quickly amplitudes diminish for successive octaves. 0..1.
        /// </summary>
        public float m_noiseMaskPersistence = 0.25f;

        /// <summary>
        /// The frequency of the first octave
        /// </summary>
        public float m_noiseMaskFrequency = 1f;

        /// <summary>
        /// The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5.
        /// </summary>
        public float m_noiseMaskLacunarity = 1.5f;

        /// <summary>
        /// The zoom level of the noise
        /// </summary>
        public float m_noiseZoom = 10f;

        /// <summary>
        /// Used to force the stamp to always show itself when you select something else in the editor
        /// </summary>
        public bool m_alwaysShow = false;

        /// <summary>
        /// Set this to true if we want to show the base
        /// </summary>
        public bool m_showBase = true;

        /// <summary>
        /// Used to get the stamp to draw the sea level
        /// </summary>
        public bool m_showSeaLevel = true;

        /// <summary>
        /// Used to show the stamp rulers - some people might find this useful
        /// </summary>
        public bool m_showRulers = false;

        /// <summary>
        /// Shows the terrain helper - handy helper feature
        /// </summary>
        public bool m_showTerrainHelper = false;

        /// <summary>
        /// Gizmo colour
        /// </summary>
        [fsIgnore]
        public Color m_gizmoColour = new Color(1f, .6f, 0f, 1f);

        /// <summary>
        /// Use for co-routine simulation
        /// </summary>
        [fsIgnore]
        public IEnumerator m_updateCoroutine;

        /// <summary>
        /// Amount of time per allowed update
        /// </summary>
        [fsIgnore]
        public float m_updateTimeAllowed = 1f / 30f;

        /// <summary>
        /// Current progress on updating the stamp
        /// </summary>
        [fsIgnore]
        public float m_stampProgress = 0f;

        /// <summary>
        /// Whether or not its completed processing
        /// </summary>
        [fsIgnore]
        public bool m_stampComplete = true;

        /// <summary>
        /// Whether or not to cancel it
        /// </summary>
        [fsIgnore]
        public bool m_cancelStamp = false;

        /// <summary>
        /// The material used to preview the stamp
        /// </summary>
        [fsIgnore]
        public Material m_previewMaterial;

        #endregion

        #region Private variables

        /// <summary>
        /// Current feature ID - used to do feature change detection
        /// </summary>
#pragma warning disable 414
        private int m_featureID;
#pragma warning restore 414

        /// <summary>
        /// Internal variables to control how the stamp stamps
        /// </summary>
        private int m_scanWidth = 0;
        private int m_scanDepth = 0;
        private int m_scanHeight = 0;
        private float m_scanResolution = 0.1f; //Every 10 cm
        private Bounds m_scanBounds;
        private UnityHeightMap m_stampHM;
        private GaiaWorldManager m_undoMgr;
        private GaiaWorldManager m_redoMgr;
        private MeshFilter m_previewFilter;
        private MeshRenderer m_previewRenderer;

        #endregion

        #region Public API Methods

        /// <summary>
        /// Load the currently selected stamp
        /// </summary>
        public void LoadStamp()
        {
            m_featureID = -1;
            m_scanBounds = new Bounds(transform.position, Vector3.one * 10f);

            //See if we have something to load
            if (m_stampPreviewImage == null)
            {
                Debug.LogWarning("Can't load feature - texture not set");
                return;
            }

            //Get path
            m_featureID = m_stampPreviewImage.GetInstanceID();
            if (!GaiaUtils.CheckValidGaiaStampPath(m_stampPreviewImage))
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("OOPS!", "The image you have selected is not a valid Stamp preview. You can find your Stamps and their Stamp previews in one of the directories underneath your Gaia\\Stamps directory. \n\nIf you want to turn this image into a Stamp that can be used by the Stamper then please use the Scanner. You can access the Scanner via the utilities section of the Gaia Manager window. You can open Gaia Manager by pressing Ctrl G, or selecting Window -> Gaia -> Gaia Manager.", "OK");
#else
                Debug.LogError("The file provided is not a valid stamp. You need to drag the stamp preview from one of the directories underneath your Gaia Stamps directory.");
#endif
                m_featureID = -1;
                m_stampPreviewImage = null;
                return;
            }
            string path = GaiaUtils.GetGaiaStampPath(m_stampPreviewImage);

            //Load stamp
            m_stampHM = new UnityHeightMap(path);
            if (!m_stampHM.HasData())
            {
                m_featureID = -1;
                m_stampPreviewImage = null;
                Debug.LogError("Was unable to load " + path);
                return;
            }

            //Get metadata
            float[] metaData = new float[5];
            Buffer.BlockCopy(m_stampHM.GetMetaData(), 0, metaData, 0, metaData.Length * 4);
            m_scanWidth = (int)metaData[0];
            m_scanDepth = (int)metaData[1];
            m_scanHeight = (int)metaData[2];
            m_scanResolution = metaData[3];
            m_baseLevel = metaData[4];
            m_scanBounds = new Bounds(transform.position, new Vector3(
               (float)m_scanWidth * m_scanResolution * m_width,
               (float)m_scanHeight * m_scanResolution * m_height,
               (float)m_scanDepth * m_scanResolution * m_width));

            //Invert
            if (m_invertStamp)
            {
                m_stampHM.Invert();
            }

            //Normalise
            if (m_normaliseStamp)
            {
                m_stampHM.Normalise();
            }

            //Generate the feature mesh
            GeneratePreviewMesh();
        }

        /// <summary>
        /// Load the stamp at the image preview path provided
        /// </summary>
        /// <param name="imagePreviewPath">Path to the image preview</param>
        public void LoadStamp(string imagePreviewPath)
        {
#if UNITY_EDITOR
            m_stampPreviewImage = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePreviewPath);
#endif
            LoadStamp();
        }

        /// <summary>
        /// Bypass the image preview stuff and load the stamp at runtime - the stamp must be located in the resources directory
        /// </summary>
        /// <param name="stampPath">Path eg Assets/Resources/Stamps/Mountain1.bytes</param>
        public bool LoadRuntimeStamp(TextAsset stamp)
        {
            //Load stamp
            m_stampHM = new UnityHeightMap(stamp);
            if (!m_stampHM.HasData())
            {
                m_featureID = -1;
                m_stampPreviewImage = null;
                Debug.LogError("Was unable to load textasset stamp");
                return false;
            }

            //Get metadata
            float[] metaData = new float[5];
            Buffer.BlockCopy(m_stampHM.GetMetaData(), 0, metaData, 0, metaData.Length * 4);
            m_scanWidth = (int)metaData[0];
            m_scanDepth = (int)metaData[1];
            m_scanHeight = (int)metaData[2];
            m_scanResolution = metaData[3];
            m_baseLevel = metaData[4];
            m_scanBounds = new Bounds(transform.position, new Vector3(
               (float)m_scanWidth * m_scanResolution * m_width,
               (float)m_scanHeight * m_scanResolution * m_height,
               (float)m_scanDepth * m_scanResolution * m_width));

            //Invert
            if (m_invertStamp)
            {
                m_stampHM.Invert();
            }

            //Normalise
            if (m_normaliseStamp)
            {
                m_stampHM.Normalise();
            }

            //We are good
            return true;
        }

        /// <summary>
        /// Invert the stamp
        /// </summary>
        public void InvertStamp()
        {
            m_stampHM.Invert();
            GeneratePreviewMesh();
        }

        /// <summary>
        /// Normalise the stamp - makes stamp use full dynamic range - particularly usefule for stencil
        /// </summary>
        public void NormaliseStamp()
        {
            m_stampHM.Normalise();
            GeneratePreviewMesh();
        }

        /// <summary>
        /// Stamp the stamp - will kick the co-routine off
        /// </summary>
        public void Stamp()
        {
            //Reset status
            m_cancelStamp = false;
            m_stampComplete = false;
            m_stampProgress = 0f;

            //Update the session
            AddToSession(GaiaOperation.OperationType.Stamp, "Stamping " + m_stampPreviewImage.name);

            //Start stamping
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                m_updateCoroutine = ApplyStamp();
                StartEditorUpdates();
            }
            else
            {
                StartCoroutine(ApplyStamp());
            }
#else
                StartCoroutine(ApplyStamp());
#endif
        }


        /// <summary>
        /// Cause any active stamp to cancel itself - the tidy up will happen in the enumerator
        /// </summary>
        public void CancelStamp()
        {
            m_cancelStamp = true;
        }

        /// <summary>
        /// Returns true if we are currently in process of stamping
        /// </summary>
        /// <returns>True if stamping, false otherwise</returns>
        public bool IsStamping()
        {
            return (m_stampComplete != true);
        }

        /// <summary>
        /// Update the stamp incase of movement etc
        /// </summary>
        /// <param name="newPosition">New location</param>
        public void UpdateStamp()
        {
            //Enforce stick to ground
            if (m_stickBaseToGround)
            {
                AlignToGround();
            }

            //Update location
            transform.position = new Vector3(m_x, m_y, m_z);

            //Update scales and rotation
            transform.localScale = new Vector3(m_width, m_height, m_width);
            transform.localRotation = Quaternion.AngleAxis(m_rotation, Vector3.up);

            //Update bounds
            m_scanBounds.center = transform.position;
            m_scanBounds.size = new Vector3(
               (float)m_scanWidth * m_scanResolution * m_width,
               (float)m_scanHeight * m_scanResolution * m_height,
               (float)m_scanDepth * m_scanResolution * m_width);

            if (m_stampHM != null)
            {
                m_stampHM.SetBoundsWU(m_scanBounds);
            }

            //Set transform changed to false to stop looped updates in OnDrawGizmos
            transform.hasChanged = false;
        }

        /// <summary>
        /// Align the stamp to ground setting - will need to be followed by an UpdateStamp call
        /// </summary>
        public void AlignToGround()
        {
            //Make sure we have something to work with
            if (m_stampHM == null || !m_stampHM.HasData())
            {
                return;
            }

            //See if we can get the height offset for the terrain, otherwise default to zero
            float terrainHeightOffset = 0f;
            Terrain t = Gaia.TerrainHelper.GetTerrain(transform.position);
            if (t == null)
            {
                t = Terrain.activeTerrain;
            }
            if (t != null)
            {
                terrainHeightOffset = t.transform.position.y;
            }

            //Now work out where we should be with relation to extents
            m_scanBounds.center = transform.position;
            m_scanBounds.size = new Vector3(
               (float)m_scanWidth * m_scanResolution * m_width,
               (float)m_scanHeight * m_scanResolution * m_height,
               (float)m_scanDepth * m_scanResolution * m_width);

            //Now update the height
            if (t == null)
            {
                m_y = terrainHeightOffset + m_scanBounds.extents.y;
            }
            else
            {
                float zero = m_scanBounds.min.y + (m_scanBounds.size.y * m_baseLevel);
                m_y = m_scanBounds.center.y - (zero - terrainHeightOffset);
            }
        }

        /// <summary>
        /// Gte the height range for this stamp
        /// </summary>
        /// <param name="minHeight">Base level for this stamp</param>
        /// <param name="minHeight">Minimum height</param>
        /// <param name="maxHeight">Maximum height</param>
        /// <returns>True if stamp had data, false otherwise</returns>
        public bool GetHeightRange(ref float baseLevel, ref float minHeight, ref float maxHeight)
        {
            if (m_stampHM == null || !m_stampHM.HasData())
            {
                return false;
            }

            baseLevel = m_baseLevel;
            m_stampHM.GetHeightRange(ref minHeight, ref maxHeight);
            return true;
        }

        /// <summary>
        /// Position and fit the stamp perfectly to the terrain - will need to be followed by an UpdateStamp call
        /// </summary>
        public void FitToTerrain()
        {
            Terrain t = Gaia.TerrainHelper.GetTerrain(transform.position);
            if (t == null)
            {
                t = Gaia.TerrainHelper.GetActiveTerrain();
            }
            if (t == null)
            {
                return;
            }
            Bounds b = new Bounds();
            if (Gaia.TerrainHelper.GetTerrainBounds(t, ref b))
            {
                m_height = (b.size.y / 100f) * 2f;
                if (m_stampHM != null && m_stampHM.HasData() != false)
                {
                    m_width = (b.size.x / (float)m_stampHM.Width()) * 10f;
                }
                else
                {
                    m_width = m_height;
                }
                //
                m_height *= 0.25f;
                //
                m_x = b.center.x;
                m_y = b.center.y;
                m_z = b.center.z;
                m_rotation = 0f;
            }
            if (m_stickBaseToGround)
            {
                AlignToGround();
            }
        }

        /// <summary>
        /// Check if the stamp has been fit to the terrain - ignoring height
        /// </summary>
        /// <returns>True if its a match</returns>
        public bool IsFitToTerrain()
        {
            Terrain t = Gaia.TerrainHelper.GetTerrain(transform.position);
            if (t == null)
            {
                t = Terrain.activeTerrain;
            }
            if (t == null || m_stampHM == null || m_stampHM.HasData() == false)
            {
                Debug.LogError("Could not check if fit to terrain - no terrain present");
                return false;
            }

            Bounds b = new Bounds();
            if (TerrainHelper.GetTerrainBounds(t, ref b))
            {
                float width = (b.size.x / (float)m_stampHM.Width()) * 10f;
                float x = b.center.x;
                float z = b.center.z;
                float rotation = 0f;

                if (
                    width != m_width ||
                    x != m_x ||
                    z != m_z ||
                    rotation != m_rotation)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add the operationm to the session manager
        /// </summary>
        /// <param name="opType">The type of operation to add</param>
        public void AddToSession(GaiaOperation.OperationType opType, string opName)
        {
            //Update the session
            GaiaSessionManager sessionMgr = GaiaSessionManager.GetSessionManager();
            if (sessionMgr != null && sessionMgr.IsLocked() != true)
            {
                GaiaOperation op = new GaiaOperation();
                op.m_description = opName;
                op.m_generatedByID = m_stampID;
                op.m_generatedByName = transform.name;
                op.m_generatedByType = this.GetType().ToString();
                op.m_isActive = true;
                op.m_operationDateTime = DateTime.Now.ToString();
                op.m_operationType = opType;
                if (opType == GaiaOperation.OperationType.Stamp)
                {
                    op.m_operationDataJson = new string[1];
                    op.m_operationDataJson[0] = this.SerialiseJson();
                }
                else
                {
                    op.m_operationDataJson = new string[0];
                }
                sessionMgr.AddOperation(op);
            }
        }

        /// <summary>
        /// Serialise this as json
        /// </summary>
        /// <returns></returns>
        public string SerialiseJson()
        {
            //Grab the various paths
#if UNITY_EDITOR
            m_resourcesPath = AssetDatabase.GetAssetPath(m_resources);
#endif

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
            var stamper = this;
            serializer.TryDeserialize<Stamper>(data, ref stamper);
#if UNITY_EDITOR
            stamper.m_resources = GaiaUtils.GetAsset(m_resourcesPath, typeof(Gaia.GaiaResource)) as Gaia.GaiaResource;
            if (m_imageMask != null)
            {
                if (m_imageMask.width == 0 && m_imageMask.height == 0)
                {
                    m_imageMask = null;
                }
            }
#endif
            stamper.LoadStamp();
            stamper.UpdateStamp();
        }

        /// <summary>
        /// Flatten all active terrains
        /// </summary>
        public void FlattenTerrain()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.FlattenTerrain, "Flattening terrain");

            //Get an undo buffer
            m_undoMgr = new GaiaWorldManager(Terrain.activeTerrains);
            m_undoMgr.LoadFromWorld();

            //Flatten the world
            m_redoMgr = new GaiaWorldManager(Terrain.activeTerrains);
            m_redoMgr.FlattenWorld();
            m_redoMgr = null;
        }

        /// <summary>
        /// Smooth all active terrains
        /// </summary>
        public void SmoothTerrain()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.SmoothTerrain, "Smoothing terrain");

            //Get an undo buffer
            m_undoMgr = new GaiaWorldManager(Terrain.activeTerrains);
            m_undoMgr.LoadFromWorld();

            //Flatten the world
            m_redoMgr = new GaiaWorldManager(Terrain.activeTerrains);
            m_redoMgr.SmoothWorld();
            m_redoMgr = null;
        }

        /// <summary>
        /// Clear trees off all actiove terrains
        /// </summary>
        public void ClearTrees()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.ClearTrees, "Clearing terrain trees");
            TerrainHelper.ClearTrees();
        }

        /// <summary>
        /// Clear all the grass off all the terrains
        /// </summary>
        public void ClearDetails()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.ClearDetails, "Clearing terrain details");
            TerrainHelper.ClearDetails();
        }

        #endregion

        #region Preview methods

        /// <summary>
        /// Return true if we have a preview we can use
        /// </summary>
        /// <returns>True if we can preview</returns>
        public bool CanPreview()
        {
            return (m_previewRenderer != null);
        }

        /// <summary>
        /// Get current preview state
        /// </summary>
        /// <returns>Current preview state</returns>
        public bool CurrentPreviewState()
        {
            if (m_previewRenderer != null)
            {
                return m_previewRenderer.enabled;
            }
            return false;
        }

        /// <summary>
        /// Show the preview if possible
        /// </summary>
        public void ShowPreview()
        {
            if (m_previewRenderer != null)
            {
                m_previewRenderer.enabled = true;
            }
        }

        /// <summary>
        /// Hide the preview if possible
        /// </summary>
        public void HidePreview()
        {
            if (m_previewRenderer != null)
            {
                m_previewRenderer.enabled = false;
            }
        }

        /// <summary>
        /// Toggle the preview mesh on and off
        /// </summary>
        public void TogglePreview()
        {
            if (m_previewRenderer != null)
            {
                m_previewRenderer.enabled = !m_previewRenderer.enabled;
            }
        }

        #endregion

        #region Undo / Redo methods

        /// <summary>
        /// Whether or not we can undo an operation. Due to memory constraints only one level of undo is supported.
        /// </summary>
        /// <returns>True if we can undo an operation</returns>
        public bool CanUndo()
        {
            if (m_undoMgr == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create an undo - creating an undo always destroys the redo if one existed
        /// </summary>
        public void CreateUndo()
        {
            //Create new undo manager
            m_undoMgr = new GaiaWorldManager(Terrain.activeTerrains);
            m_undoMgr.LoadFromWorld();

            //And destroy the redo manager
            m_redoMgr = null;
        }

        /// <summary>
        /// Undo a previous operation if possible - create redo so we can redo the undo
        /// </summary>
        public void Undo()
        {
            if (m_undoMgr != null)
            {
                //Update the session
                AddToSession(GaiaOperation.OperationType.StampUndo, "Undoing stamp");

                m_redoMgr = new GaiaWorldManager(Terrain.activeTerrains);
                m_redoMgr.LoadFromWorld();
                m_undoMgr.SaveToWorld(true);
            }
        }

        /// <summary>
        /// True if the previous undo can be redone
        /// </summary>
        /// <returns>True if a redo is possible</returns>
        public bool CanRedo()
        {
            if (m_redoMgr == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Redo a previous operation if possible
        /// </summary>
        public void Redo()
        {
            if (m_redoMgr != null)
            {
                //Update the session
                AddToSession(GaiaOperation.OperationType.StampRedo, "Redoing stamp");

                m_redoMgr.SaveToWorld(true);
                m_redoMgr = null;
            }
        }

        #endregion

        #region Unity Related Methods

        /// <summary>
        /// Called when the stamp is enabled, loads stamp if necessary
        /// </summary>
        void OnEnable()
        {
            //Check for changed feature and load if necessary
            if (m_stampPreviewImage != null)
            {
                LoadStamp();
            }
        }

        /// <summary>
        /// Called when app starts
        /// </summary>
        void Start()
        {
            //Hide stamp preview mesh at runtime
            if (Application.isPlaying)
            {
                HidePreview();
            }
        }

        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        //Stop editor updates
        public void StopEditorUpdates()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        /// <summary>
        /// This is executed only in the editor - using it to simulate co-routine execution and update execution
        /// </summary>
        void EditorUpdate()
        {
#if UNITY_EDITOR
            if (m_updateCoroutine == null)
            {
                StopEditorUpdates();
                return;
            }
            else
            {
                if (EditorWindow.mouseOverWindow != null)
                {
                    m_updateTimeAllowed = 1 / 30f;
                }
                else
                {
                    m_updateTimeAllowed = 1 / 2f;
                }
                m_updateCoroutine.MoveNext();
            }
#endif
        }

        /// <summary>
        /// Draw gizmos when selected
        /// </summary>
        void OnDrawGizmosSelected()
        {
            DrawGizmos(true);
        }

        /// <summary>
        /// Draw gizmos when not selected
        /// </summary>
        void OnDrawGizmos()
        {
            DrawGizmos(false);
        }

        /// <summary>
        /// Draw the gizmos
        /// </summary>
        /// <param name="isSelected"></param>
        void DrawGizmos(bool isSelected)
        {
            //Drop out if no stamp loaded - not necessarily what i want
            if (m_stampPreviewImage == null)
            {
                return;
            }

            //Update the stamp if necessary - should only ever happen when user moves stamp via gizmo
            if (transform.hasChanged)
            {
                m_x = transform.position.x;
                m_y = transform.position.y;
                m_z = transform.position.z;
                m_rotation = transform.localEulerAngles.y;
                if (transform.localScale.x != m_width || transform.localScale.z != m_width)
                {
                    float deltaX = Mathf.Abs(transform.localScale.x - m_width);
                    float deltaZ = Mathf.Abs(transform.localScale.z - m_width);
                    if (deltaX > deltaZ)
                    {
                        if (transform.localScale.x > 0f)
                        {
                            m_width = transform.localScale.x;
                        }
                    }
                    else
                    {
                        if (transform.localScale.z > 0f)
                        {
                            m_width = transform.localScale.z;
                        }
                    }
                }
                if (transform.localScale.y != m_height)
                {
                    if (transform.localScale.y > 0f)
                    {
                        m_height = transform.localScale.y;
                    }
                }
                UpdateStamp();
#if UNITY_EDITOR
                SceneView.RepaintAll();
#endif
            }

            //Determine whether to drop out
            if (!isSelected && !m_alwaysShow)
            {
                return;
            }

            //Now draw the gizmos

            //Base
            if (m_showBase)
            {
                Bounds bounds = new Bounds();
                if (TerrainHelper.GetTerrainBounds(transform.position, ref bounds) == true)
                {
                    bounds.center = new Vector3(bounds.center.x, m_scanBounds.min.y + (m_scanBounds.size.y * m_baseLevel), bounds.center.z);
                    bounds.size = new Vector3(bounds.size.x, 0.05f, bounds.size.z);
                    Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, Color.yellow.a / 2f);
                    Gizmos.DrawCube(bounds.center, bounds.size);
                }
            }

            //Water
            if (m_resources != null)
            {
                m_seaLevel = m_resources.m_seaLevel;
            }
            if (m_showSeaLevel)
            {
                Bounds bounds = new Bounds();
                if (TerrainHelper.GetTerrainBounds(transform.position, ref bounds) == true)
                {
                    bounds.center = new Vector3(bounds.center.x, m_seaLevel, bounds.center.z);
                    bounds.size = new Vector3(bounds.size.x, 0.05f, bounds.size.z);
                    if (isSelected)
                    {
                        Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, Color.blue.a / 2f);
                        Gizmos.DrawCube(bounds.center, bounds.size);
                    }
                    else
                    {
                        Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, Color.blue.a / 4f);
                        Gizmos.DrawCube(bounds.center, bounds.size);
                    }
                }
            }

            //Rulers
            if (m_showRulers)
            {
                DrawRulers();
            }

            //Rotation n size
            Matrix4x4 currMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 origSize = new Vector3(
                (float)m_scanWidth * m_scanResolution,
                (float)m_scanHeight * m_scanResolution,
                (float)m_scanDepth * m_scanResolution);
            Gizmos.color = new Color(m_gizmoColour.r, m_gizmoColour.g, m_gizmoColour.b, m_gizmoColour.a / 2f);
            Gizmos.DrawWireCube(Vector3.zero, origSize);
            Gizmos.matrix = currMatrix;

            //Terrain bounds
            Terrain t = Gaia.TerrainHelper.GetTerrain(transform.position);
            if (t != null)
            {
                Gizmos.color = Color.white;
                Bounds b = new Bounds();
                Gaia.TerrainHelper.GetTerrainBounds(t, ref b);
                Gizmos.DrawWireCube(b.center, b.size);
            }
        }

        /// <summary>
        /// Draw the rulers
        /// </summary>
        void DrawRulers()
        {
#if UNITY_EDITOR
            if (m_showRulers)
            {
                Gizmos.color = Color.green;

                //Ruler gizmos
                int ticks;
                float tickOffset;
                float tickInterval = 100f;
                float vertRulerSize = m_scanBounds.max.y - m_scanBounds.min.y;
                float horizRulerSize = m_scanBounds.max.x - m_scanBounds.min.x;
                Vector3 startPosition;
                Vector3 endPosition;
                Vector3 labelPosition;

                //Vertical ruler
                startPosition = m_scanBounds.center;
                startPosition.y = m_scanBounds.min.y;
                endPosition = m_scanBounds.center;
                endPosition.y = m_scanBounds.max.y;
                labelPosition = startPosition;
                labelPosition.x += 5f;
                labelPosition.y += 2f;
                Gizmos.DrawLine(startPosition, endPosition);

                ticks = Mathf.RoundToInt(vertRulerSize / tickInterval);
                tickOffset = vertRulerSize / (float)ticks;
                for (int i = 0; i <= ticks; i++)
                {
                    Handles.Label(labelPosition, string.Format("{0:0m}", labelPosition.y));
                    labelPosition.y += tickOffset;
                }

                //Horizontal ruler - x axis
                startPosition = m_scanBounds.center;
                startPosition.x = m_scanBounds.min.x;
                endPosition = m_scanBounds.center;
                endPosition.x = m_scanBounds.max.x;
                labelPosition = startPosition;
                labelPosition.x += 5f;
                labelPosition.y += 2f;
                Gizmos.DrawLine(startPosition, endPosition);

                ticks = Mathf.RoundToInt(horizRulerSize / tickInterval);
                tickOffset = horizRulerSize / (float)ticks;
                for (int i = 0; i <= ticks; i++)
                {
                    Handles.Label(labelPosition, string.Format("{0:0m}", labelPosition.x));
                    labelPosition.x += tickOffset;
                }
            }
#endif
        }

        #endregion

        #region Private worker methods

        /// <summary>
        /// Apply the stamp to the terrain
        /// </summary>
        public IEnumerator ApplyStamp()
        {
            //Update the stamp calculations
            UpdateStamp();

            //World manager - create and load from existing terrains
            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            mgr.LoadFromWorld();

            //Check to see if we loaded anything and exit if we didnt
            if (mgr.TileCount == 0)
            {
                Debug.LogError("Can not stamp without a terrain present!");
                //Signal complete
                m_stampProgress = 0f;
                m_stampComplete = true;
                m_updateCoroutine = null;
                //Exit
                yield break;
            }

            //Now create another one as an undo
            CreateUndo();

            //Load any masks
            if (m_areaMaskMode != GaiaConstants.ImageFitnessFilterMode.None)
            {
                if (!LoadImageMask())
                {
                    //Signal complete
                    m_stampProgress = 0f;
                    m_stampComplete = true;
                    m_updateCoroutine = null;
                    //Exit
                    yield break;
                }
            }

            //Rotation
            Vector3 rotation = new Vector3(0f, transform.localRotation.eulerAngles.y, 0f);

            //Work out bounds of new rotated and scaled scan map in original resolution of the terrain
            Bounds origSmBoundsWU = m_stampHM.GetBoundsWU();

            //Get new bounds taking rotation into account
            Bounds newSmBoundsWU = new Bounds();
            newSmBoundsWU.center = origSmBoundsWU.center;
            newSmBoundsWU.Encapsulate(RotatePointAroundPivot(new Vector3(origSmBoundsWU.min.x, origSmBoundsWU.center.y, origSmBoundsWU.min.z), origSmBoundsWU.center, rotation));
            newSmBoundsWU.Encapsulate(RotatePointAroundPivot(new Vector3(origSmBoundsWU.min.x, origSmBoundsWU.center.y, origSmBoundsWU.max.z), origSmBoundsWU.center, rotation));
            newSmBoundsWU.Encapsulate(RotatePointAroundPivot(new Vector3(origSmBoundsWU.max.x, origSmBoundsWU.center.y, origSmBoundsWU.min.z), origSmBoundsWU.center, rotation));
            newSmBoundsWU.Encapsulate(RotatePointAroundPivot(new Vector3(origSmBoundsWU.max.x, origSmBoundsWU.center.y, origSmBoundsWU.max.z), origSmBoundsWU.center, rotation));

            //Calculate new sm size based on conversion to terrain units and round up
            Vector3 newSmSizeTU = mgr.Ceil(mgr.WUtoTU(newSmBoundsWU.size));

            //Now rotate and scale the original into the new
            Vector3 position;
            Vector3 pivot = new Vector3(0.5f, 0f, 0.5f);
            float origSmXNU, origSmZNU;
            int newSmMaxX = (int)newSmSizeTU.x;
            int newSmMaxZ = (int)newSmSizeTU.z;
            float newSmXtoNU = 1f / newSmSizeTU.x;
            float newSmZtoNU = 1f / newSmSizeTU.z;
            float xNewSMtoOrigSMScale = newSmBoundsWU.size.x / origSmBoundsWU.size.x;
            float zNewSMtoOrigSMScale = newSmBoundsWU.size.x / origSmBoundsWU.size.z;

            //Need to offset due to scaling caused by rotation
            float scaleOffsetX = 0.5f * ((origSmBoundsWU.size.x - newSmBoundsWU.size.x) / origSmBoundsWU.size.x);
            float scaleOffsetZ = 0.5f * ((origSmBoundsWU.size.z - newSmBoundsWU.size.x) / origSmBoundsWU.size.z);

            //Timing and progress control
            float newTime, stepTime;
            float currentTime = Time.realtimeSinceStartup;
            float accumulatedTime = 0.0f;
            int currChecks = 0;
            int totalChecks = newSmMaxX * newSmMaxZ;

            //Work out offset back into global space
            //Vector3 terrainCentreTU = mgr.WorldBoundsTU.center;
            Vector3 globalCentreTU = mgr.WUtoTU(transform.position);
            Vector3 globalOffsetTU = globalCentreTU - (newSmSizeTU * 0.5f);

            //This assumes a zero centered terrain - just allow for terrain offset

            Vector3 globalPositionTU = Vector3.one;
            globalPositionTU.y = globalCentreTU.y;

            //Height calcs
            float smHeightRaw, smHeightAdj, terrainHeight, newHeight, distance, strength;

            //Source to terrain height conversion
            float smToOrigHeightConversion = origSmBoundsWU.size.y / mgr.WorldBoundsWU.size.y;
            float smHeightOffset = (origSmBoundsWU.min.y - mgr.WorldBoundsWU.min.y) / mgr.WorldBoundsWU.size.y;
            float stencilHeightNU = m_stencilHeight / mgr.WorldBoundsWU.size.y;

            double rotationCosTheta = System.Math.Cos((System.Math.PI / 180) * rotation.y);
            double rotationSinTheta = System.Math.Sin((System.Math.PI / 180) * rotation.y);

            RotationProducts xRotationProducts = new RotationProducts();
            RotationProducts[] zRotationProductCache = new RotationProducts[newSmMaxZ];

            float xNU = 0f;
            float zNU = 0f;

            //Now apply scale and rotation to new scan map
            for (int x = 0; x < newSmMaxX; x++)
            {

                xNU = x * newSmXtoNU - pivot.x;
                xRotationProducts.sinTheta = xNU * rotationSinTheta;
                xRotationProducts.cosTheta = xNU * rotationCosTheta;

                for (int z = 0; z < newSmMaxZ; z++)
                {

                    //Update progress and yield periodiocally
                    m_stampProgress = (float)(currChecks++) / (float)totalChecks;
                    newTime = Time.realtimeSinceStartup;
                    stepTime = newTime - currentTime;
                    currentTime = newTime;
                    accumulatedTime += stepTime;
                    if (accumulatedTime > m_updateTimeAllowed)
                    {
                        accumulatedTime = 0f;
                        yield return null;
                    }

                    //Check for cancelation
                    if (m_cancelStamp == true)
                    {
                        break;
                    }

                    //Update global position and check terrain bounds
                    globalPositionTU.x = z + globalOffsetTU.z;
                    globalPositionTU.z = x + globalOffsetTU.x;
                    if (!mgr.InBoundsTU(globalPositionTU))
                    {
                        continue;
                    }

                    //Rotate normal location around center
                    if (zRotationProductCache[z] == null)
                    {
                        zNU = z * newSmZtoNU - pivot.z;
                        zRotationProductCache[z] = new RotationProducts {
                            sinTheta = zNU * rotationSinTheta,
                            cosTheta = zNU * rotationCosTheta,
                        };
                    }
                    position.x = (float)(xRotationProducts.cosTheta - zRotationProductCache[z].sinTheta + pivot.x);
                    position.z = (float)(xRotationProducts.sinTheta + zRotationProductCache[z].cosTheta + pivot.z);

                    //Get new location taking into scaling due to rotation, and new offset due to scaling
                    origSmXNU = (position.x * xNewSMtoOrigSMScale) + scaleOffsetX;
                    origSmZNU = (position.z * zNewSMtoOrigSMScale) + scaleOffsetZ;

                    //Check for rotation bounds
                    if (origSmXNU < 0f || origSmXNU > 1f)
                    {
                        continue;
                    }
                    if (origSmZNU < 0f || origSmZNU > 1f)
                    {
                        continue;
                    }

                    //Calculate and apply height
                    distance = GaiaUtils.Math_Distance(origSmXNU, origSmZNU, pivot.x, pivot.z) * 2f;
                    strength = m_distanceMask.Evaluate(distance);
                    if (m_areaMaskMode != GaiaConstants.ImageFitnessFilterMode.None && m_imageMaskHM != null)
                    {
                        strength *= m_imageMaskHM[origSmXNU, origSmZNU];
                    }

                    smHeightRaw = m_heightModifier.Evaluate(m_stampHM[origSmXNU, origSmZNU]);
                    if (m_stampOperation != GaiaConstants.FeatureOperation.StencilHeight)
                    {
                        smHeightAdj = (smHeightOffset + (smHeightRaw * smToOrigHeightConversion));
                    }
                    else
                    {
                        smHeightAdj = smHeightRaw;
                    }
                    terrainHeight = mgr.GetHeightTU(globalPositionTU);
                    newHeight = CalculateHeight(terrainHeight, smHeightRaw, smHeightAdj, stencilHeightNU, strength);
                    mgr.SetHeightTU(globalPositionTU, Mathf.Clamp01(newHeight));
                }
            }

            //Apply the new heightmap to the terrain
            if (!m_cancelStamp)
            {
                mgr.SaveToWorld();
            }
            else
            {
                //All bets are off when you cancel
                m_undoMgr = null;
                m_redoMgr = null;
            }

            //Signal complete
            m_stampProgress = 0f;
            m_stampComplete = true;
            m_updateCoroutine = null;
        }

        /// <summary>
        /// Generate the preview mesh
        /// </summary>
        private void GeneratePreviewMesh()
        {
            if (m_previewMaterial == null)
            {
#if UNITY_EDITOR
                string matPath = GaiaUtils.GetAssetPath("GaiaStamperMaterial.mat");
                if (!string.IsNullOrEmpty(matPath))
                {
                    m_previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }
                else
                {
                    m_previewMaterial = new Material(Shader.Find("Diffuse"));
                    m_previewMaterial.color = Color.white;
                    if (Terrain.activeTerrain != null)
                    {
                        var splatPrototypes = GaiaSplatPrototype.GetGaiaSplatPrototypes(Terrain.activeTerrain);
                        if (splatPrototypes.Length > 0)
                        {
                            Texture2D oldTex;
                            if (splatPrototypes.Length == 4) //Defaults to cliff
                            {
                                oldTex = splatPrototypes[3].texture;
                            }
                            else
                            {
                                oldTex = splatPrototypes[0].texture;
                            }
                            GaiaUtils.MakeTextureReadable(oldTex);
                            Texture2D newTex = new Texture2D(oldTex.width, oldTex.height, TextureFormat.ARGB32, true);
                            newTex.SetPixels32(oldTex.GetPixels32());
                            newTex.wrapMode = TextureWrapMode.Repeat;
                            newTex.Apply();
                            m_previewMaterial.mainTexture = newTex;
                            m_previewMaterial.mainTextureScale = new Vector2(30f, 30f);
                        }
                    }
                    m_previewMaterial.hideFlags = HideFlags.HideInInspector;
                    m_previewMaterial.name = "StamperMaterial";
                }
#else
                m_previewMaterial = new Material(Shader.Find("Diffuse"));
                m_previewMaterial.color = Color.white;
                if (Terrain.activeTerrain != null)
                {
                    var splatPrototypes = GaiaSplatPrototype.GetGaiaSplatPrototypes(Terrain.activeTerrain);
                    if (splatPrototypes.Length > 0)
                    {
                        Texture2D oldTex;
                        if (splatPrototypes.Length == 4) //Defaults to cliff
                        {
                            oldTex = splatPrototypes[3].texture;
                        }
                        else
                        {
                            oldTex = splatPrototypes[0].texture;
                        }
                        GaiaUtils.MakeTextureReadable(oldTex);
                        Texture2D newTex = new Texture2D(oldTex.width, oldTex.height, TextureFormat.ARGB32, true);
                        newTex.SetPixels32(oldTex.GetPixels32());
                        newTex.wrapMode = TextureWrapMode.Repeat;
                        newTex.Apply();
                        m_previewMaterial.mainTexture = newTex;
                        m_previewMaterial.mainTextureScale = new Vector2(30f, 30f);
                    }
                }
                m_previewMaterial.hideFlags = HideFlags.HideInInspector;
                m_previewMaterial.name = "StamperMaterial";
#endif
                    }

            m_previewFilter = GetComponent<MeshFilter>();
            if (m_previewFilter == null)
            {
                this.gameObject.AddComponent<MeshFilter>();
                m_previewFilter = GetComponent<MeshFilter>();
                m_previewFilter.hideFlags = HideFlags.HideInInspector;
            }

            m_previewRenderer = GetComponent<MeshRenderer>();
            if (m_previewRenderer == null)
            {
                this.gameObject.AddComponent<MeshRenderer>();
                m_previewRenderer = GetComponent<MeshRenderer>();
                m_previewRenderer.hideFlags = HideFlags.HideInInspector;
            }

            m_previewRenderer.sharedMaterial = m_previewMaterial;
            Vector3 meshSize = new Vector3((float)m_scanWidth * m_scanResolution, (float)m_scanHeight * m_scanResolution, (float)m_scanDepth * m_scanResolution);
            m_previewFilter.mesh = GaiaUtils.CreateMesh(m_stampHM.Heights(), meshSize);
        }

        /// <summary>
        /// Load the image mask if one was specified, or calculate it from noise
        /// </summary>
        private bool LoadImageMask()
        {
            //Kill old image height map
            m_imageMaskHM = null;

            //Check mode & exit 
            if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.None)
            {
                return false;
            }

            //Load the supplied image
            if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageRedChannel || m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreenChannel ||
                m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageBlueChannel || m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageAlphaChannel ||
                m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreyScale)
            {
                if (m_imageMask == null)
                {
                    Debug.LogError("You requested an image mask but did not supply one. Please select mask texture.");
                    return false;
                }

                //Check the image rw
                GaiaUtils.MakeTextureReadable(m_imageMask);

                //Make it uncompressed
                GaiaUtils.MakeTextureUncompressed(m_imageMask);

                //Load the image
                m_imageMaskHM = new HeightMap(m_imageMask.width, m_imageMask.height);
                for (int x = 0; x < m_imageMaskHM.Width(); x++)
                {
                    for (int z = 0; z < m_imageMaskHM.Depth(); z++)
                    {
                        switch (m_areaMaskMode)
                        {
                            case GaiaConstants.ImageFitnessFilterMode.ImageGreyScale:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).grayscale;
                                break;
                            case GaiaConstants.ImageFitnessFilterMode.ImageRedChannel:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).r;
                                break;
                            case GaiaConstants.ImageFitnessFilterMode.ImageGreenChannel:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).g;
                                break;
                            case GaiaConstants.ImageFitnessFilterMode.ImageBlueChannel:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).b;
                                break;
                            case GaiaConstants.ImageFitnessFilterMode.ImageAlphaChannel:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).a;
                                break;
                        }
                    }
                }
            }
            else if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.PerlinNoise || m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.RidgedNoise ||
                m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.BillowNoise)
            {
                int width = 2048;
                int depth = 2048;

                Terrain t = Gaia.TerrainHelper.GetTerrain(transform.position);
                if (t == null)
                {
                    t = Terrain.activeTerrain;
                }
                if (t != null)
                {
                    width = t.terrainData.heightmapResolution;
                    depth = t.terrainData.heightmapResolution;
                }

                m_imageMaskHM = new HeightMap(width, depth);

                //Create the noise generator
                Gaia.FractalGenerator noiseGenerator = new FractalGenerator();
                noiseGenerator.Seed = m_noiseMaskSeed;
                noiseGenerator.Octaves = m_noiseMaskOctaves;
                noiseGenerator.Persistence = m_noiseMaskPersistence;
                noiseGenerator.Frequency = m_noiseMaskFrequency;
                noiseGenerator.Lacunarity = m_noiseMaskLacunarity;
                if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.PerlinNoise)
                {
                    noiseGenerator.FractalType = FractalGenerator.Fractals.Perlin;
                }
                else if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.RidgedNoise)
                {
                    noiseGenerator.FractalType = FractalGenerator.Fractals.RidgeMulti;
                }
                else if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.BillowNoise)
                {
                    noiseGenerator.FractalType = FractalGenerator.Fractals.Billow;
                }

                float zoom = 1f / m_noiseZoom;

                //Now fill it with the selected noise
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        m_imageMaskHM[x, z] = noiseGenerator.GetValue((float)(x * zoom), (float)(z * zoom));
                    }
                }
            }
            else
            {
                //Or get a new one

                //Grab the terrain 
                Terrain t = Gaia.TerrainHelper.GetTerrain(transform.position);
                if (t == null)
                {
                    t = Terrain.activeTerrain;
                }
                if (t == null)
                {
                    Debug.LogError("You requested an terrain texture mask but there is no terrain.");
                    return false;
                }

                var splatPrototypes = GaiaSplatPrototype.GetGaiaSplatPrototypes(t);

                switch (m_areaMaskMode)
                {
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture0:
                        if (splatPrototypes.Length < 1)
                        {
                            Debug.LogError("You requested an terrain texture mask 0 but there is no active texture in slot 0.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 0);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture1:
                        if (splatPrototypes.Length < 2)
                        {
                            Debug.LogError("You requested an terrain texture mask 1 but there is no active texture in slot 1.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 1);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture2:
                        if (splatPrototypes.Length < 3)
                        {
                            Debug.LogError("You requested an terrain texture mask 2 but there is no active texture in slot 2.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 2);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture3:
                        if (splatPrototypes.Length < 4)
                        {
                            Debug.LogError("You requested an terrain texture mask 3 but there is no active texture in slot 3.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 3);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture4:
                        if (splatPrototypes.Length < 5)
                        {
                            Debug.LogError("You requested an terrain texture mask 4 but there is no active texture in slot 4.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 4);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture5:
                        if (splatPrototypes.Length < 6)
                        {
                            Debug.LogError("You requested an terrain texture mask 5 but there is no active texture in slot 5.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 5);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture6:
                        if (splatPrototypes.Length < 7)
                        {
                            Debug.LogError("You requested an terrain texture mask 6 but there is no active texture in slot 6.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 6);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture7:
                        if (splatPrototypes.Length < 8)
                        {
                            Debug.LogError("You requested an terrain texture mask 7 but there is no active texture in slot 7.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 7);
                        break;
                }

                //It came from terrain so flip it
                m_imageMaskHM.Flip();
            }

            //Because images are noisy, smooth it
            if (m_imageMaskSmoothIterations > 0)
            {
                m_imageMaskHM.Smooth(m_imageMaskSmoothIterations);
            }

            //Flip it
            if (m_imageMaskFlip == true)
            {
                m_imageMaskHM.Flip();
            }

            //Normalise it if necessary
            if (m_imageMaskNormalise == true)
            {
                m_imageMaskHM.Normalise();
            }

            //Invert it if necessessary
            if (m_imageMaskInvert == true)
            {
                m_imageMaskHM.Invert();
            }

            return true;
        }

        /// <summary>
        /// Calculate the height to apply to the location supplied
        /// </summary>
        /// <param name="terrainHeight">The terrain height at this location</param>
        /// <param name="smHeightRaw">The raw unadsjusted source map height at this location</param>
        /// <param name="smHeightAdj">The adsjusted source map height at this location</param>
        /// <param name="stencilHeightNU">The stencil height in normal units</param>
        /// <param name="strength">The strength of the effect 0 - no effect - 1 - full effect</param>
        /// <returns>New height</returns>
        private float CalculateHeight(float terrainHeight, float smHeightRaw, float smHeightAdj, float stencilHeightNU, float strength)
        {
            float tmpHeight = 0f;
            float heightDiff = 0f;

            //Check for the base
            if (m_drawStampBase != true)
            {
                if (smHeightRaw < m_baseLevel)
                {
                    return terrainHeight;
                }
            }

            switch (m_stampOperation)
            {
                case GaiaConstants.FeatureOperation.RaiseHeight:
                    {
                        if (smHeightAdj > terrainHeight)
                        {
                            heightDiff = (smHeightAdj - terrainHeight) * strength;
                            terrainHeight += heightDiff;
                        }
                    }
                    break;
                case GaiaConstants.FeatureOperation.BlendHeight:
                    {
                        tmpHeight = (m_blendStrength * smHeightAdj) + ((1f - m_blendStrength) * terrainHeight);
                        heightDiff = (tmpHeight - terrainHeight) * strength;
                        terrainHeight += heightDiff;
                    }
                    break;
                case GaiaConstants.FeatureOperation.DifferenceHeight:
                    {
                        tmpHeight = Mathf.Abs(smHeightAdj - terrainHeight);
                        heightDiff = (tmpHeight - terrainHeight) * strength;
                        terrainHeight += heightDiff;
                    }
                    break;
                /*
                case Constants.FeatureOperation.OverlayHeight:
                    {
                        if (terrainHeight < 0.5f)
                        {
                            tmpHeight = 2f * terrainHeight * smHeight;
                        }
                        else
                        {
                            tmpHeight = 1f - (2f * (1f - terrainHeight) * (1f - smHeight));
                        }
                        heightDiff = (tmpHeight - terrainHeight) * strength;
                        terrainHeight += heightDiff;
                    }
                    break;
                case Constants.FeatureOperation.ScreenHeight:
                    {
                        tmpHeight = 1f - ((1f - terrainHeight) * (1f - smHeight));
                        heightDiff = (tmpHeight - terrainHeight) * strength;
                        terrainHeight += heightDiff;
                    }
                    break;
                 */
                case GaiaConstants.FeatureOperation.StencilHeight:
                    {
                        tmpHeight = terrainHeight + (smHeightAdj * stencilHeightNU);
                        heightDiff = (tmpHeight - terrainHeight) * strength;
                        terrainHeight += heightDiff;
                    }
                    break;
                case GaiaConstants.FeatureOperation.LowerHeight:
                    {
                        if (smHeightAdj < terrainHeight)
                        {
                            heightDiff = (terrainHeight - smHeightAdj) * strength;
                            terrainHeight -= heightDiff;
                        }
                    }
                    break;
            }
            return terrainHeight;
        }

        /// <summary>
        /// Rotate the point around the pivot - used to handle rotation
        /// </summary>
        /// <param name="point">Point to move</param>
        /// <param name="pivot">Pivot</param>
        /// <param name="angle">Angle to pivot</param>
        /// <returns></returns>
        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
        {
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(angle) * dir;
            point = dir + pivot;
            return point;
        }

#endregion

    }

    class RotationProducts
    {
        public double sinTheta;
        public double cosTheta;
    }
}