using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance => instance;

    // Start is called before the first frame update
    void Start()
    {
        GameObject weapon = Resources.Load("Sword") as GameObject;
        Instantiate(weapon, Vector3.zero, Quaternion.identity);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
