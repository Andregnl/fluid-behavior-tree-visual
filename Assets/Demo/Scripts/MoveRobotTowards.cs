using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using CleverCrow.Fluid.BTs.Tasks;

public class MoveRobotTowards : ActionBase
{
    [SerializeField] Transform transformToFollow;

    [SerializeField] float speed = 10.0f;

    protected override void OnInit()
    {

    }
    
    protected override TaskStatus OnUpdate()
    {
        Owner.transform.position = Vector2.MoveTowards(Owner.transform.position,
                                                       transformToFollow.position,
                                                       speed * Time.deltaTime);
        return TaskStatus.Success;
    }
}
