using UnityEngine;
using System.Collections;

namespace Gaia.GXC.ThreeDForge
{
    /// <summary>
    /// Description for village exteriors kit
    /// </summary>
    public class VillageExteriorsKit : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "3D Forge";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Village Exteriors Kit";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "VillageExteriorsKit";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"This modular kit is what you have been looking for to construct all the Medieval Fantasy Village & Town buildings you would ever need. 
Taverns & Inns, Magic Shops, Town Halls, Guild Halls, Blacksmith Forges, General Stores, Jeweler Shops, Potion Shops, Herbalist, Banks, Armorer, Fletcher, Stables, Keeps, Barns, Store Rooms and many more buildings for your game.";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "https://www.assetstore.unity3d.com/en/#!/content/38045";
        }
    }
}