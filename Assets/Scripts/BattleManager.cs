using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface
{
    private void OnTriggerEnter(Collider other)
    {
        WeaponController targetController = other.GetComponentInParent<WeaponController>();

        if (targetController == null) return;

        GameObject attacker = targetController.weaponManager.actorManager.gameObject;
        GameObject receiver = actorManager.gameObject;

        Vector3 attackingDir = receiver.transform.position - attacker.transform.position;
        float attackingAngle = Vector3.Angle(attacker.transform.forward, attackingDir);

        Vector3 counterDir = attacker.transform.position - receiver.transform.position;
        float counterAngle = Vector3.Angle(receiver.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(attacker.transform.forward, receiver.transform.forward);

        bool attackValid = (attackingAngle < 45.0f);
        bool counterValid = (counterAngle < 30.0f && Mathf.Abs(counterAngle2 - 180.0f) < 30.0f);
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            if (attackingAngle <= 45.0f)
            {
                actorManager.TryDoDamage(targetController, attackValid, counterValid);
            }
        }
    }
    
    
}
