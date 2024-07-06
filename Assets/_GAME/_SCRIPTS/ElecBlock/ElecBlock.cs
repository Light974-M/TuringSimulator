using ElectricitySimulator;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// object that can be placed on a cell of the grid
/// </summary>
public abstract class ElecBlock
{
    protected Cell _positionCell = null;
    public Cell PositionCell => _positionCell;

    public abstract void UpdateBehaviour();

    protected abstract void PropagateBehaviour(ElecBlock origin, int x, int y);

    protected void Propagate()
    {
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x != 0 ^ y != 0)) continue;

                int xLinked = _positionCell.Position.x + x;
                int yLinked = _positionCell.Position.y + y;

                if (xLinked < 0 || yLinked < 0 || xLinked >= _positionCell.LinkedLevel.Width || yLinked >= _positionCell.LinkedLevel.Height) continue;

                PropagateBehaviour(this, xLinked, yLinked);
            }
        }
    }

    protected virtual void PropagateElec(ElecBlock origin, bool elecValue, int x, int y)
    {
        Cell neighbor = _positionCell.LinkedLevel.CellsArray[x, y];
        
        if (ReferenceEquals(neighbor.BlockContained, null))
            return;

        if((neighbor.BlockContained.GetType() == typeof(PowerSource) || neighbor.BlockContained.GetType().BaseType == typeof(PowerSource)) && !elecValue)
        {
            PowerSource powerSource = neighbor.BlockContained as PowerSource;

            if (powerSource.EnabledState)
            {
                powerSource.UpdateBehaviour();
            }
        }

        if (neighbor.BlockContained.GetType() == typeof(Wire))
        {
            Wire wire = neighbor.BlockContained as Wire;
            if (wire.PowerInput != elecValue)
            {
                wire.PowerInput = elecValue;

                wire.UpdateBehaviour();
            }
        }
    }
}


