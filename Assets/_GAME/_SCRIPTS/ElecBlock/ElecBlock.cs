using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UPDB.CoreHelper.UsableMethods.Structures;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace TuringSimulator
{
    /// <summary>
    /// object that can be placed on a cell of the grid
    /// </summary>
    public abstract class ElecBlock
    {
        protected Cell _positionCell = null;
        public Cell PositionCell => _positionCell;

        protected List<Cell> _propagateNeighborVisited = new List<Cell>();

        protected static List<ElecBlock> powerSourceFounded = new List<ElecBlock>();

        protected bool _toPropagate = false;

        #region Public API

        public List<Cell> PropagateNeighborVisited
        {
            get { return _propagateNeighborVisited; }
            set { _propagateNeighborVisited = value; }
        }

        public bool ToPropagate
        {
            get => _toPropagate;
            set { _toPropagate = value; }
        }

        #endregion

        /// <summary>
        /// action to do when ElecBlock component is updating
        /// </summary>
        public abstract void UpdateBehaviour();

        /// <summary>
        /// actions to set for a component that is propagating
        /// </summary>
        /// <param name="origin">who sent order to cell, where the order is coming from</param>
        /// <param name="x">x coordinate of neighbor</param>
        /// <param name="y">y coordinate of neighbor</param>
        protected abstract void PropagateBehaviour(ElecBlock origin, int x, int y, PropagationOrder order);

        /// <summary>
        /// manage and send propagation orders, and exluding already visited neighbors
        /// </summary>
        public void Propagate(PropagationOrder order, bool isOrigin)
        {
            for (int i = 0; i < 2; i++)
            {
                //look for neighbors in y -1, y 0, and y 1
                for (int y = -1; y < 2; y++)
                {
                    //look for neighbors in x -1, x 0, and x 1
                    for (int x = -1; x < 2; x++)
                    {
                        //if both x and y are 0, or both are not 0, skip to next position
                        if (!(x != 0 ^ y != 0))
                            continue;

                        //register the position of the new tested cell neighbor
                        int xLinked = _positionCell.Position.x + x;
                        int yLinked = _positionCell.Position.y + y;

                        //if neighbor exceed array bounds, skip to next cell
                        if (xLinked < 0 || yLinked < 0 || xLinked >= _positionCell.LinkedLevel.Width || yLinked >= _positionCell.LinkedLevel.Height)
                            continue;

                        //create neighbor cell
                        Cell neighbor = _positionCell.LinkedLevel.CellsArray[xLinked, yLinked];

                        //break if neighbor is an empty cell
                        if (ReferenceEquals(neighbor.BlockContained, null))
                            continue;

                        if (i == 0)
                        {
                            //break if neighbor has already been visited
                            if (_propagateNeighborVisited.Contains(neighbor))
                                continue;

                            //register current cell into visited list of the to call neighbor and cell neighbor to visited list of current cell
                            neighbor.BlockContained.PropagateNeighborVisited.Add(_positionCell);
                            _propagateNeighborVisited.Add(neighbor);

                            neighbor.BlockContained.ToPropagate = true;

                            continue;
                        }

                        //break if neighbor has already been visited
                        if (!neighbor.BlockContained.ToPropagate)
                            continue;

                        neighbor.BlockContained.ToPropagate = false;

                        //propagate order
                        PropagateBehaviour(this, xLinked, yLinked, order);

                        //once sub propagation trough this cell is done, reset its visited array and the neighbor one
                        neighbor.BlockContained.PropagateNeighborVisited = new List<Cell>();

                    }
                }
            }

            //if origin cell is the first cell that call the order, it means nobody will ever call it, and it's list as to be reinitialized by itself
            if (isOrigin)
                _propagateNeighborVisited = new List<Cell>();
        }

        /// <summary>
        /// propagation order that set every powerable component type to a value
        /// </summary>
        /// <param name="origin">who sent order to cell, where the order is coming from</param>
        /// <param name="elecValue">power value to propagate</param>
        /// <param name="x">x coordinate of neighbor</param>
        /// <param name="y">y coordinate of neighbor</param>
        protected virtual void PropagateElec(ElecBlock origin, bool elecValue, int x, int y)
        {
            //create neighbor of tested cell
            Cell neighbor = _positionCell.LinkedLevel.CellsArray[x, y];

            //if tested eleckBlock is a wire, then set it's value and propagate
            if (neighbor.BlockContained.GetType() == typeof(Wire))
            {
                Wire wire = neighbor.BlockContained as Wire;

                if (wire.PowerOutput != elecValue)
                    wire.PowerOutput = elecValue;

                wire.Propagate(PropagationOrder.PropagatePower, false);
            }
        }

        protected virtual void PropagateSearchForSource(ElecBlock origin, int x, int y)
        {
            //create neighbor of tested cell
            Cell neighbor = _positionCell.LinkedLevel.CellsArray[x, y];

            //if tested eleckBlock is a PowerSource, check if it's currently giving power, if yes, then setthe static value powerSourceFounded at true, wich will cancel the order of updating elec
            if ((neighbor.BlockContained.GetType() == typeof(PowerSource) || neighbor.BlockContained.GetType().BaseType == typeof(PowerSource)))
            {
                PowerSource powerSource = neighbor.BlockContained as PowerSource;

                if (powerSource.EnabledState)
                    powerSourceFounded.Add(powerSource);
            }

            //if testes cell is a wire, just propagate order
            if (neighbor.BlockContained.GetType() == typeof(Wire))
            {
                Wire wire = neighbor.BlockContained as Wire;

                wire.Propagate(PropagationOrder.ScanForSource, false);
            }
        }
    }

    public enum PropagationOrder
    {
        Null,
        CleanPropagationLists,
        PropagatePower,
        ScanForSource,
    }
}


