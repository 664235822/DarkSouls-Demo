using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraPlayableBehaviour : PlayableBehaviour
{
    public Camera myCamera;
    public float myFloat;

    public override void OnPlayableCreate (Playable playable)
    {
        
    }
}
