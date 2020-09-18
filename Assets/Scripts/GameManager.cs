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

    // Start is called before the first frame update
    void Start()
    {
        weaponDataBase = new DataBase();
        weaponFactory = new WeaponFactory(weaponDataBase);
    }

    private void Awake()
    {
        if (instance != null) return;
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
