using UnityEngine;
using System.IO;
using System;

namespace Gaia
{
    /// <summary>
    /// Scanning system - creates stamps
    /// </summary>
    public class Scanner : MonoBehaviour
    {
        [Tooltip("The name of the stamp as it will be stored in the project. Initally based on the file name.")]
        public string m_featureName = string.Format("{0}",DateTime.Now);
        [Tooltip("The type of stamp, also controls which directory the stamp will be loaded into.")]
        public Gaia.GaiaConstants.FeatureType m_featureType = GaiaConstants.FeatureType.Mountains;
        [Range(0f, 1f), Tooltip("Base level for this stamp, used by stamper to cut off all heights below the base. It is the highest point of the stamp around its edges.")]
        public float m_baseLevel = 0f;
        [HideInInspector]
        [Range(0.1f, 1f), Tooltip("Scan resolution in meters. Leave this at smaller values for higher quality scans.")]
        public float m_scanResolution = 0.1f; //Every 10 cm
        [Tooltip("The material that will be used to display the scan preview. This is just for visualisation and will not affect the scan.")]
        public Material m_previewMaterial;

        private HeightMap m_scanMap;
        private Bounds m_scanBounds;
        private int m_scanWidth = 1;
        private int m_scanDepth = 1;
        private int m_scanHeight = 500;
        private Vector3 m_groundOffset = Vector3.zero;
        private Vector3 m_groundSize = Vector3.zero;

