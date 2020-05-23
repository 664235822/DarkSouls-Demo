// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GaiaCommon1
{
    /// <summary>
    /// The remote content object will automatically load and save their data and use the updater to automatically update according to their settings.
    /// </summary>
    [Serializable]
    public abstract class RemoteContent
    {
        // The last time the content was upated
        protected int m_lastUpdated = 0;

        public readonly string SOURCE_URL;

        protected readonly int SECONDS_BETWEEN_UPDATES;
        protected string m_id;
        protected string m_path;

        public bool ExistOnDisc { get { return File.Exists(m_path); } }

        /// <summary>
        /// Create a RemoteContent object.
        /// </summary>
        /// <param name="id">This will be used to identify the content on disc and in varous places. </param>
        /// <param name="url">The url for the remote content.</param>
        /// <param name="hoursBetweenUpdates">How often should this content update?</param>
        public RemoteContent(string id, string url, int hoursBetweenUpdates = 24)
        {
            m_id = id;
            m_path = "Library/" + id + ".pwrc" + PWConst.VERSION;
            SOURCE_URL = url;
            SECONDS_BETWEEN_UPDATES = hoursBetweenUpdates * 60 * 60;
        }

        /// <summary>
        /// Updates content if it's time.
        /// </summary>
        public void Update()
        {
            if (Utils.GetFrapoch() - m_lastUpdated > SECONDS_BETWEEN_UPDATES)
            {
#if PW_DEBUG
                Debug.LogFormat("[RemoteContent]: Time to update '{0}'.", m_id); 
#endif
                new RemoteContentUpdater(SOURCE_URL, ProcessUpdate);
            }
        }

        /// <summary>
        /// Save the data to disc
        /// </summary>
        public void Save()
        {
#if PW_DEBUG
                Debug.LogFormat("[RemoteContent]: Saving '{0}'...", m_id); 
#endif
            // Store in './Library'
            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(m_path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, this);
            }
        }

        /// <summary>
        /// Load the data from disc.
        /// </summary>
        protected static RemoteContent LoadPath(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (RemoteContent)formatter.Deserialize(stream);
            }
        }

        private void ProcessUpdate(PWMessage message)
        {
            if (message != null)
            {
                m_lastUpdated = Utils.GetFrapoch();
                ProcessRemoteData(message);
                Save();
            }
        }

        protected abstract void ProcessRemoteData(PWMessage message);
    }
}
