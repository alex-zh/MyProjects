namespace Robot.Core
{
    public class RobotIterationState
    {
        public bool ShouldOpen
        {
            get; set;
        }

        public bool ShouldClose
        {
            get; set;
        }

        public void Reset()
        {
            ShouldClose = false;
            ShouldOpen = false;
        }
    }
}