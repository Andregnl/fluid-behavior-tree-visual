using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;

public class BTRunner : MonoBehaviour
{
    [SerializeField] BehaviorTree tree;

    private bool hasPrintedWarning = false;

    void Update()
    {
        if (tree.Root == null)
        {
            if (!hasPrintedWarning)
            {
                Debug.LogWarning("Root shouldn't be null");
                hasPrintedWarning = true;
            }

            return;
        }

        tree.Tick();
    }
}
