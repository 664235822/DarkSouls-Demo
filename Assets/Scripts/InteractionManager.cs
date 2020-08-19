using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : IActorManagerInterface
{
    public CapsuleCollider interactionCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        EventCasterManager[] eventCasterManagers = other.GetComponents<EventCasterManager>();
        foreach (var eventCaster in eventCasterManagers)
        {
            
        }
    }
}
