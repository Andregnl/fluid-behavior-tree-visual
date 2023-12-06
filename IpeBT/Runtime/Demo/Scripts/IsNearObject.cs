using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;

public class IsNearObject : ConditionBase
{
    [SerializeField] float eps = 0.01f;
    GameObject targetObject = null;
    Player player;

    protected override void OnInit()
    {
        player = Owner.GetComponent<Player>();
        Debug.Log("NOME: " + player.gameObject.name);
        targetObject = GameObject.FindWithTag("Object");
    }

    protected override bool OnUpdate()
    {
        if (player.hasGrabbedObj) return false;

        if ((Owner.transform.position - targetObject.transform.position).magnitude < eps)
            return true;

        return false;
    }
}
