#define GAIA_PRESENT

using UnityEngine;
using System.Collections;

namespace Gaia
{
    public static class GaiaConstants
    {
        /// <summary>
        /// Where scanned assets will be loaded from and saved to
        /// </summary>
        public static readonly string AssetDir = "Gaia/Stamps";
        public static readonly string AssetDirFromAssetDB = "Assets/Gaia/Stamps";

		/// <summary>
		/// Byte orders the scanner uses to load raw files.
		/// </summary>
		public enum RawByteOrder { IBM, Macintosh }

		/// <summary>
		/// Bit depths the scanner uses to load raw files.
		/// </summary>
		public enum RawBitDepth { Sixteen, Eight }

		/// <summary>
		/// The type of thing we are targeting
		/// </summary>
		public enum EnvironmentTarget { UltraLight, MobileAndVR, Desktop, PowerfulDesktop, Custom }

        /// <summary>
        /// The type of thing we are targeting
        /// </summary>
        public enum EnvironmentRenderer { BuiltIn, LightWeight2018x, HighDefinition2018x }

        /// <summary>
        /// The size of the environment we are targeting
        /// </summary>
        public enum EnvironmentSize { FromDefaults, Is256MetersSq, Is512MetersSq, Is1024MetersSq, Is2048MetersSq, Is4096MetersSq, Is8192MetersSq, Is16384MetersSq }

        /// <summary>
        /// Control type - from std assets
        /// </summary>
        public enum EnvironmentControllerType { FirstPerson, FlyingCamera, ThirdPerson }

        /// <summary>
        /// Operational mode of the manager
        /// </summary>
        public enum ManagerEditorMode {  Standard, Advanced, Extensions, ShowMore }

        /// <summary>
        /// Operational mode of the manager
        /// </summary>
        public enum ManagerEditorNewsMode { MoreOnGaia, MoreOnProceduralWorlds }

        /// <summary>
        /// Operational mode of the spawner component
        /// </summary>
        public enum OperationMode {  DesignTime, RuntimeInterval, RuntimeTriggeredInterval }

        /// <summary>
        /// Type of terrain operation
        /// </summary>
        public enum TerrainOperationType { AddToTerrain, ApplyMaskToSplatmap, ContrastFilter, GrowFeaturesFilter, DeNoiseFilter, HydraulicFilter, MultiplyTerrain, PowerOfFilter, QuantizeFilter, QuantizeCurvesFilter, SetTerrainToHeight, ShrinkFeaturesFilter, SubtractFromTerrain, ThermalFilter, ExportAspectMap, ExportBaseMap, ExportCurvatureMap, ExportFlowMap, ExportHeightMap, ExportNoiseMap, ExportNormalMap, ExportMasks, ExportSlopeMap }

        /// <summary>
        /// Type of mask merge operation
        /// </summary>
        public enum MaskMergeType { AssignMask2IfGreaterThan, AssignMask2IfLessThan, AddMask2, MultiplyByMask2, SubtractMask2 }

        /// <summary>
        /// Type of rain map
        /// </summary>
        public enum ErosionRainType { Constant, ErodePeaks, ErodeValleys, ErodeSlopes }

        /// <summary>
        /// Type of curvature to calculate
        /// </summary>
        public enum CurvatureType {  Average, Horizontal, Vertical }

        /// <summary>
        /// Type of aspect to calculate
        /// </summary>
        public enum AspectType { Aspect, Northerness, Easterness }

        /// <summary>
        /// Type of noise being generated
        /// </summary>
        public enum NoiseType { None, Perlin, Billow, Ridged }

        /// <summary>
        /// The fitness filter mode to apply when a texture is used to filter on fitness
        /// </summary>
        public enum ImageFitnessFilterMode { None, ImageGreyScale, ImageRedChannel, ImageGreenChannel, ImageBlueChannel, ImageAlphaChannel, TerrainTexture0, TerrainTexture1, TerrainTexture2, TerrainTexture3, TerrainTexture4, TerrainTexture5, TerrainTexture6, TerrainTexture7, PerlinNoise, BillowNoise, RidgedNoise }

        /// <summary>
        /// Classification of feature types - also used to load and save features
        /// </summary>
        public enum FeatureType { Adhoc, Bases, Hills, Islands, Lakes, Mesas, Mountains, Plains, Rivers, Rocks, Valleys, Villages, Waterfalls};

        /// <summary>
        /// Types of borders to put on generated terrain
        /// </summary>
        public enum GeneratorBorderStyle { None, Mountains, Water }

        /// <summary>
        /// The type of feature operation that will be used on the terrain by the stamper
        /// AddHeight - Will add height is stamped object exceeds height at that location
        /// RemoveHeight - Will remove height if the terrain is higher than stamp at that location
        /// </summary>
        public enum FeatureOperation { RaiseHeight, LowerHeight, BlendHeight, StencilHeight, DifferenceHeight };

        /// <summary>
        /// The shape of the spawner
        /// </summary>
        public enum SpawnerShape { Box, Sphere }

        /// <summary>
        /// The algorithm used to choose spawn locations
        /// RandomLocation - A random location will be chosen
        /// RandomLocationSeeded - A random start location will be chose, the next location will be based in seed throw radius
        /// EveryLocation - Every location in the spawner area will be checked
        /// EveryLocationJittered - Every location in the spawner area will be checked, and jitter added to break up the lines
        /// </summary>
        public enum SpawnerLocation { RandomLocation, RandomLocationClustered, EveryLocation, EveryLocationJittered }

        /// <summary>
        /// The algorithm used at spawn locations to determine suitability
        /// PointCheck - Just that location is checked
        /// BoundedAreaCheck - The entire area is checked - slower but good for large object placement
        /// </summary>
        public enum SpawnerLocationCheckType { PointCheck, BoundedAreaCheck }

        /// <summary>
        /// The algorithm which defines which rule will be selected in spawn location. Regardless of whether
        ///   selected or not, the rule will still only run if it exceeds its minimum fitness settings, and 
        ///   didnt randomly fail (failure rate).
        /// All - All rules will be run at this location
        /// Fittest - Only the fittest rule will be run at this location
        /// WeightedFittest - The fittest rule will most likely run, then the next fittest, distributed by relative fitness
        /// Random - A random rule will be chosen and evaluated to run
        /// </summary>
        public enum SpawnerRuleSelector { All, Fittest, WeightedFittest, Random }

        /// <summary>
        /// The type of resource that the spawner will use in a spawn rule
        /// </summary>
        public enum SpawnerResourceType { TerrainTexture, TerrainDetail, TerrainTree, GameObject /*, Stamp*/ }

        /// <summary>
        /// Used by system determine what constitutes a valid virgin terrain height check threshold
        /// </summary>
        public static float VirginTerrainCheckThreshold = 0.01f;

        /// <summary>
        /// File type to use when saving images
        /// </summary>
        public enum ImageFileType {  Jpg, Png, Exr }

        /// <summary>
        /// Image formatting defaults
        /// </summary>
        public const TextureFormat defaultTextureFormat = TextureFormat.RGBA32;
        public const TextureFormat fmtHmTextureFormat = TextureFormat.RGBA32;
        public const TextureFormat fmtRGBA32 = TextureFormat.RGBA32;

        /// <summary>
        /// Storage formats
        /// </summary>
        public enum StorageFormat { PNG, JPG }
        public const StorageFormat defaultImageStorageFormat = StorageFormat.PNG;

        /// <summary>
        /// Image channels
        /// </summary>
        public enum ImageChannel { R, G, B, A }
        public const ImageChannel defaultImageStorageChannel = ImageChannel.R;

    }
}

