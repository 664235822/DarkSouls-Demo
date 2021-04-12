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

    public void OnSword()
    {
        weaponManager.UnloadWeapon("R");
        weaponManager.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Sword", "R", weaponManager));
        weaponManager.LeftWeaponEnabled(true);
        weaponManager.ChangeDualHands(false);
    }

    public void OnFalchion()
    {
        weaponManager.UnloadWeapon("R");
        weaponManager.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Falchion", "R", weaponManager));
        weaponManager.LeftWeaponEnabled(false);
        weaponManager.ChangeDualHands(true);
    }

    public void OnRace()
    {
        weaponManager.UnloadWeapon("R");
        weaponManager.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Mace", "R", weaponManager));
        weaponManager.LeftWeaponEnabled(true);
        weaponManager.ChangeDualHands(false);
    }

    public void OnClearAll()
    {
        weaponManager.UnloadWeapon("R");
        weaponManager.LeftWeaponEnabled(false);
    }
}
