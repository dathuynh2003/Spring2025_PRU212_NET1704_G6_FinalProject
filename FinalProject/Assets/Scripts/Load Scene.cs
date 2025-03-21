using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Button button;
    [SerializeField] private Button backToMainButton;
    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else if (backToMainButton != null)
        {
            {
                backToMainButton.onClick.AddListener(OnBackToMainClicked);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnButtonClicked()
    {
        Debug.Log("Play Clicked! Loading MainGameScene...");
        SceneManager.LoadScene("CharacterSelectUI", LoadSceneMode.Single);
    }
    void OnBackToMainClicked()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }
}
