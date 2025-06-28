using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LineClearAnimator : MonoBehaviour
{
    public Animator[] animator;
    public int count=0;
    public int index=0;
    public int gameOverCount=2;
    private bool isGameOverTriggered = false;
    
    private void Start()
    { 
        //确保有Animator组件
       if (animator == null)
       {
           animator = GetComponents<Animator>();
            
           if (animator == null)
           {
               Debug.LogError("LineClearAnimator: No Animator component found!");
               enabled = false;
                return;
           }
       }
        
        // 注册事件监听
        GameEvent.Instance.clearLine.AddListener(PlayAnimation);
    }
    
    private void OnDestroy()
    {
        // 安全取消注册
        if (GameEvent.Instance != null && GameEvent.Instance.clearLine != null)
        {
            GameEvent.Instance.clearLine.RemoveListener(PlayAnimation);
        }
    }
    
    public void PlayAnimation()
    {
        count++;
        
        Debug.Log("a");
        if (animator != null && index < animator.Length && animator[index] != null && count%2==0)
        {
            animator[index].SetInteger("change",index+1);
            index++;
        }
    }

    
    protected virtual IEnumerator GameOverRoutine()
    {
        if (index >= gameOverCount && !isGameOverTriggered)
        {
            isGameOverTriggered = true; // 标记已触发
            
            // 等待两秒
            yield return new WaitForSeconds(2f);
            
            // 修复：添加场景索引检查
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(currentIndex + 1);
            }
            else
            {
                Debug.LogWarning("下一个场景不存在，无法切换");
            }
        }
    }

    private void Update()
    {
        // 每帧检查并启动协程
        if (index >= gameOverCount && !isGameOverTriggered)
        {
            StartCoroutine(GameOverRoutine());
        }
    }
}