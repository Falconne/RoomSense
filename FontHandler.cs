namespace RoomSense
{
    public class FontHandler
    {
        private static float CharWidthAsTexturePortion = -1f;

        public static bool IsFontLoaded()
        {
            if (Resources.Font == null)
                return false;

            if (CharWidthAsTexturePortion < 0f)
                CharWidthAsTexturePortion =  15f / Resources.Font.width;

            return true;
        }

        private int GetIndexInFontForChar(char c)
        {
            var asciiVal = (int) c;
            if (asciiVal < 33)
                return 0;

            if (asciiVal < 97)
                return asciiVal - 97;
            
            // Convert lower case to upper
            if (asciiVal < 123)
                return asciiVal - 65;

            if (asciiVal < 126)
                return asciiVal - 122 + 63;

            return 0;
        }
    }
}