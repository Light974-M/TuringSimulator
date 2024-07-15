using System.Collections.Generic;
using System.Diagnostics;

namespace TuringSimulator
{
    public class PowerSource : ElecBlock
    {
        protected bool _enabledState = true;

        #region Public API

        public bool EnabledState
        {
            get => _enabledState;
            set => _enabledState = value;
        }

        #endregion


        public PowerSource(Cell positionCell)
        {
            _enabledState = true;
            _positionCell = positionCell;
        }

        public override void UpdateBehaviour()
        {
            Propagate(PropagationOrder.PropagatePower, true);

            if (!_enabledState)
            {
                powerSourceFounded = new List<ElecBlock>();
                Propagate(PropagationOrder.ScanForSource, true);

                if (powerSourceFounded.Count == 0)
                    return;

                for (int i = 0; i < powerSourceFounded.Count; i++)
                    powerSourceFounded[i].UpdateBehaviour();
            }

        }

        protected override void PropagateBehaviour(ElecBlock origin, int x, int y, PropagationOrder order)
        {
            if (order == PropagationOrder.ScanForSource)
            {
                PropagateSearchForSource(origin, x, y);
                return;
            }

            if (order == PropagationOrder.PropagatePower)
            {
                PropagateElec(origin, _enabledState, x, y);
                return;
            }
        }
    }
}
