using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public ActorController actorController;
    public BattleManager battleManager;
    public WeaponManager weaponManager;
    public ActorManager actorManager;
    public StateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TryDoDamage(WeaponController target)
    {
        if (stateManager.isImmortal)
        {
            
        }
        else if (stateManager.isCounterBack)
        {
            target.weaponManager.actorManager.Stunned();
        }
        else if (stateManager.isDefence)
        {
            Blocked();
        }
        else
        {
            if (stateManager.HP > 0)
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

}
