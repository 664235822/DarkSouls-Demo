using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// Gaia compatible extension publisher
    /// </summary>
    public class GaiaCompatiblePublisher
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        public string m_publisherName;

        /// <summary>
        /// Are we folded out in the intalled tab
        /// </summary>
        public bool m_installedFoldedOut;

        /// <summary>
        /// Are we folded out in the compatible tab
        /// </summary>
        public bool m_compatibleFoldedOut;

        /// <summary>
        /// The packages that belong to this publisher
        /// </summary>
        private Dictionary<string, GaiaCompatiblePackage> m_packages = new Dictionary<string,GaiaCompatiblePackage>();

        /// <summary>
        /// Get an existing package, or null if its not there
        /// </summary>
        /// <param name="packageName">Package name to get</param>
        /// <returns>Package if we have it, or null if we dont</returns>
        public GaiaCompatiblePackage GetPackage(string packageName)
        {
            GaiaCompatiblePackage package;
            if (m_packages.TryGetValue(packageName, out package))
            {
                return package;
            }
            return null;
        }

        /// <summary>
        /// Get a list of the packages being managed for this publisher
        /// </summary>
        /// <returns></returns>
        public List<GaiaCompatiblePackage> GetPackages()
        {
            List<GaiaCompatiblePackage> packages = new List<GaiaCompatiblePackage>(m_packages.Values);
            packages.Sort((a, b) => a.m_packageName.CompareTo(b.m_packageName));
            return packages;
        }

        /// <summary>
        /// Return the number of installed packages
        /// </summary>
        /// <returns>The number of installed packages</returns>
        public int InstalledPackages()
        {
            int installedExtensions = 0;
            foreach (KeyValuePair<string, GaiaCompatiblePackage> kvp in m_packages)
            {
                if (kvp.Value.m_isInstalled)
                {
                    installedExtensions++;
                }
            }
            return installedExtensions;
        }

        /// <summary>
        /// Return the number of compatible packages
        /// </summary>
        /// <returns>The number of compatible packages</returns>
        public int CompatiblePackages()
        {
            int compatiblePackages = 0;
            foreach (KeyValuePair<string, GaiaCompatiblePackage> kvp in m_packages)
            {
                if (kvp.Value.m_isCompatible)
                {
                    compatiblePackages++;
                }
            }
            return compatiblePackages;
        }

        /// <summary>
        /// Add a package
        /// </summary>
        /// <param name="package"></param>
        public void AddPackage(GaiaCompatiblePackage package)
        {
            m_packages.Add(package.m_packageName, package);
        }
    }
}