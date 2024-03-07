namespace Utility
{
    public class SingletonBase<T> where T : new()
    {
        private static T _instance;
        private static readonly object Locker = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }
                }
                return _instance;
            }
        }
    }
}