using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class DirectorManager : IActorManagerInterface
{
    public PlayableDirector playableDirector;
    public ActorManager attacker;
    public ActorManager victim;

    public TimelineAsset timelineAsset;

    // Start is called before the first frame update
    void Start()
    {
        playableDirector.playableAsset = timelineAsset;

        foreach (var track in playableDirector.playableAsset.outputs)
        {
            if (track.streamName == "Attacker Animation")
            {
                playableDirector.SetGenericBinding(track.sourceObject, attacker.actorController.anim);
            }
            else if (track.streamName == "Victim Animation")
            {
                playableDirector.SetGenericBinding(track.sourceObject, victim.actorController.anim);
            }
            else if (track.streamName == "Attacker Script")
            {
                playableDirector.SetGenericBinding(track.sourceObject, attacker);
            }
            else if (track.streamName == "Victim Script")
            {
                playableDirector.SetGenericBinding(track.sourceObject, victim);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && gameObject.tag == "Player")
        {
            playableDirector.Play();
        }
    }
}
