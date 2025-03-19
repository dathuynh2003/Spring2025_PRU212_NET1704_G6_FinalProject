using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuu : MonoBehaviour
{
    public GameObject menupanel;
    public GameObject guidepanel;
    private void Start()
    {
        menupanel.SetActive(true);
        guidepanel.SetActive(false);
    }
    public void PlayGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void GoToGuideMenu()
    {
        menupanel.SetActive(false);
        guidepanel.SetActive(true);
    }
    public void GoToMainMenu()
    {
        menupanel.SetActive(true);
        guidepanel.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
