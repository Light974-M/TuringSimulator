using ElectricitySimulator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : ElecBlock
{
    private bool _powerOutput = false;

    #region Public API

    public bool PowerInput
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
        Propagate();
    }

    protected override void PropagateBehaviour(ElecBlock origin, int x, int y)
    {
        PropagateElec(origin, _powerOutput, x, y);
    }
}
