using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Gaia
{
    /// <summary>
    /// Gaia compatible extension package
    /// </summary>
    public class GaiaCompatiblePackage
    {
        /// <summary>
        /// Package name
        /// </summary>
        public string m_packageName;

        /// <summary>
        /// The description of this package
        /// </summary>
        public string m_packageDescription;

        /// <summary>
        /// The image for this package
        /// </summary>
        public string m_packageImageName;

        /// <summary>
        /// The URL for this package
        /// </summary>
        public string m_packageURL;

        /// <summary>
        /// Whether or not it has been installed
        /// </summary>
        public bool m_isCompatible;

        /// <summary>
        /// Whether or not it has been installed
        /// </summary>
        public bool m_isInstalled;

        /// <summary>
        /// Are we folded out in the intalled tab
        /// </summary>
        public bool m_installedFoldedOut;

        /// <summary>
        /// Are we folded out in the compatible tab
        /// </summary>
        public bool m_compatibleFoldedOut;

        /// <summary>
        /// Extension methods
        /// </summary>
        public List<MethodInfo> m_methods = new List<MethodInfo>();

        /// <summary>
        /// Extension method foldouts
        /// </summary>
        public Dictionary<string, bool> m_methodGroupFoldouts = new Dictionary<string, bool>();
    }
}