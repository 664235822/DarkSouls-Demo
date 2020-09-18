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
        GameObject obj= GameObject.Instantiate(weapon, pos, rot);
        WeaponData weaponData = obj.AddComponent<WeaponData>();
        weaponData.ATK = weaponDataBase.weaponJson[weaponName]["ATK"].f;

        return obj;
    }

    public GameObject CreateWeapon(string weaponName, Transform parent)
    {
        GameObject weapon = Resources.Load(weaponName) as GameObject;
        GameObject obj= GameObject.Instantiate(weapon, parent);
        WeaponData weaponData = obj.AddComponent<WeaponData>();
        weaponData.ATK = weaponDataBase.weaponJson[weaponName]["ATK"].f;

        return obj;
    }
}
