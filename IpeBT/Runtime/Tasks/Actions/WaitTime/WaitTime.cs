using UnityEngine;

namespace CleverCrow.Fluid.BTs.Tasks.Actions {
    /// <summary>
    /// Return continue until the time has passed
    /// </summary>
    public class WaitTime : ActionBase {
        public ITimeMonitor _timeMonitor;
        private float _timePassed;

        [HideInInspector] public bool testMode = false;
        public float time = 1;
        
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/Hourglass.png";

        public WaitTime (ITimeMonitor timeMonitor) {
            _timeMonitor = timeMonitor;
        }

        protected override void OnInit()
        {
            if (testMode) return;

            _timeMonitor = new TimeMonitor();
        }

        protected override void OnStart () {
            _timePassed = 0;
        }

        protected override TaskStatus OnUpdate () {
            _timePassed += _timeMonitor.DeltaTime;

            if (_timePassed < time) {
                return TaskStatus.Continue;                
            }

            return TaskStatus.Success;
        }
    }
}
