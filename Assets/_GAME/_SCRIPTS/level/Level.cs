using UnityEngine;

namespace ElectricitySimulator
{
    ///<summary>
    /// Level GridMap, represented by a 2D Array filled with cells classes.
    ///</summary>
    public class Level
    {
        #region variables

        private Cell[,] _cellsArray = null;

        private int _width = 3;

        private int _height = 3;

        private bool _isGameOver = false;

        private bool _hasWin = false;

        private bool _isPaused = false;

        #endregion


        #region public API

        public int Width => _width;
        public int Height => _height;
        public Cell[,] CellsArray
        {
            get
            {
                return _cellsArray;
            }
        }

        public bool IsGameOver
        {
            get { return _isGameOver; }
            set { _isGameOver = value; }
        }

        public bool HasWin
        {
            get { return _hasWin; }
            set { _hasWin = value; }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set { _isPaused = value; }
        }

        #endregion

        /// <summary>
        /// Constructor for level map
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Level(int width, int height)
        {
            _width = width;
            _height = height;

            _cellsArray = new Cell[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _cellsArray[x, y] = new Cell(x, y, this);
                }
            }
        }

        /// <summary>
        /// Constructor for level map
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Level(int width, int height, bool dontBuildList)
        {
            _width = width;
            _height = height;

            _cellsArray = new Cell[width, height];

            if (!dontBuildList)
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        _cellsArray[x, y] = new Cell(x, y, this);
        }

        private void Win()
        {
            IsGameOver = true;
            _hasWin = true;

            GameOverApply();
        }

        private void Loose()
        {
            IsGameOver = true;
            _hasWin = false;

            GameOverApply();
        }

        private void GameOverApply()
        {
            //insert code for finishing the game
        }

        public void PauseSwitch()
        {
            _isPaused = !_isPaused;
        }
    }
}