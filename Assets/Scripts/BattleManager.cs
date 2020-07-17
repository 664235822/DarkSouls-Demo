using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("hit");
    }
    
    
}
