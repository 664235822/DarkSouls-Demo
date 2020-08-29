using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraPlayableClip : PlayableAsset, ITimelineClipAsset
{
    public CameraPlayableBehaviour template = new CameraPlayableBehaviour ();
    public ExposedReference<ActorManager> actorManager;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CameraPlayableBehaviour>.Create (graph, template);
        CameraPlayableBehaviour clone = playable.GetBehaviour ();
        clone.actorManager = actorManager.Resolve (graph.GetResolver ());
        return playable;
    }
}
