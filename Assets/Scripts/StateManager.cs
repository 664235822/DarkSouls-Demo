using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public ActorManager actorManager;

    public float HP = 15.0f;
    public float HP_Max = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        AddHP(0);   
    }
    
    public void AddHP(float value)
    {
        HP += value;
        HP = Mathf.Clamp(HP, 0, HP_Max);
    }
}
