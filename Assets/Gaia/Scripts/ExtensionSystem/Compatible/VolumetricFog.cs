using UnityEngine;
using System.Collections;

namespace Gaia.GXC.Kronnect
{
    /// <summary>
    /// Description for village exteriors kit
    /// </summary>
    public class VolumetricFog : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Kronnect";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Volumetric Fog & Mist";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "VolumetricFog";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
			return @"Volumetric Fog & Mist is a full-screen image effect that adds realistic, live, moving fog, mist, dust, clouds and sky haze to your scenes making them less dull and boring.

Simply add the main script to your camera and you're set.

Volumetric Fog & Mist has been designed to provide a better looking fog and cloud formations with support of lighting and glow effects.

Comes with 12 highliy customizable configuration presets for quick setup & run: mist, windy mist, low cloud formation, sea of clouds, ground fog, frosted ground, foggy lake, fog and heavy fog, smoke, toxic swamp and two sand storm variations.

**Advanced features**

- Fog Volumes, special areas where fog & sky haze alpha is automatically changed. This feature is useful to hide the fog under water, to make it appear automatically (with a smooth transition) when player enter certain areas, or for whatever reason you need to control its transparency automatically. 

- Advanced support for the transparent and fade rendering mode of the Standard Shader. Not only it renders behind transparent objects, but also it allows you to enable a second render pass only over the transparent objects so the fog is really drawn both behind and in front of transparent objects improving significantly the effect over this type of objects.

- Elevated Fog & Clouds! allowing to render the fog along any vertical range. For instance, you can set the Base Height of the fog above Camera position to simulate floating smoke or clouds! And even you can fly-through these cloud formations!

- Void areas. This feature is useful to show a clear area around a world space position. For instance, in a 3rd Person View, you may want to show a clear area around the character. Spherical and boxed void areas are supported.

- Automatic Sun tracking, which allows you to simply assign (optionally) a light in your scene which represents the Sun to Volumetric script so it can automatically adjust the fog specular light direction, intensity and color. Just check the video called Volumetric Fog Dawn demo for an example.

- Fog of War: clear any number of areas in the fog! Check out the Fog of War Walking demo where you cut/clear the fog as you cross it! This is done calling just a function call passing the world position, radius and desired alpha for that area. Call this any number of times for any number of positions!
";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
			return "http://u3d.as/m5S";
        }
    }
}