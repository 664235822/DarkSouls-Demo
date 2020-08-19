using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : IActorManagerInterface
{
    public CapsuleCollider interactionCollider;

    public List<EventCasterManager> list = new List<EventCasterManager>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        EventCasterManager[] eventCasterManagers = other.GetComponents<EventCasterManager>();
        foreach (var eventCaster in eventCasterManagers)
        {
            if (!list.Contains(eventCaster))
            {
                list.Add(eventCaster);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        EventCasterManager[] eventCasterManagers = other.GetComponents<EventCasterManager>();
        foreach (var eventCaster in eventCasterManagers)
        {
            if (!list.Contains(eventCaster))
            {
                list.Remove(eventCaster);
            }
        }
    }
}
