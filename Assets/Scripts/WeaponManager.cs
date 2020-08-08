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

    [SerializeField] WeaponController leftWeaponController;
    [SerializeField] WeaponController rightWeaponController;
    
    // Start is called before the first frame update
    void Start()
    {
        leftHandle = transform.DeepFind("WeaponHandleL").gameObject;
        rightHandle = transform.DeepFind("WeaponHandleR").gameObject;

        leftWeaponController = BindWeaponController(leftHandle);
        rightWeaponController = BindWeaponController(rightHandle);

        leftWeaponCollider = leftHandle.transform.GetComponentInChildren<BoxCollider>();
        rightWeaponCollider = rightHandle.transform.GetComponentInChildren<BoxCollider>();
    }

    public WeaponController BindWeaponController(GameObject target)
    {
        WeaponController temp;
        temp = target.GetComponent<WeaponController>();
        if (temp == null)
        {
            temp = target.AddComponent<WeaponController>();
        }

        temp.weaponManager = this;
        return temp;
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

    public void CounterBackEnable()
    {
        actorManager.SetIsCounterBack(true);
    }

    public void CounterBackDisable()
    {
        actorManager.SetIsCounterBack(false);
    }
}
