using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// Manages extensions
    /// </summary>
    public class GaiaExtensionManager
    {
        /// <summary>
        /// The installed extensions
        /// </summary>
        private Dictionary<string, GaiaCompatiblePublisher> m_extensions = new Dictionary<string, GaiaCompatiblePublisher>();

        /// <summary>
        /// Scan for installed extensions
        /// </summary>
        public void ScanForExtensions()
        {
            #if UNITY_EDITOR
            if (EditorApplication.isCompiling)
            {
                return;
            }
            #endif

            m_extensions.Clear();

            string[] parsedName;
            string publisherName = "", packageName = "", packageImage = "", packageDescription = "", packageURL = "";
            MethodInfo method;
            MethodInfo[] methods;
            List<MethodInfo> extensionMethods;
            List<Type> types = GetTypesInNamespace("Gaia.GX.");

            //Process installed extensions
            for (int typeIdx = 0; typeIdx < types.Count; typeIdx++)
            {
                //Get publisher and package name
                parsedName = types[typeIdx].FullName.Split('.');
                publisherName = Regex.Replace(parsedName[2], "(\\B[A-Z])", " $1");
                packageName = Regex.Replace(parsedName[3], "(\\B[A-Z])", " $1");

                //Grab the extension methods, update publisher and package name if necessary
                methods = types[typeIdx].GetMethods(BindingFlags.Public | BindingFlags.Static);
                extensionMethods = new List<MethodInfo>();
                for (int methodIdx = 0; methodIdx < methods.Length; methodIdx++)
                {
                    method = methods[methodIdx];

                    if (method.Name.StartsWith("GX_"))
                    {
                        extensionMethods.Add(method);
                    }
                    else if (method.Name == "GetPublisherName")
                    {
                        publisherName = (string)method.Invoke(null, null);
                    }
                    else if (method.Name == "GetPackageName")
                    {
                        packageName = (string)method.Invoke(null, null);
                    }
                }

                //See if we can locate the publisher, if not then add them
                GaiaCompatiblePublisher publisher = null;
                if (!m_extensions.TryGetValue(publisherName, out publisher))
                {
                    publisher = new GaiaCompatiblePublisher();
                    publisher.m_publisherName = publisherName;
                    publisher.m_compatibleFoldedOut = false;
                    publisher.m_installedFoldedOut = false;
                    m_extensions.Add(publisherName, publisher);
                }

                //See if we can locate the extension, if not then add it
                GaiaCompatiblePackage package = publisher.GetPackage(packageName);
                if (package == null)
                {
                    package = new GaiaCompatiblePackage();
                    package.m_compatibleFoldedOut = false;
                    package.m_installedFoldedOut = false;
                    package.m_packageName = packageName;
                    publisher.AddPackage(package);
                }
                if (extensionMethods.Count > 0)
                {
                    package.m_isInstalled = true;
                }
                else
                {
                    package.m_isInstalled = false;
                }
                package.m_methods = new List<MethodInfo>(extensionMethods);
            }

            //Then process compatible extensions
            types = GetTypesInNamespace("Gaia.GXC.");
            for (int typeIdx = 0; typeIdx < types.Count; typeIdx++)
            {
                //Get publisher and package name
                parsedName = types[typeIdx].FullName.Split('.');
                publisherName = Regex.Replace(parsedName[2], "(\\B[A-Z])", " $1");
                packageName = Regex.Replace(parsedName[3], "(\\B[A-Z])", " $1");

                //Grab the extension methods, update publisher and package name if necessary
                methods = types[typeIdx].GetMethods(BindingFlags.Public | BindingFlags.Static);
                for (int methodIdx = 0; methodIdx < methods.Length; methodIdx++)
                {
                    method = methods[methodIdx];

                    if (method.Name == "GetPublisherName")
                    {
                        publisherName = (string)method.Invoke(null, null);
                    }
                    else if (method.Name == "GetPackageName")
                    {
                        packageName = (string)method.Invoke(null, null);
                    }
                    else if (method.Name == "GetPackageImage")
                    {
                        packageImage = (string)method.Invoke(null, null);
                    }
                    else if (method.Name == "GetPackageDescription")
                    {
                        packageDescription = (string)method.Invoke(null, null);
                    }
                    else if (method.Name == "GetPackageURL")
                    {
                        packageURL = (string)method.Invoke(null, null);
                    }
                }

                //See if we can locate the publisher, if not then add them
                GaiaCompatiblePublisher publisher = null;
                if (!m_extensions.TryGetValue(publisherName, out publisher))
                {
                    publisher = new GaiaCompatiblePublisher();
                    publisher.m_publisherName = publisherName;
                    publisher.m_compatibleFoldedOut = false;
                    publisher.m_installedFoldedOut = false;
                    m_extensions.Add(publisherName, publisher);
                }

                //See if we can locate the extension, if not then add it
                GaiaCompatiblePackage package = publisher.GetPackage(packageName);
                if (package == null)
                {
                    package = new GaiaCompatiblePackage();
                    package.m_compatibleFoldedOut = false;
                    package.m_installedFoldedOut = false;
                    package.m_packageName = packageName;
                    publisher.AddPackage(package);
                }
                package.m_isCompatible = true;
                package.m_packageDescription = packageDescription;
                package.m_packageImageName = packageImage;
                package.m_packageURL = packageURL;
            }
        }

        /// <summary>
        /// Ruturn the number of installed extensions
        /// </summary>
        /// <returns>Number of installed extensions</returns>
        public int GetInstalledExtensionCount()
        {
            int iec = 0;

            foreach (GaiaCompatiblePublisher publisher in m_extensions.Values)
            {
                iec += publisher.InstalledPackages();
            }

            return iec;
        }


        /// <summary>
        /// Get a list of the current publishers
        /// </summary>
        /// <returns></returns>
        public List<GaiaCompatiblePublisher> GetPublishers()
        {
            List<GaiaCompatiblePublisher> publishers = new List<GaiaCompatiblePublisher>(m_extensions.Values);
            publishers.Sort((a, b) => a.m_publisherName.CompareTo(b.m_publisherName));
            return publishers;
        }

        /// <summary>
        /// Scan unity cimpled assemblies for all the types in the provided namespace
        /// </summary>
        /// <param name="nameSpace">Namespace to search</param>
        /// <returns>Listr of types</returns>
        public List<Type> GetTypesInNamespace(string nameSpace)
        {
            List<Type> matchingTypes = new List<Type>();

            int assyIdx, typeIdx;
            System.Type[] types;
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (assyIdx = 0; assyIdx < assemblies.Length; assyIdx++)
            {
                if (assemblies[assyIdx].FullName.StartsWith("Assembly"))
                {
                    types = assemblies[assyIdx].GetTypes();
                    for (typeIdx = 0; typeIdx < types.Length; typeIdx++)
                    {
                        if (!string.IsNullOrEmpty(types[typeIdx].Namespace))
                        {
                            if (types[typeIdx].Namespace.StartsWith(nameSpace))
                            {
                                matchingTypes.Add(types[typeIdx]);
                            }
                        }
                    }
                }
            }
            return matchingTypes;
        }
    }

}
