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
        GameObject receiver = actorManager.actorController.model.gameObject;

        bool attackValid = CheckAngleTarget(receiver, attacker, 45.0f);
        bool counterValid = CheckAnglePlayer(receiver, attacker, 30.0f);

        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            actorManager.TryDoDamage(targetController, attackValid, counterValid);
        }
    }

    public static bool CheckAnglePlayer(GameObject player, GameObject target, float playerAngle)
    {
        Vector3 counterDir = target.transform.position - player.transform.position;
        float counterAngle = Vector3.Angle(player.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(target.transform.forward, player.transform.forward);

        bool counterValid = (counterAngle < playerAngle && Mathf.Abs(counterAngle2 - 180.0f) < playerAngle);
        return counterValid;
    }

    public static bool CheckAngleTarget(GameObject player, GameObject target, float targetAngle)
    {
        Vector3 attackingDir = player.transform.position - target.transform.position;
        float attackingAngle = Vector3.Angle(target.transform.forward, attackingDir);

        bool attackValid = (attackingAngle < targetAngle);
        return attackValid;
    }


}
