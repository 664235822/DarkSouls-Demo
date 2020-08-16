using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityStandardAssets.CrossPlatformInput;

public class DirectorManager : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public Animator attacker;
    public Animator victim;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var track in playableDirector.playableAsset.outputs)
            {
                if (track.streamName == "Attacker Animation")
                {
                    playableDirector.SetGenericBinding(track.sourceObject, attacker);
                }
                else if (track.streamName == "VicTim Animation")
                {
                    playableDirector.SetGenericBinding(track.sourceObject, victim);
                }

            }

            playableDirector.Play();
        }
    }
}
