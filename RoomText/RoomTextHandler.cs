namespace RoomSense
{
    public class RoomTextHandler
    {
        private FontHandler _fontHandler;

        public bool IsReady()
        {
            return _fontHandler.IsFontLoaded();
        }
    }
}