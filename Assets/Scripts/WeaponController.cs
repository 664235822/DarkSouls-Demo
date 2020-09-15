using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponManager weaponManager;
    public WeaponData WeaponData;

    private void Awake()
    {
        WeaponData = GetComponentInChildren<WeaponData>();
    }
}
