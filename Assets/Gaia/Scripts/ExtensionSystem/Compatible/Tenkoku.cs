using UnityEngine;
using System.Collections;

namespace Gaia.GXC.TanukiDigital
{
    /// <summary>
    /// Description for village exteriors kit
    /// </summary>
    public class TenkokuDynamicSky : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Tanuki Digital";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "TENKOKU Dynamic Sky";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "Tenkoku";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"Tenkoku – Dynamic Sky brings completely dynamic high-fidelity sky rendering to Unity developers. There are no sky boxes or static elements, everything from the clouds to each individual star is it's own individual element with appropriate movement and behavior. 

Accuracy
- Complete 24hour day-night-year cycle
- True equinox & precession positioning
- Latitude and longitude adjustment
- Accurate sun, planet, and star positions
- Accurate lunar movement and phases

Lighting & Rendering
- Adjustable full scene lighting
- Volumetric lit atmosphere effects
- Multi-layer dynamic cloud formations
- Beautiful night sky rendering

Advanced Effects
- Ambient sound effects
- Advanced weather effects
- Aurora rendering based on latitude
- Partial, annular, and total solar eclipse

Useability
- Built for Unity 5
- Simple to setup and customize
- Easy-to-use custom interface
- For PC/Linux/Mac & Webplayer
";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "https://www.assetstore.unity3d.com/en/#!/content/34435";
        }
    }
}