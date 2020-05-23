using UnityEngine;
using System.Collections;

namespace Gaia.GXC.Dogmatic
{
    /// <summary>
    /// Description for village exteriors kit
    /// </summary>
    public class AQUASWaterSet : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Dogmatic";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "AQUAS Water Set";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "AQUASWaterSet";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"AQUAS Water contains a set of 9 flat water shaders for all types of platforms, environments and games. It is highly customizable and feature rich to suit all needs.
            
AQUAS contains 4 mobile shaders for different performance levels and 5 high-quality shaders for web and desktop applications.

Features:

Rendering:
- Multi-Light-Support
- Distorted Realtime Reflections
- Realtime Refraction
- Depth Based Color Absorption
- Self-Sustaining Fog System, that works with any custom lighting
- Dual-layered Caustic effects

Underwater Effects
- Limited & Distorted Vision
- Bloom, Blur & Godrays
- 3D Morphing Bubbles
- Realistic Bubble Spawner
- Advanced Wet Lens Effect

Ease of use
- Works out of the box (Demo Scene included)
- Highly customizable
- Quick Setup

AQUAS works with Unity Free & Pro";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "https://www.assetstore.unity3d.com/en/#!/content/52103";
        }
    }
}