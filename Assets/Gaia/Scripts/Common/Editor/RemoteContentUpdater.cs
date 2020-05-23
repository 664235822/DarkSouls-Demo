// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

namespace GaiaCommon1
{
    /// <summary>
    /// Helper class to update remote content
    /// </summary>
    public class RemoteContentUpdater
    {
        private readonly string URL;
        private IEnumerator m_updateCoroutine;
        private Action<PWMessage> m_downloadCompleteCallback;

        #region Constructors, Destructor, Disposal

        public RemoteContentUpdater(string url, Action<PWMessage> downloadCompleteCallback)
        {
            URL = url;
            m_downloadCompleteCallback = downloadCompleteCallback;

            // Handle updates
            if (!Application.isPlaying)
            {
                m_updateCoroutine = GetRemoteUpdate();
                EditorApplication.update += EditorUpdateDelegate;
            }
#if PW_DEBUG
            else
            {
                Debug.LogFormat("[RemoteContentUpdater]: Fetching content cancelled due to active play mode.");
            } 
#endif
        }

        /// <summary>
        /// Tidy things up
        /// </summary>
        ~RemoteContentUpdater()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose of things
        /// </summary>
        public void Dispose()
        {
            EditorApplication.update -= EditorUpdateDelegate;
        }

        #endregion

        /// <summary>
        /// This is executed only in the editor - we are using it to simulate co-routine execution and update execution
        /// </summary>
        void EditorUpdateDelegate()
        {
            if (m_updateCoroutine == null)
            {
                EditorApplication.update -= EditorUpdateDelegate;
            }
            else
            {
                m_updateCoroutine.MoveNext();
            }
        }

        /// <summary>
        /// Get the latest content from the web site at most once every 24 hours
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator GetRemoteUpdate()
        {
#if UNITY_2018_3_OR_NEWER
            using (UnityWebRequest www = UnityWebRequest.Get(URL))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    try
                    {
                        string result = www.downloadHandler.text;
#else
            using (WWW www = new WWW(URL))
            {
                while (!www.isDone)
                {
                    yield return www;
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.Log(www.error);
                }
                else
                {
                    try
                    {
                        string result = www.text;
#endif
                        int first = result.IndexOf("####");
                        if (first > 0)
                        {
                            result = result.Substring(first + 10);
                            first = result.IndexOf("####");
                            if (first > 0)
                            {
                                result = result.Substring(0, first);
                                result = result.Replace("<br />", "");
                                result = result.Replace("&#8221;", "\"");
                                result = result.Replace("&#8220;", "\"");
                                m_downloadCompleteCallback(JsonUtility.FromJson<PWMessage>(result));

                                //Debug.Log(result);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
            m_updateCoroutine = null;
        }
    }
}
