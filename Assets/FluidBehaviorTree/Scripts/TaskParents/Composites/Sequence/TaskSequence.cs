﻿using Adnc.FluidBT.Tasks;

namespace Adnc.FluidBT.TaskParents {
    public class TaskSequence : TaskParentBase {
        private int childIndex;

        protected override TaskStatus OnUpdate () {
            if (AbortType.HasFlag(AbortType.Self)
                && childIndex > 0
                && children[0].Update() == TaskStatus.Failure) {
                children[childIndex].End();
                Reset();
                return TaskStatus.Failure;
            }

            for (var i = childIndex; i < children.Count; i++) {
                var child = children[childIndex];

                if (child.Enabled) {
                    var status = child.Update();
                    if (status != TaskStatus.Success) {
                        return status;
                    }
                }

                childIndex++;
            }

            return TaskStatus.Success;
        }

        public override void Reset (bool hardReset = false) {
            childIndex = 0;

            base.Reset(hardReset);
        }
    }
}
