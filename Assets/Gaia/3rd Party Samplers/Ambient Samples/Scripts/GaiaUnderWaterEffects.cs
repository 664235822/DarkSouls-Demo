using System.Collections;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
#if HDPipeline
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Gaia
{
    /// <summary>
    /// Setup for the underwater FX features   
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class GaiaUnderWaterEffects : MonoBehaviour
    {
        #region Variables
        [Header("Global")]
        public GaiaConstants.EnvironmentRenderer m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
        [Header("Caustic Settings")]
        [Tooltip("Sets if the light object follows the player")]
        public bool m_followPlayer = false;
        [Tooltip("Creates gameobject walls around the player to fix the horizon color issues with the fog rendering")]
        public bool m_useHorizonFix = true;
        [Tooltip("Creates simple underwater particles effect")]
        public bool m_useUnderwaterparticles = true;
        [Tooltip("Sets the caustics size, not this only works on directonial lights")]
        [Range(0, 2000)]
        public int m_causticsSize = 5;
        [Tooltip("Caustic textures used to generate the effect")]
        public Texture[] m_cookies = new Texture[16];
        [Tooltip("How many frame renders are made, higher the number the faster the animation. Recommend between 15-30 for optimial performance and visuals")]
        public float m_framesPerSecond = 25f;
        [Tooltip("What the current sea level is. Gaias default is 50")]
        public float m_sealevel = 50f;
        [Header("Underwater Settings")]
        [Range(0f, 1f)]
        [Tooltip("Sets the underwater ambiance audio volume")]
        public float m_underwaterSoundFXVolume = 0.4f;
        [Range(0f, 1f)]
        [Tooltip("Sets the water submerge audio volume")]
        public float m_waterSubmergeSounfFXVolume = 0.4f;
        [Tooltip("Sets the submerge down sound fx")]
        public AudioClip m_submergeSoundFXDown;
        [Tooltip("Sets the submerge up sound fx")]
        public AudioClip m_submergeSoundFXUp;
        [Tooltip("Sets the underwater fog color")]
        public Color32 m_underWaterFogColor = new Color32(76, 112, 142, 255);
        [Tooltip("Sets the underwater fog distance")]
        public float m_underWaterFogDistance = 70f;
        //The light
        Light mainlight;
        //This object it's attached too
        Transform causticsObject;
        //The player object
        //[HideInInspector]
        public Transform player;
        //What texture number it's on
        int indexNumber = 0;
        //Status of the coroutine
        bool coroutineStatus = false;
        //Fog color stored
        [HideInInspector]
        public Color32 storedFogColor;
        //Fog distance stored
        [HideInInspector]
        public float storedFogDistance;
        //Ambient audio
        GameObject ambientAudio;
        //Underwater audio
        GameObject underwaterAudio;
        //The horizon fix gameobject
        GameObject horizonObject;
        [HideInInspector]
        public GameObject horizonObjectStored;
        //The objects audio source
        AudioSource objectAudioSource;
        //The underwater particles
        GameObject underwaterParticles;
        [HideInInspector]
        //Stored underwater particles gameobject
        public GameObject underwaterParticlesStored;
#if UNITY_2019_1_OR_NEWER && HDPipeline
        [Header("HDRP 2019 Underwater Effects")]
        public VolumeProfile m_aboveWaterProfile;
        public VolumeProfile m_underwaterProfile;
        public Volume postVolume;
        public Volume volume;
#elif !UNITY_2019_1_OR_NEWER && HDPipeline
        public Volume volume;
#endif
        //Parent Object
        private Transform partentObject;
        //Gaia settings for pipeline support
        private GaiaSettings m_gaiaSettings;
        //Gaia scene info
        private GaiaSceneInfo m_gaiaSceneInfo;
#if HDPipeline
        //Underwater reflection probe for HD pipeline
        private ReflectionProbe m_underwaterProbe;
#endif
#if UNITY_POST_PROCESSING_STACK_V2
        public PostProcessVolume transitionPostFX;
        public PostProcessVolume underwaterPostFX;
#endif
#endregion

        #region Setup
        /// <summary>
        /// Start function
        /// </summary>
        void Start()
        {
#if UNITY_POST_PROCESSING_STACK_V2
            transitionPostFX = GameObject.Find("Underwater Transition PostFX").GetComponent<PostProcessVolume>();

            underwaterPostFX = GameObject.Find("Underwater PostFX").GetComponent<PostProcessVolume>();

            if (Application.isPlaying)
            {

                if (transitionPostFX != null)
                {
                    transitionPostFX.enabled = true;
                }
                if (underwaterPostFX != null)
                {
                    underwaterPostFX.enabled = true;
                }
            }
            else
            {
                if (transitionPostFX != null)
                {
                    transitionPostFX.enabled = false;
                }
                if (underwaterPostFX != null)
                {
                    underwaterPostFX.enabled = false;
                }
            }
#endif

            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }

            if (m_gaiaSceneInfo == null)
            {
                m_gaiaSceneInfo = GaiaSceneInfo.GetSceneInfo();
            }

            if (m_gaiaSettings != null)
            {
                m_currentRenderer = m_gaiaSettings.m_currentRenderer;
            }

#if HDPipeline
            if (m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GameObject underwaterProbe = GameObject.Find("Underwater Reflection Probe");
                if (underwaterProbe != null)
                {
                    m_underwaterProbe = GameObject.Find("Underwater Reflection Probe").GetComponent<ReflectionProbe>();
                }
            }
#endif
            GameObject waterPlanar = GameObject.Find("Water Planar Reflections");
            if (waterPlanar != null)
            {
                ReflectionProbe reflection = GameObject.Find("Water Planar Reflections").GetComponent<ReflectionProbe>();
                {
                    if (reflection != null)
                    {
                        reflection.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
                    }
#if HDPipeline
                    HDAdditionalReflectionData reflectionData = GameObject.Find("Water Planar Reflections").GetComponent<HDAdditionalReflectionData>();
                    if (reflectionData != null)
                    {
#if !UNITY_2019_1_OR_NEWER
                        reflectionData.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
#else
                        reflectionData.mode = ProbeSettings.Mode.Realtime;
#endif
                    }
#endif
                    }
            }

#if HDPipeline
            if (m_underwaterProbe != null)
            {
                m_underwaterProbe.enabled = false;
            }
#endif

            mainlight = gameObject.GetComponent<Light>();
            mainlight.cookie = null;
            causticsObject = gameObject.transform;
            partentObject = GetOrCreateEnvironmentParent().transform;
            if (m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GameObject volumeObject = GameObject.Find("High Definition Environment Volume");
                if (volumeObject != null)
                {
#if !UNITY_2019_1_OR_NEWER && HDPipeline
                    if (volume == null)
                    {
                        volume = GameObject.Find("High Definition Environment Volume").GetComponent<Volume>();
                        if (volume != null)
                        {
                            VolumeProfile profile = volume.sharedProfile;
                            if (profile != null)
                            {
                                VolumetricFog fog;
                                if (profile.TryGet(out fog))
                                {
                                    storedFogColor = fog.albedo.value;
                                    storedFogDistance = fog.meanFreePath.value;
                                }
                            }
                        }
                    }
#endif
                }
                else
                {
                    Debug.LogWarning("Unabled to find a HDRP environment volume in the scene. Please insure one is set in this scene.");
                }
#if UNITY_2019_1_OR_NEWER && HDPipeline
                postVolume = GameObject.Find("Gaia HDRP Post Processing").GetComponent<Volume>();
#endif
            }
            else
            {
                storedFogColor = RenderSettings.fogColor;
                storedFogDistance = RenderSettings.fogEndDistance;
            }

            ambientAudio = null;
            if (ambientAudio == null)
            {
                ambientAudio = GameObject.Find("Ambient Audio");
            }

            underwaterAudio = null;
            if (underwaterAudio == null)
            {
                underwaterAudio = GameObject.Find("Underwater SoundFX");
            }
#if UNITY_EDITOR
            if (player == null)
            {
                player = GetThePlayer();
            }
#endif
            if (m_gaiaSceneInfo != null)
            {
                m_sealevel = m_gaiaSceneInfo.m_seaLevel;
            }

            if (m_useHorizonFix)
            {
                horizonObject = GameObject.Find("Ambient Underwater Horizon");
                if (horizonObjectStored != null)
                {
                    horizonObject = Instantiate(horizonObjectStored);
                    horizonObject.name = "Ambient Underwater Horizon";
                    if (partentObject != null)
                    {
                        horizonObject.transform.parent = partentObject;
                    }

                    MeshRenderer[] theRenders = horizonObject.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer rends in theRenders)
                    {
                        rends.enabled = false;
                    }
                }
            }

            if (m_useUnderwaterparticles)
            {
                underwaterParticles = GameObject.Find("Underwater Particles Effects");
                if (underwaterParticlesStored != null)
                {
                    underwaterParticles = Instantiate(underwaterParticlesStored);
                    underwaterParticles.name = "Underwater Particles Effects";
                    underwaterParticles.SetActive(false);
                    if (partentObject != null)
                    {
                        underwaterParticles.transform.parent = partentObject;
                    }
                }
            }

            if (gameObject.GetComponent<AudioSource>() == null)
            {
                gameObject.AddComponent<AudioSource>();
                objectAudioSource = gameObject.GetComponent<AudioSource>();
                objectAudioSource.volume = m_waterSubmergeSounfFXVolume;
            }
            else
            {
                objectAudioSource = gameObject.GetComponent<AudioSource>();
                objectAudioSource.volume = m_waterSubmergeSounfFXVolume;
            }

            if (mainlight.type == LightType.Directional)
            {
                mainlight.cookieSize = m_causticsSize;
            }

            StopAllCoroutines();
        }

        /// <summary>
        /// On Enable
        /// </summary>
        private void OnEnable()
        {
#if UNITY_EDITOR
            m_gaiaSettings = GaiaUtils.GetGaiaSettings();

            m_gaiaSceneInfo = GaiaSceneInfo.GetSceneInfo();

            if (m_gaiaSettings != null)
            {
                m_currentRenderer = m_gaiaSettings.m_currentRenderer;
            }
#if UNITY_POST_PROCESSING_STACK_V2
            transitionPostFX = GameObject.Find("Underwater Transition PostFX").GetComponent<PostProcessVolume>();

            underwaterPostFX = GameObject.Find("Underwater PostFX").GetComponent<PostProcessVolume>();

            if (Application.isPlaying)
            {
                transitionPostFX = GameObject.Find("Underwater Transition PostFX").GetComponent<PostProcessVolume>();
                if (transitionPostFX != null)
                {
                    transitionPostFX.enabled = true;
                }

                underwaterPostFX = GameObject.Find("Underwater PostFX").GetComponent<PostProcessVolume>();
                if (underwaterPostFX != null)
                {
                    underwaterPostFX.enabled = true;
                }
            }
            else
            {
                transitionPostFX = GameObject.Find("Underwater Transition PostFX").GetComponent<PostProcessVolume>();
                if (transitionPostFX != null)
                {
                    transitionPostFX.enabled = false;
                }

                underwaterPostFX = GameObject.Find("Underwater PostFX").GetComponent<PostProcessVolume>();
                if (underwaterPostFX != null)
                {
                    underwaterPostFX.enabled = false;
                }
            }
#endif

#if UNITY_2019_1_OR_NEWER && HDPipeline
            if (m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                if (m_aboveWaterProfile == null)
                {
                    m_aboveWaterProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GaiaPipelineUtils.GetAssetPath("Gaia HDRP Post Processing Profile"));
                }

                if (m_underwaterProfile == null)
                {
                    m_underwaterProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GaiaPipelineUtils.GetAssetPath("Gaia HDRP Underwater Post Processing Profile"));
                }
            }
