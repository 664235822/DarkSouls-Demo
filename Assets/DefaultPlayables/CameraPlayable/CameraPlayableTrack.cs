using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(CameraPlayableClip))]
[TrackBindingType(typeof(ActorManager))]
public class CameraPlayableTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CameraPlayableMixerBehaviour>.Create (graph, inputCount);
    }
}
