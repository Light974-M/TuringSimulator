using ElectricitySimulator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Propagate();
    }

    protected override void PropagateBehaviour(ElecBlock origin, int x, int y)
    {
        PropagateElec(origin, _enabledState, x, y);
    }
}
