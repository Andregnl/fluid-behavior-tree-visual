using UnityEngine;
using System.Collections;
using CleverCrow.Fluid.BTs.Tasks;
using Codice.CM.Client.Differences.Merge;
using CleverCrow.Fluid.BTs.Trees;

namespace CleverCrow.Fluid.BTs.Decorators
{
    public class Interrupt : DecoratorBase
    {
        public bool condition = true;

        private IEnumerator conditionChecker;
        private CoroutineHandler coroutineHandler;

        public BehaviorTree tree;

        // public override string IconPath { get; } = $"{PACKAGE_ROOT}/Invert.png";

        public class CoroutineHandler : MonoBehaviour
        {

        }

        protected override TaskStatus OnUpdate()
        {

            Debug.Log("Entrei no Interrupt");

            if (condition)
            {
                var childStatus = Child.Update();
                return childStatus;
            }

            if (coroutineHandler != null)
            {
                coroutineHandler.StopAllCoroutines();
                conditionChecker = CheckCondition();
                coroutineHandler.StartCoroutine(conditionChecker);
            }

            return TaskStatus.Success;
        }

        private IEnumerator CheckCondition()
        {
            bool keepChecking = true;

            while (keepChecking)
            {
                yield return new WaitForSeconds(1);
                Debug.Log("Checando condition...");

                if (condition)
                {
                    keepChecking = false;
                    tree.ResetTree();
                }
            }
        }

    }
}
