namespace RoomSense
{
    class Label
    {
        //Vector
    }

    public class LabelPlacementHandler
    {
        private FontHandler _fontHandler;

        public bool IsReady()
        {
            return _fontHandler.IsFontLoaded();
        }
    }
}