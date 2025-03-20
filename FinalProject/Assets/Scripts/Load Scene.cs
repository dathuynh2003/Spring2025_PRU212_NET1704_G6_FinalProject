using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Button button;
    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
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
}
