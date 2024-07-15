using System.Collections.Generic;

namespace TuringSimulator
{
    public class Wire : ElecBlock
    {
        private bool _powerOutput = false;

        #region Public API

        public bool PowerOutput
        {
            get => _powerOutput;
            set => _powerOutput = value;
        }

        #endregion

        public Wire(Cell positionCell)
        {
            _powerOutput = false;
            _positionCell = positionCell;
        }

        public override void UpdateBehaviour()
        {
            Propagate(PropagationOrder.PropagatePower, true);

            powerSourceFounded = new List<ElecBlock>();
            Propagate(PropagationOrder.ScanForSource, true);
        }

        protected override void PropagateBehaviour(ElecBlock origin, int x, int y, PropagationOrder order)
        {
            if (order == PropagationOrder.PropagatePower)
            {
                PropagateElec(origin, _powerOutput, x, y);
                return;
            }

            if (order == PropagationOrder.ScanForSource)
            {
                PropagateSearchForSource(origin, x, y);
                return;
            }
        }

        public void CallLinkedPowerSourceUpdates()
        {
            if (powerSourceFounded.Count == 0)
                return;

            for (int i = 0; i < powerSourceFounded.Count; i++)
                powerSourceFounded[i].UpdateBehaviour();
        }
    }
}
