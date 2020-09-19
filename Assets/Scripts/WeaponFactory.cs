using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory
{
    private readonly DataBase weaponDataBase;

    public WeaponFactory(DataBase weaponDataBase)
    {
        this.weaponDataBase = weaponDataBase;
    }

    public GameObject CreateWeapon(string weaponName, Vector3 pos, Quaternion rot)
    {
        GameObject weapon = Resources.Load(weaponName) as GameObject;
        GameObject obj = GameObject.Instantiate(weapon, pos, rot);
        WeaponData weaponData = obj.AddComponent<WeaponData>();
        weaponData.ATK = weaponDataBase.weaponJson[weaponName]["ATK"].f;

        return obj;
    }

    public BoxCollider CreateWeapon(string weaponName, string side, WeaponManager weaponManager)
    {
        WeaponController weaponController;
        switch (side)
        {
            case "L":
                weaponController = weaponManager.leftWeaponController;
                break;
            case "R":
                weaponController = weaponManager.rightWeaponController;
                break;
            default:
                return null;
        }

        GameObject weapon = Resources.Load(weaponName) as GameObject;
        GameObject obj = GameObject.Instantiate(weapon, weaponController.transform);
        WeaponData weaponData = obj.AddComponent<WeaponData>();
        weaponData.ATK = weaponDataBase.weaponJson[weaponName]["ATK"].f;
        weaponController.WeaponData = weaponData;

        return obj.GetComponent<BoxCollider>();
    }
}