namespace _Scripts
{
    public static class AppEvents
    {
        public delegate void HobbyLaunched(bool launched); 
        public static event HobbyLaunched OnHobbyLaunched;

        public static void RaiseHobbyLaunched(bool launched)
        {
            OnHobbyLaunched?.Invoke(launched);
        }
    }
}   