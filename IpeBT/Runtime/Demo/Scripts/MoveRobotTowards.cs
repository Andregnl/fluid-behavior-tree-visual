using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using CleverCrow.Fluid.BTs.Tasks;

public class MoveRobotTowards : ActionBase
{
    [SerializeField] float speed = 10.0f;
    protected GameObject objectToFollow = null;

    protected override void OnInit()
    {
        objectToFollow = GameObject.FindWithTag("Object");
    }
    
    protected override TaskStatus OnUpdate()
    {
        if (objectToFollow == null) return TaskStatus.Failure;

        Owner.transform.position = Vector2.MoveTowards(Owner.transform.position,
                                                       objectToFollow.transform.position,
                                                       speed * Time.deltaTime);
        return TaskStatus.Success;
    }
}
