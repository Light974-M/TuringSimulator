using UnityEngine;

namespace TuringSimulator
{
    ///<summary>
    /// parameter of level that will be used between scene loads
    ///</summary>
    [CreateAssetMenu(fileName = "new System", menuName = "ScriptableObjects/System")]
    public class LevelPreset : ScriptableObject
    {
        [SerializeField]
        private bool _saveInEditor = true;

        [SerializeField, Tooltip("width of level in x")]
        private int _width = 3;

        [SerializeField, Tooltip("height of level in y")]
        private int _height = 3;

        [SerializeField]
        private bool _save = false;

        [SerializeField]
        private bool _load = false;

        [SerializeField]
        private bool _erase = false;

        private CellSaveRender[] _cellListSave = null;

        private Level _level = null;

        private int _fixedWidth = 0;

        private int _fixedHeight = 0;

        private bool _isFirstEditorFrame = false;

        #region public API

        public bool IsFirstEditorFrame
        {
            get => _isFirstEditorFrame;
            set => _isFirstEditorFrame = value;
        }

        public bool SaveInEditor
        {
            get => _saveInEditor;
            set => _saveInEditor = value;
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        [System.Serializable]
        public class CellSaveRender
        {
            [SerializeField]
            private Vector2Int _position;

            [SerializeField]
            private BlockTagList _blockContained;

            #region public API

            public Vector2Int Position => _position;

            public BlockTagList BlockContained
            {
                get => _blockContained;
                set => _blockContained = value;
            }

            #endregion

            public CellSaveRender(int x, int y)
            {
                _position = new Vector2Int(x, y);
            }
        }

        //make a get of Level, and make at the same time sure that _level is not null, if it is, it will make a new level
        public Level Level
        {
            get
            {
                if (_level == null || _width != _level.Width || _height != _level.Height)
                {
                    LoadCellList();
                    return _level;
                }

                return _level;
            }
        }

        public CellSaveRender[] CellListSave
        {
            get
            {
                if (_cellListSave == null)
                {
                    SaveCellList();
                }

                return _cellListSave;
            }
        }

        #endregion


        private void OnValidate()
        {
            if (_save)
            {
                SaveCellList();
                _save = false;
            }

            if (_load)
            {
                LoadCellList();
                _load = false;
            }

            if(_erase)
            {
                _level = new Level(_width, _height);

                SaveCellList();
                _erase = false;
            }
        }

        public void SaveCellList()
        {
            _cellListSave = new CellSaveRender[Level.CellsArray.Length];

            _fixedWidth = Level.Width;
            _fixedHeight = Level.Height;

            for (int i = 0; i < _cellListSave.Length; i++)
            {
                int y = i / _width;
                int x = i - (y * _width);
                Cell toWrite = Level.CellsArray[x, y];

                _cellListSave[i] = new CellSaveRender(toWrite.Position.x, toWrite.Position.y);

                if (ReferenceEquals(toWrite.BlockContained, null))
                {
                    _cellListSave[i].BlockContained = BlockTagList.Empty;
                    continue;
                }

                if (toWrite.BlockContained.GetType() == typeof(Wire))
                {
                    _cellListSave[i].BlockContained = BlockTagList.Wire;
                    continue;
                }

                if (toWrite.BlockContained.GetType() == typeof(Switch))
                {
                    _cellListSave[i].BlockContained = BlockTagList.Switch;
                    continue;
                }
            }
        }

        public void LoadCellList()
        {
            _level = new Level(_width, _height, true);

            for (int y = 0; y < _level.Height; y++)
                for (int x = 0; x < _level.Width; x++)
                    _level.CellsArray[x, y] = new Cell(x, y, _level);

            for (int y = 0; y < _fixedHeight; y++)
            {
                for (int x = 0; x < _fixedWidth; x++)
                {
                    int i = x + (_fixedWidth * y);

                    if (x >= _level.Width || y >= _level.Height)
                        continue;

                    if (CellListSave[i].BlockContained == BlockTagList.Empty)
                        continue;

                    if (CellListSave[i].BlockContained == BlockTagList.Wire)
                    {
                        _level.CellsArray[x, y].BlockContained = new Wire(_level.CellsArray[x, y]);
                        continue;
                    }

                    if (CellListSave[i].BlockContained == BlockTagList.Switch)
                    {
                        _level.CellsArray[x, y].BlockContained = new Switch(_level.CellsArray[x, y]);
                        continue;
                    }
                }
            }

            SaveCellList();
        }
    }
}