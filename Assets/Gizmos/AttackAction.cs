using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AttackAction : Action
{
    public SharedFloat waitTime;
    private float time;

    public override void OnStart()
    {
        GetComponent<DummyInput>().attackRight = true;
    }

    public override TaskStatus OnUpdate()
    {
        if (time >= waitTime.Value)
        {
            time = 0;
            return TaskStatus.Success;
        }

        time += Time.deltaTime;
        return TaskStatus.Running;
    }
}