        void OnEnable()
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null)
            {
                mf = gameObject.AddComponent<MeshFilter>();
            }
            mf.hideFlags = HideFlags.HideInInspector;
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr == null)
            {
                mr = gameObject.AddComponent<MeshRenderer>();
            }
            mr.hideFlags = HideFlags.HideInInspector;
        }

        /// <summary>
        /// Knock ourselves out if we happen to be left on in play mode
        /// </summary>
        void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Reset the scanner
        /// </summary>
        public void Reset()
        {
            m_featureName = "";
            m_scanMap = null;
            m_scanBounds = new Bounds(transform.position, Vector3.one * 10f);
            m_scanWidth = m_scanDepth = m_scanHeight = 0;
            m_scanResolution = 0.1f;
            m_baseLevel = 0f;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Load the raw file at the path given
        /// </summary>
        /// <param name="path">Full path of the raw file</param>
        public void LoadRawFile(string path, GaiaConstants.RawByteOrder byteOrder, ref GaiaConstants.RawBitDepth bitDepth, ref int resolution)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Must supply a valid path. Raw load Aborted!");
            }

            //Clear out the old
            Reset();

            //Load up the new
            m_featureName = Path.GetFileNameWithoutExtension(path);
            m_scanMap = new HeightMap();
            m_scanMap.LoadFromRawFile(path, byteOrder, ref bitDepth, ref resolution);
            if (m_scanMap.HasData() == false)
            {
                Debug.LogError("Unable to load raw file. Raw load aborted.");
                return;
            }

            m_scanWidth = m_scanMap.Width();
            m_scanDepth = m_scanMap.Depth();
            m_scanHeight = m_scanWidth / 2;
            m_scanResolution = 0.1f;
            m_scanBounds = new Bounds(transform.position, new Vector3(m_scanWidth * m_scanResolution, m_scanWidth * m_scanResolution * 0.4f, m_scanDepth * m_scanResolution));
            m_baseLevel = m_scanMap.GetBaseLevel();

            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null)
            {
                mf = gameObject.AddComponent<MeshFilter>();
                mf.hideFlags = HideFlags.HideInInspector;
            }
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr == null)
            {
                mr = gameObject.AddComponent<MeshRenderer>();
                mr.hideFlags = HideFlags.HideInInspector;
            }
            mf.mesh = Gaia.GaiaUtils.CreateMesh(m_scanMap.Heights(), m_scanBounds.size);
            if (m_previewMaterial != null)
            {
                m_previewMaterial.hideFlags = HideFlags.HideInInspector;
                mr.sharedMaterial = m_previewMaterial;
            }
        }

        /// <summary>
        /// Load the texture file provided
        /// </summary>
        /// <param name="texture">Texture file to load</param>
        public void LoadTextureFile(Texture2D texture)
        {
            //Check not null
            if (texture == null)
            {
                Debug.LogError("Must supply a valid texture! Texture load aborted.");
                return;
            }

            //Clear out the old
            Reset();

            //Load up the new
            m_featureName = texture.name;

            m_scanMap = new UnityHeightMap(texture);
            if (m_scanMap.HasData() == false)
            {
                Debug.LogError("Unable to load Texture file. Texture load aborted.");
                return;
            }

            m_scanWidth = m_scanMap.Width();
            m_scanDepth = m_scanMap.Depth();
            m_scanHeight = m_scanWidth / 2;
            m_scanResolution = 0.1f;
            m_scanBounds = new Bounds(transform.position, new Vector3(m_scanWidth * m_scanResolution, m_scanWidth * m_scanResolution * 0.4f, m_scanDepth * m_scanResolution));
            m_baseLevel = m_scanMap.GetBaseLevel();

            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null)
            {
                mf = gameObject.AddComponent<MeshFilter>();
                mf.hideFlags = HideFlags.HideInInspector;
            }
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr == null)
            {
                mr = gameObject.AddComponent<MeshRenderer>();
                mr.hideFlags = HideFlags.HideInInspector;
            }
            mf.mesh = Gaia.GaiaUtils.CreateMesh(m_scanMap.Heights(), m_scanBounds.size);
            if (m_previewMaterial != null)
            {
                m_previewMaterial.hideFlags = HideFlags.HideInInspector;
                mr.sharedMaterial = m_previewMaterial;
            }
        }

        /// <summary>
        /// Load the terrain provided
        /// </summary>
        /// <param name="texture">Terrain to load</param>
        public void LoadTerain(Terrain terrain)
        {
            //Check not null
            if (terrain == null)
            {
                Debug.LogError("Must supply a valid terrain! Terrain load aborted.");
                return;
            }

            //Clear out the old
            Reset();

            //Load up the new
            m_featureName = terrain.name;

            m_scanMap = new UnityHeightMap(terrain);
            if (m_scanMap.HasData() == false)
            {
                Debug.LogError("Unable to load terrain file. Terrain load aborted.");
                return;
            }

            m_scanMap.Flip(); //Undo unity terrain shenannigans

            m_scanWidth = m_scanMap.Width();
            m_scanDepth = m_scanMap.Depth();
            m_scanHeight = (int)terrain.terrainData.size.y;
            m_scanResolution = 0.1f;
            m_scanBounds = new Bounds(transform.position, new Vector3(m_scanWidth * m_scanResolution, m_scanWidth * m_scanResolution * 0.4f, m_scanDepth * m_scanResolution));
            m_baseLevel = m_scanMap.GetBaseLevel();

            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null)
            {
                mf = gameObject.AddComponent<MeshFilter>();
                mf.hideFlags = HideFlags.HideInInspector;
            }
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr == null)
            {
                mr = gameObject.AddComponent<MeshRenderer>();
                mr.hideFlags = HideFlags.HideInInspector;
            }
            mf.mesh = Gaia.GaiaUtils.CreateMesh(m_scanMap.Heights(), m_scanBounds.size);
            if (m_previewMaterial != null)
            {
                m_previewMaterial.hideFlags = HideFlags.HideInInspector;
                mr.sharedMaterial = m_previewMaterial;
            }
        }

        /// <summary>
        /// Load the object provided
        /// </summary>
        /// <param name="go">Terrain to load</param>
        public void LoadGameObject(GameObject go)
        {
            //Check not null
            if (go == null)
            {
                Debug.LogError("Must supply a valid game object! GameObject load aborted.");
                return;
            }

            //Clear out the old
            Reset();

            //Load up the new
            m_featureName = go.name;

            //Duplicate the object
            GameObject workingGo = GameObject.Instantiate(go);

            workingGo.transform.position = transform.position;
            workingGo.transform.localRotation = Quaternion.identity;
            workingGo.transform.localScale = Vector3.one;

            //Delete any old colliders
            Collider[] colliders = workingGo.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                DestroyImmediate(c);
            }

            //Now add mesh colliders to all active game objects for the most accurate possible scanning
            Transform[] transforms = workingGo.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.AddComponent<MeshCollider>();
                }
            }

            //Calculate bounds
            m_scanBounds.center = workingGo.transform.position;
            m_scanBounds.size = Vector3.zero;
            foreach (MeshCollider c in workingGo.GetComponentsInChildren<MeshCollider>())
            {
                m_scanBounds.Encapsulate(c.bounds);
            }

            //Update scan array details - dont need to allocate mem until we scan
            m_scanWidth = (int)(Mathf.Ceil(m_scanBounds.size.x * (1f / m_scanResolution)));
            m_scanHeight = (int)(Mathf.Ceil(m_scanBounds.size.y * (1f / m_scanResolution)));
            m_scanDepth = (int)(Mathf.Ceil(m_scanBounds.size.z * (1f / m_scanResolution)));

            //Now scan the object
            m_scanMap = new HeightMap(m_scanWidth, m_scanDepth);
            Vector3 scanMin = m_scanBounds.min;
            Vector3 scanPos = scanMin;
            scanPos.y = m_scanBounds.max.y;
            RaycastHit scanHit;

            //Perform the scan - only need to store hits as float arrays inherently zero
            for (int x = 0; x < m_scanWidth; x++)
            {
                scanPos.x = scanMin.x + (m_scanResolution * (float)x);
                for (int z = 0; z < m_scanDepth; z++)
                {
                    scanPos.z = scanMin.z + (m_scanResolution * (float)z);
                    if (Physics.Raycast(scanPos, Vector3.down, out scanHit, m_scanBounds.size.y))
                    {
                        m_scanMap[x, z] = 1f - (scanHit.distance / m_scanBounds.size.y);
                    }
                }
            }

            //Now delete the scanned clone
            DestroyImmediate(workingGo);

            //Nad make sure we had some data
            if (m_scanMap.HasData() == false)
            {
                Debug.LogError("Unable to scan GameObject. GameObject load aborted.");
                return;
            }

            m_scanBounds = new Bounds(transform.position, new Vector3(m_scanWidth * m_scanResolution, m_scanWidth * m_scanResolution * 0.4f, m_scanDepth * m_scanResolution));
            m_baseLevel = m_scanMap.GetBaseLevel();

            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null)
            {
                mf = gameObject.AddComponent<MeshFilter>();
                mf.hideFlags = HideFlags.HideInInspector;
            }
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr == null)
            {
                mr = gameObject.AddComponent<MeshRenderer>();
                mr.hideFlags = HideFlags.HideInInspector;
            }
            mf.mesh = Gaia.GaiaUtils.CreateMesh(m_scanMap.Heights(), m_scanBounds.size);
            if (m_previewMaterial != null)
            {
                m_previewMaterial.hideFlags = HideFlags.HideInInspector;
                mr.sharedMaterial = m_previewMaterial;
            }
        }

        /// <summary>
        /// Save the stamp
        /// </summary>
        /// <returns>Path of saved stamp</returns>

        public string SaveScan()
        {
            if (m_scanMap == null || !m_scanMap.HasData())
            {
                Debug.LogWarning("Cant save scan as none has been loaded");
                return null;
            }

            //Save preview
            string path = Gaia.GaiaUtils.GetGaiaAssetPath(m_featureType, m_featureName);
            Gaia.GaiaUtils.CompressToSingleChannelFileImage(m_scanMap.Heights(), path, Gaia.GaiaConstants.fmtHmTextureFormat, false, true);

            path = Gaia.GaiaUtils.GetGaiaStampAssetPath(m_featureType, m_featureName);

            //Save stamp
            path = path + ".bytes";
            float [] metaData = new float[5];
            metaData[0] = m_scanWidth;
            metaData[1] = m_scanDepth;
            metaData[2] = m_scanHeight;
            metaData[3] = m_scanResolution;
            metaData[4] = m_baseLevel;
            byte[] byteData = new byte[metaData.Length * 4];
            Buffer.BlockCopy(metaData, 0, byteData, 0, byteData.Length);
            m_scanMap.SetMetaData(byteData);
            m_scanMap.SaveToBinaryFile(path);

            return path;
        }

        /// <summary>
        /// Update the scanner settings, fix any location and rotation, and perform any other housekeeping
        /// </summary>
        private void UpdateScanner()
        {
            //Reset rotation and scaling on scanner 
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            m_scanBounds.center = transform.position;
        }

        /// <summary>
        /// Draw gizmos, and make updates / overrides
        /// </summary>
        void OnDrawGizmosSelected()
        {
            //Housekeep
            UpdateScanner();

            //Draw a border to show what we are working on
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(m_scanBounds.center, m_scanBounds.size);

            //Draw the ground plane
            m_groundOffset = m_scanBounds.center;
            m_groundOffset.y = m_scanBounds.min.y + (m_scanBounds.max.y - m_scanBounds.min.y) * m_baseLevel;
            m_groundSize = m_scanBounds.size;
            m_groundSize.y = 0.1f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(m_groundOffset, m_groundSize);
        }
    }
}