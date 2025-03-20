using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    public void PlayGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
