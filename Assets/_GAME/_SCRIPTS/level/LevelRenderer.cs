using System.Collections;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace TuringSimulator
{
    ///<summary>
    /// renderer of levelMap for unityEngine
    ///</summary>
    public class LevelRenderer : Singleton<LevelRenderer>
    {
        private const int _intInfinity = 2147483647;


        [Header("USER PARAMETERS")]
        [SerializeField, Tooltip("")]
        private LevelPreset _levelPreset;


        [Header("LEVEL PARAMETERS")]
        [SerializeField]
        private int _renderDistance = 1000;

        [SerializeField]
        private GameObject _cellObjectPrefab;

        [SerializeField, Tooltip("GameObject that contain every cells of the grid")]
        private GameObject _cellsParentObject;



        [Header("TOOLS PARAMETERS")]
        [SerializeField, Tooltip("")]
        private Camera _gameCamera;

        [SerializeField]
        private BlockTagList _selectedBrush;

        [SerializeField, Tooltip("allow certain actions, such as changing values of eleckblock without updating anything")]
        private bool _debugMode = false;

        private float _timer = 0;

        private CellRenderer[,] _cellRenderersDynamicList = null;

        #region public API

        public BlockTagList SelectedBrush => _selectedBrush;

        public float Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }

        public LevelPreset LvlPreset => _levelPreset;

        public bool DebugMode => _debugMode;

        #endregion

        protected override void Awake()
        {
            base.Awake();

            if (!_levelPreset)
            {
                Debug.LogError("no levelPreset specified");
                return;
            }

            _gameCamera = FindObjectOfType<Camera>();

            int width = Mathf.Clamp(_levelPreset.Level.Width, 0, 10);
            int height = Mathf.Clamp(_levelPreset.Level.Height, 0, 10);
            _gameCamera.transform.position = new Vector3((width / 2f - 0.5f) * transform.localScale.x, (height / 2f - 0.5f) * transform.localScale.y, -100) + transform.position;
            _gameCamera.orthographicSize = (width + height) / 4f;

            if (_cellRenderersDynamicList == null)
                _cellRenderersDynamicList = new CellRenderer[_levelPreset.Level.Width, _levelPreset.Level.Height];
        }

        private void Update()
        {
            if (!_levelPreset)
            {
                Debug.LogError("no levelPreset specified");
                return;
            }

            if (!_levelPreset.Level.IsPaused && !_levelPreset.Level.IsGameOver)
                TimerUpdate();

            PauseInputManager();

            LevelRenderUpdate();
        }

        protected override void OnDrawGizmos()
        {
            if (!_levelPreset)
            {
                Debug.LogError("no levelPreset specified");
                return;
            }

            base.OnDrawGizmos();

            if (Application.isPlaying)
            {
                _levelPreset.IsFirstEditorFrame = true;
            }
            else if (_levelPreset.IsFirstEditorFrame)
            {
                _levelPreset.IsFirstEditorFrame = false;

                if (_levelPreset.SaveInEditor)
                    _levelPreset.SaveCellList();
                else
                    _levelPreset.LoadCellList();
            }
        }

        protected override void OnScene()
        {
            if (_gameCamera == null)
                _gameCamera = FindObjectOfType<Camera>();

            if (_cellsParentObject == null)
            {
                if (transform.Find("Cells") == null)
                {
                    _cellsParentObject = new GameObject("Cells");
                    _cellsParentObject.transform.SetParent(transform);
                }
                else
                {
                    _cellsParentObject = transform.Find("Cells").gameObject;
                }
            }

            _levelPreset.Width = Mathf.Clamp(_levelPreset.Width, 1, _intInfinity);
            _levelPreset.Height = Mathf.Clamp(_levelPreset.Height, 1, _intInfinity);

            int width = Mathf.Clamp(_levelPreset.Level.Width, 0, 10);
            int height = Mathf.Clamp(_levelPreset.Level.Height, 0, 10);
            _gameCamera.transform.position = new Vector3((width / 2f - 0.5f) * transform.localScale.x, (height / 2f - 0.5f) * transform.localScale.y, -100) + transform.position;
            _gameCamera.orthographicSize = (width + height) / 4f;

            int posMinXNearestFromCam = 0;
            int posMinYNearestFromCam = 0;
            int posMaxXNearestFromCam = 0;
            int posMaxYNearestFromCam = 0;

            GenerateVisibleListBorders(ref posMinXNearestFromCam, ref posMinYNearestFromCam, ref posMaxXNearestFromCam, ref posMaxYNearestFromCam, Camera.current, false);

            for (int y = posMinYNearestFromCam; y < posMaxYNearestFromCam; y++)
            {
                for (int x = posMinXNearestFromCam; x < posMaxXNearestFromCam; x++)
                {
                    if (ReferenceEquals(_levelPreset.Level.CellsArray[x, y].BlockContained, null))
                    {
                        Vector2 pos = (new Vector3(x * transform.localScale.x, y * transform.localScale.y) + transform.position);
                        Gizmos.DrawWireCube(pos, Vector2.one * transform.localScale);
                    }
                }
            }

            for (int y = posMinYNearestFromCam; y < posMaxYNearestFromCam; y++)
            {
                for (int x = posMinXNearestFromCam; x < posMaxXNearestFromCam; x++)
                {
                    if (ReferenceEquals(_levelPreset.Level.CellsArray[x, y].BlockContained, null))
                        continue;

                    Color color = Color.white;

                    if (_levelPreset.Level.CellsArray[x, y].BlockContained.GetType() == typeof(Wire))
                        color = Color.red;

                    if (_levelPreset.Level.CellsArray[x, y].BlockContained.GetType() == typeof(Switch))
                        color = Color.black;

                    Color gizmoColor = Gizmos.color;
                    Gizmos.color = color;

                    Vector2 pos = (new Vector3(x * transform.localScale.x, y * transform.localScale.y) + transform.position);
                    Gizmos.DrawCube(pos, Vector2.one * transform.localScale);

                    Gizmos.color = gizmoColor;
                }
            }
        }

        private void LevelRenderUpdate()
        {
            int posMinXNearestFromCam = 0;
            int posMinYNearestFromCam = 0;
            int posMaxXNearestFromCam = 0;
            int posMaxYNearestFromCam = 0;

            GenerateVisibleListBorders(ref posMinXNearestFromCam, ref posMinYNearestFromCam, ref posMaxXNearestFromCam, ref posMaxYNearestFromCam, Camera.main, true);

            for (int i = 0; i < _cellsParentObject.transform.childCount; i++)
            {
                CellRenderer renderer = _cellsParentObject.transform.GetChild(i).GetComponent<CellRenderer>();
                Coords2D pos = renderer.LinkedCell.Position;

                if (pos.x < posMinXNearestFromCam || pos.x >= posMaxXNearestFromCam || pos.y < posMinYNearestFromCam || pos.y >= posMaxYNearestFromCam)
                {
                    _cellRenderersDynamicList[pos.x, pos.y] = null;
                    Destroy(renderer.gameObject);
                }
            }

            for (int y = posMinYNearestFromCam; y < posMaxYNearestFromCam; y++)
            {
                for (int x = posMinXNearestFromCam; x < posMaxXNearestFromCam; x++)
                {
                    if (!_cellRenderersDynamicList[x, y])
                    {
                        GameObject cellPrefab = Instantiate(_cellObjectPrefab, new Vector3(x * transform.localScale.x, y * transform.localScale.y) + transform.position, Quaternion.identity);
                        if (!cellPrefab.TryGetComponent(out CellRenderer cellScript))
                            cellScript = cellPrefab.AddComponent<CellRenderer>();

                        cellPrefab.transform.SetParent(_cellsParentObject.transform);
                        cellScript.LinkedCell = _levelPreset.Level.CellsArray[x, y];

                        _cellRenderersDynamicList[x, y] = cellScript;
                    }
                }
            }

        }

        private void GenerateVisibleListBorders(ref int minX, ref int minY, ref int maxX, ref int maxY, Camera usedCam, bool useOrthoSize)
        {
            minX = Mathf.Clamp(RetrieveXCamOutsideOfCam(-1, usedCam, useOrthoSize) - 1, 0, _levelPreset.Level.Width - 1);
            minY = Mathf.Clamp(RetrieveYCamOutsideOfCam(-1, usedCam, useOrthoSize) - 1, 0, _levelPreset.Level.Height - 1);
            maxX = Mathf.Clamp(RetrieveXCamOutsideOfCam(1, usedCam, useOrthoSize), 0, _levelPreset.Level.Width - 1);
            maxY = Mathf.Clamp(RetrieveYCamOutsideOfCam(1, usedCam, useOrthoSize), 0, _levelPreset.Level.Height - 1);

            int xDistance = maxX - minX;
            int yDistance = maxY - minY;

            int xRenderDistance = (int)(_renderDistance * (usedCam.scaledPixelWidth / 1000f));
            int yRenderDistance = (int)(_renderDistance * (usedCam.scaledPixelHeight / 1000f));

            int xClampValue = (xDistance - xRenderDistance) / 2;
            int yClampValue = (yDistance - yRenderDistance) / 2;

            minX = Mathf.Clamp(minX, minX + xClampValue, maxX - xClampValue);
            minY = Mathf.Clamp(minY, minY + yClampValue, maxY - yClampValue);
            maxX = Mathf.Clamp(maxX, minX + xClampValue, maxX - xClampValue);
            maxY = Mathf.Clamp(maxY, minY + yClampValue, maxY - yClampValue);

            maxX++;
            maxY++;
        }

        public void GraphicsUpdate()
        {
            CellRenderer[] cellRendererArray = FindObjectsOfType<CellRenderer>();

            foreach (CellRenderer cellRenderer in cellRendererArray)
                cellRenderer.GraphicUpdate();
        }

        private void TimerUpdate()
        {
            _timer += Time.deltaTime;
        }

        private void PauseInputManager()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                _levelPreset.Level.PauseSwitch();
        }

        private float GetXCamOutsideOfCam(float posX)
        {
            Vector3 camPos = Camera.current.transform.position;

            if (camPos.z > 0)
                return 999;

            float posXToCam = ((posX - camPos.x) / -camPos.z) / (Camera.current.scaledPixelWidth / 1000f);

            if (posXToCam > -1 && posXToCam < 1)
                return 0;

            return posXToCam;
        }

        private int RetrieveXCamOutsideOfCam(float posXToCam, Camera camUsed, bool useOrthoSize)
        {
            Vector3 camPos = camUsed.transform.position;

            float posX = useOrthoSize ? ((posXToCam * (camUsed.scaledPixelWidth / 1000f)) * (camUsed.orthographicSize * 2.8651f)) + camPos.x : ((posXToCam * (camUsed.scaledPixelWidth / 1000f)) * -camPos.z) + camPos.x;

            return Mathf.RoundToInt(posX);
        }

        private float GetYCamOutsideOfCam(float posY)
        {
            Vector3 camPos = Camera.current.transform.position;

            if (camPos.z > 0)
                return 999;

            float posYToCam = ((posY - camPos.y) / -camPos.z) / ((Camera.current.scaledPixelHeight / 1000f));

            if (posYToCam > -1 && posYToCam < 1)
                return 0;

            return posYToCam;
        }

        private int RetrieveYCamOutsideOfCam(float posYToCam, Camera camUsed, bool useOrthoSize)
        {
            Vector3 camPos = camUsed.transform.position;

            float posY = useOrthoSize ? ((posYToCam * (camUsed.scaledPixelHeight / 1000f)) * (camUsed.orthographicSize * 2.8651f)) + camPos.y : ((posYToCam * (camUsed.scaledPixelHeight / 1000f)) * -camPos.z) + camPos.y;

            return Mathf.RoundToInt(posY);
        }
    }
}
