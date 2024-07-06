using ElectricitySimulator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : PowerSource
{
    #region Public API

    public bool EnabledState
    {
        get => _enabledState;
        set => _enabledState = value;
    }

    #endregion

    public Switch(Cell positionCell) : base(positionCell)
    {
        _enabledState = false;
    }

    public override void UpdateBehaviour()
    {
        Propagate();
    }

    protected override void PropagateBehaviour(ElecBlock origin, int x, int y)
    {
        PropagateElec(origin, _enabledState, x, y);
    }
}
