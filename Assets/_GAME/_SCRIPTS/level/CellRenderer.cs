using UnityEngine;
using UPDB.CoreHelper.UsableMethods.Structures;

namespace TuringSimulator
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

        private Trigger _leftClickEnterTrigger;
        private Trigger _leftClickStayTrigger;
        private Trigger _rightClickStayTrigger;


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
            GraphicUpdate();
        }

        private void OnMouseOver()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                LeftMouseEntryAction();
                return;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                LeftMouseStayAction();
                return;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                RightMouseStayAction();
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

                if (wire.PowerOutput)
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
            if (ReferenceEquals(_linkedCell.BlockContained, null))
            {
                return;
            }

            if (_linkedCell.BlockContained.GetType() == typeof(Switch))
            {
                OnSwitchClicked();
                return;
            }

            if (_linkedCell.BlockContained.GetType() == typeof(Wire))
            {
                OnWireClicked();
                return;
            }
        }

        private void LeftMouseStayAction()
        {
            if (LevelRenderer.Instance.SelectedBrush == BlockTagList.Wire)
            {
                if (ReferenceEquals(_linkedCell.BlockContained, null))
                    OnWireCreate();

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

        private void OnWireCreate()
        {
            _linkedCell.BlockContained = new Wire(_linkedCell);

            if (LevelRenderer.Instance.DebugMode)
                return;

            Wire wire = _linkedCell.BlockContained as Wire;
            wire.UpdateBehaviour();
            wire.CallLinkedPowerSourceUpdates();
        }

        private void OnWireDestroy()
        {
            Wire wire = _linkedCell.BlockContained as Wire;
            wire.PowerOutput = false;

            wire.UpdateBehaviour();

            _linkedCell.BlockContained = null;

            wire.CallLinkedPowerSourceUpdates();
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

        private void OnSwitchClicked()
        {
            Switch switchObj = _linkedCell.BlockContained as Switch;

            switchObj.EnabledState = !switchObj.EnabledState;

            if (LevelRenderer.Instance.DebugMode)
                return;

            switchObj.UpdateBehaviour();
        }

        private void OnWireClicked()
        {
            if (LevelRenderer.Instance.DebugMode)
            {
                Wire wire = _linkedCell.BlockContained as Wire;
                wire.PowerOutput = !wire.PowerOutput;
            }
        }
    }
}
