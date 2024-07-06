using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace ElectricitySimulator
{
    ///<summary>
    /// renderer for every game cells, making textures, and collision detections
    ///</summary>
    public class CellRenderer : MonoBehaviour
    {
        [Header("INPUT TEXTURES\n")]

        [SerializeField]
        private SpriteRenderer _cellTile;

        private Cell _linkedCell = null;


        #region Public API

        public Cell LinkedCell
        {
            get { return _linkedCell; }
            set { _linkedCell = value; }
        }

        #endregion

        private void Start()
        {
            GraphicUpdate();
        }

        private void FixedUpdate()
        {
            if (!ReferenceEquals(_linkedCell.BlockContained, null) && _linkedCell.BlockContained.GetType() == typeof(Switch))
            {
                GraphicUpdate();
            }

            if (!ReferenceEquals(_linkedCell.BlockContained, null) && _linkedCell.BlockContained.GetType() == typeof(Wire))
            {
                GraphicUpdate();
            }
        }

        private void OnMouseOver()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                LeftMouseEntryAction();

                GraphicUpdate();
                return;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                LeftMouseStayAction();

                GraphicUpdate();
                return;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                RightMouseStayAction();

                GraphicUpdate();
                return;
            }
        }

        public void GraphicUpdate()
        {
            if (ReferenceEquals(_linkedCell.BlockContained, null))
            {
                _cellTile.color = Color.clear;
                return;
            }

            if (_linkedCell.BlockContained.GetType() == typeof(Wire))
            {
                Wire wire = _linkedCell.BlockContained as Wire;

                if (wire.PowerInput)
                    _cellTile.color = new Color(1, 0.2f, 0.2f, 1);
                else
                    _cellTile.color = new Color(0.5f, 0, 0, 1);

                return;
            }

            if (_linkedCell.BlockContained.GetType() == typeof(Switch))
            {
                Switch switchObj = _linkedCell.BlockContained as Switch;

                if (switchObj.EnabledState)
                    _cellTile.color = Color.white;
                else
                    _cellTile.color = Color.black;

                return;
            }

            if (_linkedCell.BlockContained.GetType() == typeof(PowerSource))
            {
                PowerSource powerSource = _linkedCell.BlockContained as PowerSource;

                if (powerSource.EnabledState)
                    _cellTile.color = Color.gray;
                else
                    _cellTile.color = Color.magenta;

                return;
            }
        }

        private void LeftMouseEntryAction()
        {
            if (!ReferenceEquals(_linkedCell.BlockContained, null) && _linkedCell.BlockContained.GetType() == typeof(Switch))
            {
                Switch switchObj = _linkedCell.BlockContained as Switch;

                switchObj.EnabledState = !switchObj.EnabledState;
                switchObj.UpdateBehaviour();
            }
        }

        private void LeftMouseStayAction()
        {
            if (LevelRenderer.Instance.SelectedBrush == BlockTagList.Wire)
            {
                if (ReferenceEquals(_linkedCell.BlockContained, null))
                {
                    OnWireCreate();
                }

                return;
            }

            if (LevelRenderer.Instance.SelectedBrush == BlockTagList.Switch)
            {
                if (ReferenceEquals(_linkedCell.BlockContained, null))
                    OnSwitchCreate();

                return;
            }

            if (LevelRenderer.Instance.SelectedBrush == BlockTagList.PowerSource)
            {
                if (ReferenceEquals(_linkedCell.BlockContained, null))
                    OnPowerSourceCreate();

                return;
            }
        }

        private void RightMouseStayAction()
        {
            if (ReferenceEquals(_linkedCell.BlockContained, null))
                return;

            if (_linkedCell.BlockContained.GetType() == typeof(PowerSource) || _linkedCell.BlockContained.GetType().BaseType == typeof(PowerSource))
            {
                OnPowerSourceDestroy();
                return;
            }

            if (_linkedCell.BlockContained.GetType() == typeof(Wire))
            {
                OnWireDestroy();
                return;
            }

            _linkedCell.BlockContained = null;
            return;
        }

        private void PropagateArround()
        {
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (!(x != 0 ^ y != 0)) continue;

                    int xLinked = _linkedCell.Position.x + x;
                    int yLinked = _linkedCell.Position.y + y;

                    if (xLinked < 0 || yLinked < 0 || xLinked >= _linkedCell.LinkedLevel.Width || yLinked >= _linkedCell.LinkedLevel.Height) continue;

                    PropagateToNeigbor(xLinked, yLinked);
                }
            }
        }

        private void PropagateToNeigbor(int x, int y)
        {
            Cell neighbor = _linkedCell.LinkedLevel.CellsArray[x, y];

            if (ReferenceEquals(neighbor.BlockContained, null))
                return;

            if (neighbor.BlockContained.GetType() == typeof(Wire))
            {
                Wire wire = neighbor.BlockContained as Wire;
                if (wire.PowerInput != false)
                {
                    wire.PowerInput = false;

                    wire.UpdateBehaviour();
                }
            }
        }

        private void OnWireCreate()
        {
            _linkedCell.BlockContained = new Wire(_linkedCell);
            Wire wire = _linkedCell.BlockContained as Wire;
            wire.UpdateBehaviour();
        }

        private void OnWireDestroy()
        {
            Wire wire = _linkedCell.BlockContained as Wire;
            wire.PowerInput = false;
            _linkedCell.BlockContained = null;

            PropagateArround();
        }

        private void OnSwitchCreate()
        {
            _linkedCell.BlockContained = new Switch(_linkedCell);
        }

        private void OnPowerSourceCreate()
        {
            _linkedCell.BlockContained = new PowerSource(_linkedCell);
            PowerSource powerSource = _linkedCell.BlockContained as PowerSource;
            powerSource.UpdateBehaviour();
        }

        private void OnPowerSourceDestroy()
        {
            PowerSource powerSource = _linkedCell.BlockContained as PowerSource;
            powerSource.EnabledState = false;
            powerSource.UpdateBehaviour();

            _linkedCell.BlockContained = null;
        }
    }
}
