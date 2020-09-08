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
    public TimelineAsset openBox;
    public TimelineAsset openLevel;

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

    public void PlayFrontStab(ActorManager attacker, ActorManager victim)
    {
        if (playableDirector.state == PlayState.Playing) return;

        playableDirector.playableAsset = Instantiate(frontStab);

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
                    cameraPlayableClip.actorManager.exposedName = System.Guid.NewGuid().ToString();
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
                    cameraPlayableClip.actorManager.exposedName = System.Guid.NewGuid().ToString();
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

    public void PlayOpenBox(ActorManager player, ActorManager box)
    {
        if (playableDirector.state == PlayState.Playing) return;

        playableDirector.playableAsset = Instantiate(openBox);

        TimelineAsset timeline = (TimelineAsset) playableDirector.playableAsset;

        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == "Player Script")
            {
                playableDirector.SetGenericBinding(track, player);
                foreach (var clip in track.GetClips())
                {
                    CameraPlayableClip cameraPlayableClip = (CameraPlayableClip) clip.asset;
                    CameraPlayableBehaviour cameraPlayableBehaviour = cameraPlayableClip.template;
                    cameraPlayableClip.actorManager.exposedName = System.Guid.NewGuid().ToString();
                    playableDirector.SetReferenceValue(cameraPlayableClip.actorManager.exposedName, player);
                }
            }
            else if (track.name == "Box Script")
            {
                playableDirector.SetGenericBinding(track, box);
                foreach (var clip in track.GetClips())
                {
                    CameraPlayableClip cameraPlayableClip = (CameraPlayableClip) clip.asset;
                    CameraPlayableBehaviour cameraPlayableBehaviour = cameraPlayableClip.template;
                    cameraPlayableClip.actorManager.exposedName = System.Guid.NewGuid().ToString();
                    playableDirector.SetReferenceValue(cameraPlayableClip.actorManager.exposedName, box);
                }
            }
            else if (track.name == "Player Animation")
            {
                playableDirector.SetGenericBinding(track, player.actorController.anim);
            }
            else if (track.name == "Box Animation")
            {
                playableDirector.SetGenericBinding(track, box.actorController.anim);
            }
        }

        playableDirector.Evaluate();

        playableDirector.Play();
    }
    
     public void PlayOpenLevel(ActorManager player, ActorManager box)
    {
        if (playableDirector.state == PlayState.Playing) return;

        playableDirector.playableAsset = Instantiate(openLevel);

        TimelineAsset timeline = (TimelineAsset) playableDirector.playableAsset;

        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == "Player Script")
            {
                playableDirector.SetGenericBinding(track, player);
                foreach (var clip in track.GetClips())
                {
                    CameraPlayableClip cameraPlayableClip = (CameraPlayableClip) clip.asset;
                    CameraPlayableBehaviour cameraPlayableBehaviour = cameraPlayableClip.template;
                    cameraPlayableClip.actorManager.exposedName = System.Guid.NewGuid().ToString();
                    playableDirector.SetReferenceValue(cameraPlayableClip.actorManager.exposedName, player);
                }
            }
            else if (track.name == "Level Script")
            {
                playableDirector.SetGenericBinding(track, box);
                foreach (var clip in track.GetClips())
                {
                    CameraPlayableClip cameraPlayableClip = (CameraPlayableClip) clip.asset;
                    CameraPlayableBehaviour cameraPlayableBehaviour = cameraPlayableClip.template;
                    cameraPlayableClip.actorManager.exposedName = System.Guid.NewGuid().ToString();
                    playableDirector.SetReferenceValue(cameraPlayableClip.actorManager.exposedName, box);
                }
            }
            else if (track.name == "Player Animation")
            {
                playableDirector.SetGenericBinding(track, player.actorController.anim);
            }
            else if (track.name == "Level Animation")
            {
                playableDirector.SetGenericBinding(track, box.actorController.anim);
            }
        }

        playableDirector.Evaluate();

        playableDirector.Play();
    }

}
