using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;

public class HasGrabbedObject : ConditionBase
{
    Player player;

    protected override void OnInit()
    {
        player = Owner.GetComponent<Player>();
    }

    protected override bool OnUpdate()
    {
        return player.hasGrabbedObj;
    }
}
