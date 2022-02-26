using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class StopAttackAction : Action
{
    public override void OnStart()
    {
        GetComponent<DummyInput>().attackRight = false;
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
