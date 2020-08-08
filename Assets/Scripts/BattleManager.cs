using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : MonoBehaviour
{
    public ActorManager actorManager;
    
    private void OnTriggerEnter(Collider other)
    {
        WeaponController targetController = other.GetComponentInParent<WeaponController>();
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            actorManager.TryDoDamage(targetController);
        }
    }
    
    
}
