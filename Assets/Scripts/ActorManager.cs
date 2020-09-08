using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public ActorController actorController;
    public BattleManager battleManager;
    public WeaponManager weaponManager;
    public StateManager stateManager;
    public DirectorManager directorManager;
    public InteractionManager interactionManager;

    public void TryDoDamage(WeaponController target, bool attackValid, bool counterValid)
    {
        if (stateManager.isImmortal)
        {

        }
        else if (stateManager.isCounterBack && counterValid)
        {
            target.weaponManager.actorManager.Stunned();
        }
        else if (stateManager.isDefence)
        {
            Blocked();
        }
        else
        {
            if (stateManager.HP > 0 && attackValid)
            {
                stateManager.AddHP(-5.0f);
                if (stateManager.HP > 0)
                {
                    Hit();
                }
                else
                {
                    Die();
                }
            }
        }
    }

    private void Blocked()
    {
        actorController.IssueTrigger("blocked");
    }
    
    private void Hit()
    {
        actorController.IssueTrigger("hit");
    }

    public void Stunned()
    {
        actorController.IssueTrigger("stunned");
    }

    public void SetIsCounterBack(bool value)
    {
        stateManager.isCounterBack = value;
    }
    
    private void Die()
    {
        actorController.IssueTrigger("die");
        actorController.playerInput.inputEnabled = false;
        if (actorController.cameraController.lockState)
        {
            actorController.cameraController.lockState = false;
        }
        actorController.cameraController.enabled = false;
    }

    public void Lock(bool value)
    {
        actorController.SetBool("lock", value);
    }

    public void OnAction()
    {
        if (interactionManager.list.Count == 0) return;

        if (interactionManager.list[0].isActive)
        {
            if (interactionManager.list[0].eventName == "frontStab")
            {
                directorManager.PlayFrontStab(this, interactionManager.list[0].actorManager);
            }
            else if (interactionManager.list[0].eventName == "openBox")
            {
                if (BattleManager.CheckAnglePlayer(actorController.model.gameObject,
                    interactionManager.list[0].actorManager.gameObject, 30.0f))
                {
                    interactionManager.list[0].isActive = false;
                    actorController.model.LookAt(interactionManager.list[0].actorManager.transform, Vector3.up);
                    directorManager.PlayOpenBox(this, interactionManager.list[0].actorManager);
                }
            }
            else if (interactionManager.list[0].eventName == "openLevel")
            {
                if (BattleManager.CheckAnglePlayer(actorController.model.gameObject,
                    interactionManager.list[0].actorManager.gameObject, 30.0f))
                {
                    interactionManager.list[0].isActive = false;
                    actorController.model.LookAt(interactionManager.list[0].actorManager.transform, Vector3.up);
                    directorManager.PlayOpenLevel(this, interactionManager.list[0].actorManager);
                }
            }
        }

        interactionManager.list.Clear();
        
    }
}
