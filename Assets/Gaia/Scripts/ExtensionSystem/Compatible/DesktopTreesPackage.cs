using UnityEngine;
using System.Collections;

namespace Gaia.GXC.SpeedTree
{
    /// <summary>
    /// Description for village exteriors kit
    /// </summary>
    public class DesktopTreesPackage : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "SpeedTree";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Desktop Trees Package";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "DesktopTreesPackage1";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"These SpeedTree models feature seamless LOD transitions and scalable wind effects that work in Unity out of the box. Package includes:
 
• An assortment of summer, autumn, winter, juvenile, and bare versions in desktop resolution.
• 190 high-res diffuse/normal/specular maps.
• 24 leaf map maker SPM files (details).
 
Create limitless variations of each model or design something completely new with a $19 USD/month subscription to the SpeedTree Modeler.
 
What makes SpeedTree models more than just a mesh? Click the link below.
";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "https://www.assetstore.unity3d.com/en/#!/content/24350";
        }
    }
}