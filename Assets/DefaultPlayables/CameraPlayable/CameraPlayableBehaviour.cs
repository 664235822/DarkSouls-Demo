using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraPlayableBehaviour : PlayableBehaviour
{
    public Camera myCamera;
    public float myFloat;

    PlayableDirector playableDirector;

    public override void OnPlayableCreate(Playable playable)
    {

    }

    public override void OnGraphStart(Playable playable)
    {
        playableDirector = (PlayableDirector) playable.GetGraph().GetResolver();
        foreach (var track in playableDirector.playableAsset.outputs)
        {
            if (track.streamName == "Attacker Script" || track.streamName == "Victim Script")
            {
                ActorManager actorManager = (ActorManager) playableDirector.GetGenericBinding(track.sourceObject);
                actorManager.Lock(true);
            }
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        foreach (var track in playableDirector.playableAsset.outputs)
        {
            if (track.streamName == "Attacker Script" || track.streamName == "Victim Script")
            {
                ActorManager actorManager = (ActorManager) playableDirector.GetGenericBinding(track.sourceObject);
                actorManager.Lock(false);
            }
        }
    }
}
