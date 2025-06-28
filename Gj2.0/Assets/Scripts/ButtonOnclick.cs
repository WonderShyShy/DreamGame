using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class ButtonOnclick:MonoBehaviour
    {
        public GameObject obj;
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif 
        }
        public  void Restart()
        {
            // 修复：确保时间缩放恢复正常
            Time.timeScale = 1f;
            
            // 重置游戏数据
            GameData.Instance.Reset();
            
            // 修复：重置事件系统实例
            GameEvent.ResetInstance();
    
            // 重置网格
            TetrisBlock.ResetGrid();
    
            // 修复：添加场景索引检查
            var currentIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentIndex > 0)
            {
                SceneManager.LoadScene(currentIndex - 1);
            }
            else
            {
                // 如果当前是第一个场景，重新加载当前场景
                SceneManager.LoadScene(currentIndex);
            }
        }

        public void SetObjActive()
        {
            Time.timeScale = 0;
            obj.gameObject.SetActive(true);
        }

        public void SetObjUnActive()
        {
            Time.timeScale = 1;
            obj.gameObject.SetActive(false);
        }

        public void Back()
        {
            // 修复：确保时间缩放恢复正常
            Time.timeScale = 1f;
            
            var currentIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentIndex > 0)
            {
                SceneManager.LoadScene(currentIndex - 1);
            }
        }

        public void Next()
        {
            // 修复：确保时间缩放恢复正常  
            Time.timeScale = 1f;
            
            var currentIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(currentIndex + 1);
            }
        }
    }
    
}