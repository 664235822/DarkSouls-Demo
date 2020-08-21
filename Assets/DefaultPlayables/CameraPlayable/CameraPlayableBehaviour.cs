using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class CameraPlayableBehaviour : PlayableBehaviour
{
    public ActorManager actorManager;

    PlayableDirector playableDirector;

    public override void OnGraphStart(Playable playable)
    {
        playableDirector = (PlayableDirector) playable.GetGraph().GetResolver();
    }
    
    public override void OnGraphStop(Playable playable)
    {
        if (playableDirector != null)
        {
            playableDirector.playableAsset = null;
        }
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        actorManager.Lock(true);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        actorManager.Lock(false);
    }
}
