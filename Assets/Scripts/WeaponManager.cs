using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : IActorManagerInterface
{
    [SerializeField] GameObject leftHandle;
    [SerializeField] GameObject rightHandle;
    
    [SerializeField] BoxCollider leftWeaponCollider;
    [SerializeField] BoxCollider rightWeaponCollider;

    public WeaponController leftWeaponController;
    public WeaponController rightWeaponController;
    
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

    private WeaponController BindWeaponController(GameObject target)
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

    public void UpdateWeaponCollider(string side, BoxCollider collider)
    {
        switch (side)
        {
            case "L":
                leftWeaponCollider = collider;
                break;
            case "R":
                rightWeaponCollider = collider;
                break;
        }
    }

    public void UnloadWeapon(string side)
    {
        switch (side)
        {
            case "L":
            {
                foreach (Transform item in leftWeaponController.transform)
                {
                    leftWeaponCollider = null;
                    leftWeaponController.WeaponData = null;
                    Destroy(item.gameObject);
                }

                break;
            }
            case "R":
            {
                foreach (Transform item in rightWeaponController.transform)
                {
                    rightWeaponCollider = null;
                    rightWeaponController.WeaponData = null;
                    Destroy(item.gameObject);
                }

                break;
            }
        }
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
