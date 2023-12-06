using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;

public class BTRunner : MonoBehaviour
{
    [SerializeField] BehaviorTree tree;
	private bool runTree = true;

    private bool hasPrintedWarning = false;

    void Awake()
    {
        tree.SetOwner(gameObject);
        tree.ResetInits();
    }

    void Update()
    {
		if (!runTree) return;
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

	public void ReStartTree() { runTree = true; }
	public void StopTree() { runTree = false; }
}
