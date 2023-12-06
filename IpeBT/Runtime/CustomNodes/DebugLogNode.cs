using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;

public class DebugLogNode : ActionBase
{
    public string message = "Debug Log Node";
    protected override TaskStatus OnUpdate()
    {
        Debug.Log(message);
        return TaskStatus.Success;
    }
}
