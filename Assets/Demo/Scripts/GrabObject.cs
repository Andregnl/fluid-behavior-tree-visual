using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using CleverCrow.Fluid.BTs.Tasks;

public class GrabObject : ActionBase
{
    Player player;
    GameObject targetObject = null;

    protected override void OnInit()
    {
        player = Owner.GetComponent<Player>();
        targetObject = GameObject.FindWithTag("Object");
    }

    protected override TaskStatus OnUpdate()
    {
        targetObject.transform.parent = Owner.transform;
        player.hasGrabbedObj = true;
        return TaskStatus.Success;
    }

}
