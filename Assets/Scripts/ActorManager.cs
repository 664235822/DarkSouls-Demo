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

    public void TryDoDamage()
    {
        if (stateManager.HP > 0)
        {
            stateManager.AddHP(-5.0f);
        }
       
        
    }

    public void Hit()
    {
        actorController.IssueTrigger("hit");
    }

    public void Die()
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
