using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraPlayableBehaviour : PlayableBehaviour
{
    public Camera myCamera;
    public float myFloat;

    ActorManager actorManager;

    public override void OnPlayableCreate(Playable playable)
    {
        PlayableDirector playableDirector = (PlayableDirector) playable.GetGraph().GetResolver();
        foreach (var track in playableDirector.playableAsset.outputs)
        {
            if (track.streamName == "Attack Script" || track.streamName == "Victim Script")
            {
                actorManager = (ActorManager) playableDirector.GetGenericBinding(track.sourceObject);
            }
        }
    }

    public override void OnGraphStart(Playable playable)
    {
        if (actorManager) actorManager.Lock(true);
    }

    public override void OnGraphStop(Playable playable)
    {
        if (actorManager) actorManager.Lock(false);
    }
}
