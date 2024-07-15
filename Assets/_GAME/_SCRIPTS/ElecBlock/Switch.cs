namespace TuringSimulator
{
    public class Switch : PowerSource
    {
        public Switch(Cell positionCell) : base(positionCell)
        {
            _enabledState = false;
        }
    } 
}
