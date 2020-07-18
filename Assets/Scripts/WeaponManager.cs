using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public BoxCollider weaponCollider;
    
    public void WeaponEnable()
    {
        weaponCollider.enabled = true;
    }

    public void WeaponDisable()
    {
        weaponCollider.enabled = false;
    }
}
