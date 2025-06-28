namespace DefaultNamespace
{
    public class GameData
    {
        public int currentHeight = 0;
        public float newRowInterval = 7f;
        public float volume=0.8f;
        public bool isMuted=false;
        
        private static GameData _instance;
        public static GameData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameData();
                return _instance;
            }
        }
        
        public void Reset()
        {
            currentHeight = 0;
            newRowInterval = 10f;
        }
        public static void ResetInstance()
        {
            _instance = null;
        }
    }
}