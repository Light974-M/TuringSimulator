namespace TuringSimulator
{
    ///<summary>
    /// represent cells epxloitable into a 2D grid
    ///</summary>
    public class Cell
    {
        private Level _linkedLevel;

        private Coords2D _position;

        private ElecBlock _blockContained;

        #region public API

        public Coords2D Position => _position;

        public ElecBlock BlockContained
        {
            get => _blockContained;
            set => _blockContained = value;
        }

        public Level LinkedLevel
        {
            get => _linkedLevel; 
            set => _linkedLevel = value;
        }

        #endregion

        public Cell(int x, int y, Level linkedLevel)
        {
            _position = new Coords2D(x, y);
            _linkedLevel = linkedLevel;
        }
    } 
}
