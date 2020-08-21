using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class DirectorManager : IActorManagerInterface
{
    public PlayableDirector playableDirector;

    public TimelineAsset frontStab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && gameObject.tag == "Player")
        {
            playableDirector.Play();
        }
    }

    public void PlayFrontStab(string timelineName, ActorManager attacker, ActorManager victim)
    {
        if (playableDirector.playableAsset != null) return;

        playableDirector.playableAsset = frontStab;

        TimelineAsset timeline = (TimelineAsset) playableDirector.playableAsset;

        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == "Attacker Script")
            {
                playableDirector.SetGenericBinding(track, attacker);
                foreach (var clip in track.GetClips())
                {
                    CameraPlayableClip cameraPlayableClip = (CameraPlayableClip) clip.asset;
                    CameraPlayableBehaviour cameraPlayableBehaviour = cameraPlayableClip.template;
                    playableDirector.SetReferenceValue(cameraPlayableClip.actorManager.exposedName, attacker);
                }
            }
            else if (track.name == "Victim Script")
            {
                playableDirector.SetGenericBinding(track, victim);
                foreach (var clip in track.GetClips())
                {
                    CameraPlayableClip cameraPlayableClip = (CameraPlayableClip) clip.asset;
                    CameraPlayableBehaviour cameraPlayableBehaviour = cameraPlayableClip.template;
                    playableDirector.SetReferenceValue(cameraPlayableClip.actorManager.exposedName, victim);
                }
            }
            else if (track.name == "Attacker Animation")
            {
                playableDirector.SetGenericBinding(track, attacker.actorController.anim);
            }
            else if (track.name == "Victim Animation")
            {
                playableDirector.SetGenericBinding(track, victim.actorController.anim);
            }
        }
        
        playableDirector.Evaluate();

        playableDirector.Play();
    }
    
}
