using Unity.Cinemachine;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject dragonPrefab;

    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;

    [Header("Cinemachine Camera")]
    [SerializeField] private CinemachineVirtualCameraBase cinemachineCamera;

    private GameObject currentPlayer; // giữ player hiện tại

    void Start()
    {
        SetupCharacter();
    }

    private void SetupCharacter()
    {
        // Kiểm tra nhân vật đã chọn trong GameManager
        switch (GameManager.Instance.selectedCharacter)
        {
            case GameManager.CharacterType.Knight:
                SpawnCharacter(knightPrefab);
                break;

            case GameManager.CharacterType.Dragon:
                SpawnCharacter(dragonPrefab);
                break;

            default:
                Debug.LogWarning("No character selected!");
                break;
        }
    }

    private void SpawnCharacter(GameObject characterPrefab)
    {
        if (characterPrefab == null)
        {
            Debug.LogError("Character prefab is null!");
            return;
        }

        // Nếu đã có player cũ thì xóa đi
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        } 
        else
        {
            Debug.Log("currentPlayer null");

        }

        // Spawn prefab tại spawnPoint
        currentPlayer = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("CurrentPlayer: " + currentPlayer.name);

        // Set Camera Follow + LookAt
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = currentPlayer.transform;
            cinemachineCamera.LookAt = currentPlayer.transform;
        }
        else
        {
            Debug.LogError("Cinemachine Camera chưa gán!");
        }
    }
}
