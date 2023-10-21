using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using CleverCrow.Fluid.BTs.Tasks;

public class GrabObject : ActionBase
{
    [SerializeField] Transform targetObject;
    Player player;

    protected override void OnInit()
    {
        player = Owner.GetComponent<Player>();
    }

    protected override TaskStatus OnUpdate()
    {
        targetObject.parent = Owner.transform;
        player.hasGrabbedObj = true;
        return TaskStatus.Success;
    }

}
