using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private GameObject currentPlayer;
    public GameObject knightPlayer;
    public GameObject dragonPlayer;
    public Transform spawnPoint;

    private float knightHealth;
    private float knightDame;

    private float dragonHealth;
    private float dragonDame;

    private bool canSwap = true;

    public CinemachineVirtualCameraBase cinemachineCamera;
    private void Awake()
    {
        currentPlayer = Instantiate(knightPlayer, spawnPoint.position, Quaternion.identity);
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = currentPlayer.transform;
            cinemachineCamera.LookAt = currentPlayer.transform;
        }
        var stats = currentPlayer.GetComponent<IPlayerStats>();
        if (stats != null)
        {
            knightHealth = stats.GetHealth();
            knightDame = stats.GetDame();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))  // Ấn T để hoán đổi
        {
            if (canSwap)
            {
                SwapPlayer();
            }
            else
            {
                Debug.Log("Cannot swap character");
            }
        }
    }

    public void SwapPlayer()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("No current player to swap!");
            return;
        }

        Vector3 curPosition = currentPlayer.transform.position;
        Quaternion curRotation = currentPlayer.transform.rotation;

        // Lưu máu & dame hiện tại
        IPlayerStats stats = currentPlayer.GetComponent<IPlayerStats>();
        if (stats != null)
        {
            if (currentPlayer.name.Contains("Knight"))
            {
                knightHealth = stats.GetHealth();
                knightDame = stats.GetDame();
            }
            else
            {
                dragonHealth = stats.GetHealth();
                dragonDame = stats.GetDame();
            }
        }

        // Xoá player cũ
        Destroy(currentPlayer);

        // Instantiate player mới và gán lại máu/dame
        if (currentPlayer.name.Contains("Knight"))
        {
            currentPlayer = Instantiate(dragonPlayer, curPosition, curRotation);
            IPlayerStats newStats = currentPlayer.GetComponent<IPlayerStats>();
            if (newStats != null)
            {
                newStats.SetHealth(dragonHealth);
                newStats.SetDame(dragonDame);
            }

            Debug.LogFormat("Dragon Heal: {0} | Dame: {1}", dragonHealth, dragonDame);
        }
        else
        {
            currentPlayer = Instantiate(knightPlayer, curPosition, curRotation);
            IPlayerStats newStats = currentPlayer.GetComponent<IPlayerStats>();
            if (newStats != null)
            {
                newStats.SetHealth(knightHealth);
                newStats.SetDame(knightDame);
            }
            Debug.LogFormat("Knight Heal: {0} | Dame: {1}", knightHealth, knightDame);
        }

        // Swap camera to follow new player
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = currentPlayer.transform;
            cinemachineCamera.LookAt = currentPlayer.transform;
        }
    }

    public float CurrentPlayerHealth()
    {
        if (currentPlayer == null)
        {
            return 0;
        }
        if (currentPlayer.name.Contains("Knight"))
        {
            return knightHealth;
        }
        else
        {
            return dragonHealth;
        }
    }

    public Vector3 PlayerPosition()
    {
        return currentPlayer.transform.position;
    }

    public void setCanSwap(bool value)
    {
        canSwap = value;
    }

    public float GetCurrentPlayerHeal()
    {
        if (currentPlayer.name.Contains("Knight"))
        {
            return knightHealth;
        }
        else if (currentPlayer.name.Contains("Dragon"))
        {
            return dragonHealth;
        }
        else
        {
            return 0;
        }
    }
}
