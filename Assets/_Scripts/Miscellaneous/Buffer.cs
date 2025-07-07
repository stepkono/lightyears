namespace _Scripts
{
    public static class Buffer
    {
        public static int investedSeconds = 0;

        public static void AddToBuffer(int seconds)
        {
            investedSeconds += seconds;
        }
    }
}