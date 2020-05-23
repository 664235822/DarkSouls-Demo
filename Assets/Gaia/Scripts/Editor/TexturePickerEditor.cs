using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Gaia
{

    public delegate void ImageSelectedHandler(Texture2D imageName);

    public class TexturePickerEditor : EditorWindow
    {
        public List<Texture2D> images = new List<Texture2D>();

        /// <summary>
        /// A flag to indicate if the editor window has been setup or not.
        /// </summary>
        private bool m_isSetup = false;

        private ImageSelectedHandler m_handler;

        #region Setup

        /// <summary>
        /// Attempts to setup the editor by reading in textures from specified path.
        /// </summary>
        /// <param name='path'>
        /// Path to load images from.
        /// </param>
        public void Setup(string path, ImageSelectedHandler functionHandler)
        {
            string[] paths = new string[] { path };
            Setup(paths, functionHandler);
        } // eo Setup

        /// <summary>
        /// Attempts to setup the editor by reading in all textures specified
        /// by the various paths. Supports multiple paths of textures.
        /// </summary>
        /// <param name='paths'>
        /// Paths of textures to read in.
        /// </param>
        public void Setup(string[] paths, ImageSelectedHandler functionHandler)
        {
            m_isSetup = true;
            ReadInAllTextures(paths);
            m_handler = functionHandler;
        } // eo Setup

        #endregion Setup

        #region GUI

        Vector2 m_scrollPosition = Vector2.zero;
        void OnGUI()
        {
            if (!m_isSetup)
                return;

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

            // create a button for each image loaded in, 4 buttons in width
            // calls the handler when a new image is selected.
            int counter = 0;
            foreach (Texture2D img in images)
            {
                if (counter % 4 == 0 || counter == 0)
                    EditorGUILayout.BeginHorizontal();
                ++counter;

                if (GUILayout.Button(img, GUILayout.Height(100), GUILayout.Width(100)))
                {
                    // tell handler about new image, close selection window
                    m_handler(img);
                    EditorWindow.focusedWindow.Close();
                }

                if (counter % 4 == 0)
                    EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.EndScrollView();
        }

        #endregion GUI

        #region Utility

        /// <summary>
        /// Reads the in all textures from the paths.
        /// </summary>
        /// <param name='paths'>
        /// The paths to read images from.
        /// </param>
        void ReadInAllTextures(string[] paths)
        {
            foreach (string path in paths)
            {
                string[] allFilesInPath = Directory.GetFiles(path);
                foreach (string filePath in allFilesInPath)
                {
                    Texture2D obj = (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D));
                    if (obj is Texture2D)
                    {
                        images.Add(obj as Texture2D);
                    }
                }
            }
        }

        #endregion Utility
    }
}