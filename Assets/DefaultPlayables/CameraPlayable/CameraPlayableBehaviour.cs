using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class CameraPlayableBehaviour : PlayableBehaviour
{
    public ActorManager actorManager;
    
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        actorManager.Lock(true);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        actorManager.Lock(false);
    }
}
