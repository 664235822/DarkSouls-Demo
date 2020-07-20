using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public ActorManager actorManager;

    [SerializeField] GameObject leftHandle;
    [SerializeField] GameObject rightHandle;
    
    [SerializeField] BoxCollider leftWeaponCollider;
    [SerializeField] BoxCollider rightWeaponCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        leftHandle = transform.DeepFind("WeaponHandleL").gameObject;
        rightHandle = transform.DeepFind("WeaponHandleR").gameObject;

        leftWeaponCollider = leftHandle.transform.GetComponentInChildren<BoxCollider>();
        rightWeaponCollider = rightHandle.transform.GetComponentInChildren<BoxCollider>();
    }
    
    public void WeaponEnable()
    {
        if (actorManager.actorController.CheckStateTag("attackR"))
        {
            rightWeaponCollider.enabled = true;
        }
        else if (actorManager.actorController.CheckStateTag("attackL"))
        {
            leftWeaponCollider.enabled = true;
        }

    }

    public void WeaponDisable()
    {
        rightWeaponCollider.enabled = false;
        leftWeaponCollider.enabled = false;
    }
}
