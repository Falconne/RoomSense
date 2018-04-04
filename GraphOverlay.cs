using Verse;

namespace RoomSense
{
    public class GraphOverlay
    {
        private int _nextUpdateTick;

        public void Update(int updateDelay)
        {
            var tick = Find.TickManager.TicksGame;
            if (_nextUpdateTick == 0 || tick >= _nextUpdateTick)
            {
                _nextUpdateTick = tick + updateDelay;
            }
        }

        public void Reset()
        {
            _nextUpdateTick = 0;
        }
    }

}