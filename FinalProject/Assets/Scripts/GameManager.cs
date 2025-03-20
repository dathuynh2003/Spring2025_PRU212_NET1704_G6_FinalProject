using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum CharacterType { None, Knight, Dragon }
    public CharacterType selectedCharacter = CharacterType.None;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // giữ lại khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectKnight()
    {
        selectedCharacter = CharacterType.Knight;
        Debug.Log("Knight selected!");
    }

    public void SelectDragon()
    {
        selectedCharacter = CharacterType.Dragon;
        Debug.Log("Dragon selected!");
    }
}
