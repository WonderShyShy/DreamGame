using Other;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {

        EnterGame();
    }

    private void EnterGame()
    {
        var sceneVersion = "3";
        SceneManager.LoadScene("GameScene" + sceneVersion);
    }
}
