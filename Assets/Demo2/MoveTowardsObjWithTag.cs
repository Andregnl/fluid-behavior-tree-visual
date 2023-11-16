using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using CleverCrow.Fluid.BTs.Tasks;

public class MoveTowardsObjWithTag : ActionBase
{
    [SerializeField] float speed = 10.0f;
    [SerializeField] string tagToFollow = "";
    [SerializeField] float eps = 0.1f;
    protected GameObject objectToFollow = null;

    protected override void OnInit()
    {
        objectToFollow = GameObject.FindWithTag(tagToFollow);
    }

    protected override TaskStatus OnUpdate()
    {
        if (objectToFollow == null) return TaskStatus.Failure;

        Owner.transform.position = Vector2.MoveTowards(Owner.transform.position,
                                                       objectToFollow.transform.position,
                                                       speed * Time.deltaTime);
        if (IsNearObject())
            return TaskStatus.Success;

        return TaskStatus.Continue;
    }

    private bool IsNearObject()
    {
        if ((Owner.transform.position - objectToFollow.transform.position).magnitude < eps)
        {
            return true;
        }

        return false;
    }
}
