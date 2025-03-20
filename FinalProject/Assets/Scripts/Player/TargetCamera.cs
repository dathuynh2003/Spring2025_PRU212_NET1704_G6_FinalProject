using Unity.Cinemachine;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
    [SerializeField] private KnightController knightPlayer;
    [SerializeField] private DragronController dragonPlayer;
    [SerializeField] private CinemachineVirtualCameraBase cinemachineCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (knightPlayer.isActiveAndEnabled)
        {
            cinemachineCamera.LookAt = knightPlayer.transform;
            cinemachineCamera.Follow = knightPlayer.transform;
        }
        else
        {
            cinemachineCamera.LookAt = dragonPlayer.transform;
            cinemachineCamera.Follow = dragonPlayer.transform;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