#endif
#endif

            if (m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
                GameObject volumeObject = GameObject.Find("High Definition Environment Volume");
                if (volumeObject != null)
                {
#if !UNITY_2019_1_OR_NEWER && HDPipeline
                    if (volume == null)
                    {
                        volume = GameObject.Find("High Definition Environment Volume").GetComponent<Volume>();
                        if (volume != null)
                        {
                            VolumeProfile profile = volume.sharedProfile;
                            if (profile != null)
                            {
                                VolumetricFog fog;
                                if (profile.TryGet(out fog))
                                {
                                    storedFogColor = fog.albedo.value;
                                    storedFogDistance = fog.meanFreePath.value;
                                }
                            }
                        }
                    }
#endif
                }
                else
                {
                    Debug.LogWarning("Unabled to find a HDRP environment volume in the scene. Please insure one is set in this scene.");
                }
            }

#if UNITY_2019_1_OR_NEWER && HDPipeline
                postVolume = GameObject.Find("Gaia HDRP Post Processing").GetComponent<Volume>();
#endif
        }

            /// <summary>
            /// Gets player and returns if found
            /// </summary>
            /// <returns>Player</returns>
            public Transform GetThePlayer()
        {
#if UNITY_EDITOR
            Transform thePlayer = null;
            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }

            if (m_gaiaSettings.m_currentController == GaiaConstants.EnvironmentControllerType.FirstPerson)
            {
                if (GameObject.Find("FirstPersonCharacter") != null)
                {
                    thePlayer = GameObject.Find("FirstPersonCharacter").transform;

                    if (thePlayer != null)
                    {
                        return thePlayer;
                    }
                }
            }

            if (m_gaiaSettings.m_currentController == GaiaConstants.EnvironmentControllerType.FlyingCamera)
            {
                if (GameObject.Find("FlyCam") != null)
                {
                    thePlayer = GameObject.Find("FlyCam").transform;

                    if (thePlayer != null)
                    {
                        return thePlayer;
                    }
                }
            }

            if (m_gaiaSettings.m_currentController == GaiaConstants.EnvironmentControllerType.ThirdPerson)
            {
                if (GameObject.Find("Main Camera") != null)
                {
                    thePlayer = GameObject.Find("Main Camera").transform;

                    if (thePlayer != null)
                    {
                        return thePlayer;
                    }
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// Gets and returns the parent gameobject
        /// </summary>
        /// <returns>Parent Object</returns>
        private static GameObject GetOrCreateEnvironmentParent()
        {
            GameObject parent = GameObject.Find("Gaia Environment");
            if (parent == null)
            {
                parent = new GameObject("Gaia Environment");
            }
            return parent;
        }

        /// <summary>
        /// Sets fog back to default
        /// </summary>
        private void OnDisable()
        {
            if (m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
            {
#if HDPipeline
                if (volume != null)
                {
                    VolumeProfile profile = volume.sharedProfile;
                    if (profile != null)
                    {
                        VisualEnvironment environment;
                        if (profile.TryGet(out environment))
                        {
                            environment.fogType.value = FogType.Volumetric;
                        }

                        VolumetricFog fog;
                        if (profile.TryGet(out fog))
                        {
                            fog.active = true;
                        }

                        LinearFog linearFog;
                        if (profile.TryGet(out linearFog))
                        {
                            linearFog.active = false;
                        }
                    }
                }
#endif
            }
            else
            {
                RenderSettings.fogColor = storedFogColor;
                RenderSettings.fogEndDistance = storedFogDistance;
            }
        }
        #endregion

        #region Positioning and Enabling
        /// <summary>
        /// Late update
        /// </summary>
        private void Update()
        {            
            if (player != null)
            {
                if (m_useHorizonFix && horizonObject != null)
                {
                    horizonObject.transform.position = new Vector3(player.position.x + 1000f, m_sealevel - 300f, player.position.z);
                }

                if (m_useUnderwaterparticles && underwaterParticles != null)
                {
                    underwaterParticles.transform.position = player.position;
                }

                if (m_followPlayer)
                {
                    causticsObject.position = new Vector3(player.position.x, m_sealevel, player.position.z);
                }

                if (player.position.y >= m_sealevel)
                {
                    if (coroutineStatus)
                    {
                        StopAllCoroutines();
                        StopCoroutine(CausticsAnimation(false));

#if HDPipeline
                        if (m_underwaterProbe != null)
                        {
                            m_underwaterProbe.enabled = false;
                        }
#endif

                        if (m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
                        {
#if HDPipeline
                            if (volume != null)
                            {
                                VolumeProfile profile = volume.sharedProfile;
                                if (profile != null)
                                {
                                    VisualEnvironment environment;
                                    if (profile.TryGet(out environment))
                                    {
                                        environment.fogType.value = FogType.Volumetric;
                                    }

                                    VolumetricFog fog;
                                    if (profile.TryGet(out fog))
                                    {
                                        fog.active = true;
                                        fog.albedo.value = storedFogColor;
                                        fog.meanFreePath.value = storedFogDistance;
                                    }

                                    LinearFog linearFog;
                                    if (profile.TryGet(out linearFog))
                                    {
                                        linearFog.active = false;
                                    }
                                }
                            }

#if UNITY_2019_1_OR_NEWER
                            if (postVolume != null)
                            {
                                if (m_underwaterProfile != null)
                                {
                                    postVolume.sharedProfile = m_aboveWaterProfile;
                                }
                                else
                                {
                                    Debug.LogError("Missing underwater profile for HDRP 2019");
                                }
                            }
#endif
#endif
                        }
                        else
                        {
                            RenderSettings.fogColor = storedFogColor;
                            RenderSettings.fogEndDistance = storedFogDistance;
                        }
                        indexNumber = 0;
                        if (mainlight != null)
                        {
                            mainlight.cookie = null;
                        }

                        if (m_submergeSoundFXUp != null && objectAudioSource != null)
                        {
                            objectAudioSource.PlayOneShot(m_submergeSoundFXUp, m_waterSubmergeSounfFXVolume);
                        }

                        if (m_useHorizonFix)
                        {
                            if (horizonObject != null)
                            {
                                MeshRenderer[] theRenders = horizonObject.GetComponentsInChildren<MeshRenderer>();
                                foreach (MeshRenderer rends in theRenders)
                                {
                                    rends.enabled = false;
                                }
                            }
                        }

                        if (m_useUnderwaterparticles)
                        {
                            underwaterParticles.SetActive(false);
                        }
                        
                        if (ambientAudio != null)
                        {
                            AudioSource ambientAudioSource = ambientAudio.GetComponent<AudioSource>();
                            if (ambientAudioSource != null)
                            {
                                ambientAudioSource.volume = 0.5f;
                            }
                        }

                        if (underwaterAudio != null)
                        {
                            AudioSource underwaterAudioSource = underwaterAudio.GetComponent<AudioSource>();
                            if (underwaterAudioSource != null)
                            {
                                underwaterAudioSource.volume = 0f;
                            }
                        }

                        coroutineStatus = false;
                    }
                }
                else if (player.position.y < m_sealevel)
                {
                    if (!coroutineStatus)
                    {
                        StartCoroutine(CausticsAnimation(true));
#if HDPipeline
                        if (m_underwaterProbe != null)
                        {
                            m_underwaterProbe.enabled = true;
                        }
#endif
                        if (m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition2018x)
                        {
#if HDPipeline
                        if (volume != null)
                        {
                            VolumeProfile profile = volume.sharedProfile;
                            if (profile != null)
                            {
                                VisualEnvironment environment;
                                if (profile.TryGet(out environment))
                                {
                                    environment.fogType.value = FogType.Linear;
                                }

                                VolumetricFog fog;
                                if (profile.TryGet(out fog))
                                {
                                    fog.active = false;
                                }

                                LinearFog linearFog;
                                if (profile.TryGet(out linearFog))
                                {
                                    linearFog.active = true;
                                    linearFog.colorMode.value = FogColorMode.ConstantColor;
                                    linearFog.color.value = m_underWaterFogColor;
                                    linearFog.color.overrideState = true;
                                    linearFog.fogStart.value = 0f;
                                    linearFog.fogEnd.value = m_underWaterFogDistance / 1.7f;
                                    linearFog.fogHeightStart.value = 50f;
                                    linearFog.fogHeightEnd.value = 400f;
                                    linearFog.maxFogDistance.value = 25f;
                                }
                            }
                        }

#if UNITY_2019_1_OR_NEWER
                            if (postVolume != null)
                            {
                                if (m_underwaterProfile != null)
                                {
                                    postVolume.sharedProfile = m_underwaterProfile;
                                }
                                else
                                {
                                    Debug.LogError("Missing underwater profile for HDRP 2019");
                                }
                            }
#endif
#endif
                        }
                        else
                        {
                            RenderSettings.fogColor = m_underWaterFogColor;
                            RenderSettings.fogEndDistance = m_underWaterFogDistance;
                        }

                        if (objectAudioSource != null)
                        {
                            if (m_submergeSoundFXDown != null)
                            {
                                objectAudioSource.PlayOneShot(m_submergeSoundFXDown, m_waterSubmergeSounfFXVolume);
                            }
                        }

                        if (m_useHorizonFix)
                        {
                            if (horizonObject != null)
                            {
                                MeshRenderer[] theRenders = horizonObject.GetComponentsInChildren<MeshRenderer>();
                                foreach (MeshRenderer rends in theRenders)
                                {
                                    rends.enabled = true;
                                }
                            }
                        }

                        if (m_useUnderwaterparticles)
                        {
                            underwaterParticles.SetActive(true);
                            underwaterParticles.GetComponent<ParticleSystem>().Play();
                        }
                        
                        if (ambientAudio != null)
                        {
                            AudioSource ambientAudioSource = ambientAudio.GetComponent<AudioSource>();
                            if (ambientAudioSource != null)
                            {
                                ambientAudioSource.volume = 0f;
                            }
                        }

                        if (underwaterAudio != null)
                        {
                            AudioSource underwaterAudioSource = underwaterAudio.GetComponent<AudioSource>();
                            if (underwaterAudioSource != null)
                            {
                                underwaterAudioSource.volume = m_underwaterSoundFXVolume;
                            }
                        }

                        coroutineStatus = true;
                    }
                }
            }
        }
        #endregion

        #region Caustics Setup
        /// <summary>
        /// Plays the underwater caustic animations
        /// </summary>
        /// <param name="systemOn"></param>
        /// <returns></returns>
        IEnumerator CausticsAnimation(bool systemOn)
        {
            while (systemOn)
            {          
                if (mainlight != null)
                {
                    mainlight.cookie = m_cookies[indexNumber];
                    indexNumber++;
                }

                if (indexNumber == m_cookies.Length)
                {
                    indexNumber = 0;
                }
                yield return new WaitForSeconds(1 / m_framesPerSecond);
            }
        }
        
        /// <summary>
        /// Applies the required components
        /// </summary>
        public void LoadCaustics()
        {
#if UNITY_EDITOR
            m_cookies = new Texture2D[16];
            m_cookies[0] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_001.tif"));
            m_cookies[1] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_002.tif"));
            m_cookies[2] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_003.tif"));
            m_cookies[3] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_004.tif"));
            m_cookies[4] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_005.tif"));
            m_cookies[5] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_006.tif"));
            m_cookies[6] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_007.tif"));
            m_cookies[7] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_008.tif"));
            m_cookies[8] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_009.tif"));
            m_cookies[9] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_010.tif"));
            m_cookies[10] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_011.tif"));
            m_cookies[11] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_012.tif"));
            m_cookies[12] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_013.tif"));
            m_cookies[13] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_014.tif"));
            m_cookies[14] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_015.tif"));
            m_cookies[15] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_016.tif"));

            m_submergeSoundFXDown = AssetDatabase.LoadAssetAtPath<AudioClip>(GaiaUtils.GetAssetPath("Gaia Ambient Submerge Down.mp3"));
            m_submergeSoundFXUp = AssetDatabase.LoadAssetAtPath<AudioClip>(GaiaUtils.GetAssetPath("Gaia Ambient Submerge Up.mp3"));
            underwaterParticlesStored = AssetDatabase.LoadAssetAtPath<GameObject>(GaiaUtils.GetAssetPath("Underwater Particles Effects.prefab"));
            horizonObjectStored = AssetDatabase.LoadAssetAtPath<GameObject>(GaiaUtils.GetAssetPath("Ambient Underwater Horizon.prefab"));
#endif
        }
        #endregion
    }
}