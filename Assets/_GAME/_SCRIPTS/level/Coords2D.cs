namespace TuringSimulator
{
    ///<summary>
    /// coordinate system for Cells, objects, and every native classes that need to be represented into a position in 2D
    ///</summary>
    public struct Coords2D
    {
        public int x;
        public int y;

        public Coords2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Coords2D Zero
        {
            get { return new Coords2D(0, 0); }
        }

        public static Coords2D One
        {
            get { return new Coords2D(1, 1); }
        }

        public static Coords2D ZeroOne
        {
            get { return new Coords2D(0, 1); }
        }

        public static Coords2D OneZero
        {
            get { return new Coords2D(1, 0); }
        }

        public static Coords2D Ten
        {
            get { return new Coords2D(10, 10); }
        }
    } 
}
