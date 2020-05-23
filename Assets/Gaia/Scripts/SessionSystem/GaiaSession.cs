using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// A wrapper for scriptable objects
    /// </summary>
    [System.Serializable]
    public class ScriptableObjectWrapper
    {
        public string m_name;
        public string m_fileName;
        public byte[] m_content;

        /// <summary>
        /// Merge the file name with the sessioned file name
        /// </summary>
        /// <returns></returns>
        public string GetSessionedFileName(string sessionName)
        {
            return GaiaCommon1.Utils.FixFileName(sessionName) + "_" + Path.GetFileName(m_fileName);
        }

        /// <summary>
        /// Merge the file name with the sessioned file name
        /// </summary>
        /// <param name="sessionName">session file name</param>
        /// <param name="soFileName">scriptable object file name</param>
        /// <returns></returns>
        public static string GetSessionedFileName(string sessionName, string soFileName)
        {
            return GaiaCommon1.Utils.FixFileName(sessionName) + "_" + Path.GetFileName(soFileName);
        }

    }

    /// <summary>
    /// A Gaia session - core data structure used to store sessions
    /// </summary>
    [System.Serializable]
    public class GaiaSession : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Session name
        /// </summary>
        [TextArea(1,1)]
        public string m_name = string.Format("Session {0:yyyyMMdd-HHmmss}", DateTime.Now);

        /// <summary>
        /// Session description
        /// </summary>
        [TextArea(3, 5)]
        public string m_description = "";

        [HideInInspector]
        public Texture2D m_previewImage;

        /// <summary>
        /// When the session was created
        /// </summary>
        public string m_dateCreated = DateTime.Now.ToString();

        /// <summary>
        /// The width of the terrain it was created with
        /// </summary>
        public int m_terrainWidth = 0;

        /// <summary>
        /// The depth of the terrain it was created with
        /// </summary>
        public int m_terrainDepth = 0;

        /// <summary>
        /// The height of the terrain it was created with
        /// </summary>
        public int m_terrainHeight = 0;

        /// <summary>
        /// The session sea level
        /// </summary>
        public float m_seaLevel = 0f;

        /// <summary>
        /// Locked or not - if locked then no changes will be made to it
        /// </summary>
        public bool m_isLocked = false;

        /// <summary>
        /// The defaults for this session
        /// </summary>
        [HideInInspector]
        public ScriptableObjectWrapper m_defaults = null;

        /// <summary>
        /// The resources for this session - could be multiple of them
        /// </summary>
        [HideInInspector]
        public Dictionary<string, ScriptableObjectWrapper> m_resources = new Dictionary<string, ScriptableObjectWrapper>();

        /// <summary>
        /// The keys to the dictionary for serialisation
        /// </summary>
        [HideInInspector]
        public List<string> m_resourcesKeys = new List<string>();

        /// <summary>
        /// The values of the resources dictionary for serialisation
        /// </summary>
        [HideInInspector]
        public List<ScriptableObjectWrapper> m_resourcesValues = new List<ScriptableObjectWrapper>();

        /// <summary>
        /// The review image for this session
        /// </summary>
        [HideInInspector]
        public byte [] m_previewImageBytes = new byte[0];

        /// <summary>
        /// Width of preview image if there is one
        /// </summary>
        [HideInInspector]
        public int m_previewImageWidth = 0;

        /// <summary>
        /// Height of preview image if there is one
        /// </summary>
        [HideInInspector]
        public int m_previewImageHeight = 0;

        /// <summary>
        /// The operations that this session is made up of
        /// </summary>
        [HideInInspector]
        public List<GaiaOperation> m_operations = new List<GaiaOperation>();

        /// <summary>
        /// Populate our lists with the dictionary content
        /// </summary>
        public void OnBeforeSerialize()
        {
            m_resourcesKeys.Clear();
            m_resourcesValues.Clear();
            foreach (var kvp in m_resources)
            {
                m_resourcesKeys.Add(kvp.Key);
                m_resourcesValues.Add(kvp.Value);
            }
        }

        /// <summary>
        /// Pop the contents back into the dictionary
        /// </summary>
        public void OnAfterDeserialize()
        {
            m_resources = new Dictionary<string, ScriptableObjectWrapper>();
            for (int i = 0; i != Math.Min(m_resourcesKeys.Count, m_resourcesValues.Count); i++)
            {
                m_resources.Add(m_resourcesKeys[i], m_resourcesValues[i]);
            }
        }

        /// <summary>
        /// Get the session name as fixed file name without the extension
        /// </summary>
        /// <returns></returns>
        public string GetSessionFileName()
        {
            return GaiaCommon1.Utils.FixFileName(m_name);
        }

        /// <summary>
        /// Get the embedded preview image or null
        /// </summary>
        /// <returns>Embedded preview image or null</returns>
        public Texture2D GetPreviewImage()
        {
            if (m_previewImageBytes.GetLength(0) == 0)
            {
                return null;
            }

            Texture2D image = new Texture2D(m_previewImageWidth, m_previewImageHeight, TextureFormat.ARGB32, false);
            image.LoadRawTextureData(m_previewImageBytes);
            image.Apply();

            //Do a manual colour mod if in linear colour space
            #if UNITY_EDITOR
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                Color[] pixels = image.GetPixels();
                for (int idx = 0; idx < pixels.GetLength(0); idx++)
                {
                    pixels[idx] = pixels[idx].gamma;
                }
                image.SetPixels(pixels);
                image.Apply();
            }
            #endif

            image.name = m_name;
            return image;
        }
    }
}