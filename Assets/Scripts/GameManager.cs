using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance => instance;

    private DataBase weaponDataBase;
    private WeaponFactory weaponFactory;

    [SerializeField] private WeaponManager weaponManager;

    // Start is called before the first frame update
    void Start()
    {
        weaponDataBase = new DataBase();
        weaponFactory = new WeaponFactory(weaponDataBase);
        
        weaponManager.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Sword", "R", weaponManager));
        weaponManager.ChangeDualHands(false);
    }

    private void Awake()
    {
        if (instance != null) return;
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 30), "R: Sword"))
        {
            weaponManager.UnloadWeapon("R");
            weaponManager.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Sword", "R", weaponManager));
            weaponManager.ChangeDualHands(false);
        }
        if (GUI.Button(new Rect(10, 50, 150, 30), "R: Falchion"))
        {
            weaponManager.UnloadWeapon("R");
            weaponManager.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Falchion", "R", weaponManager));
            weaponManager.ChangeDualHands(true);
        }
        if (GUI.Button(new Rect(10, 90, 150, 30), "R: Mace"))
        {
            weaponManager.UnloadWeapon("R");
            weaponManager.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Mace", "R", weaponManager));
            weaponManager.ChangeDualHands(false);
        }
        if (GUI.Button(new Rect(10, 130, 150, 30), "R: Clear All Weapon"))
        {
            weaponManager.UnloadWeapon("R");
        }
    }
}
