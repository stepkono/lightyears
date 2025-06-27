namespace _Scripts
{
    public static class AppEvents
    {
        public delegate void HobbyLaunched(); 
        public static event HobbyLaunched OnHobbyLaunched;

        public static void RaiseHobbyLaunched()
        {
            OnHobbyLaunched?.Invoke();
        }
    }
}   