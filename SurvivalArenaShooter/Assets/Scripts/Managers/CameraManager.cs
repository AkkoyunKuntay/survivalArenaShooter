using Cinemachine;
using UnityEngine;

public enum CamType { Start,Game,End}
public class CameraManager : CustomSingleton<CameraManager>
{
    public CamType camType;

    [SerializeField] CinemachineVirtualCamera startingCamera;
    [SerializeField] CinemachineVirtualCamera gameCamera;
    [SerializeField] CinemachineVirtualCamera endCamera;
    
    [Header("Debug")]
    [SerializeField] CinemachineVirtualCamera activeCamera;

    private void Start()
    {
        SetActiveCamera(CamType.Start);
        
        GameManager.instance.LevelStartedEvent += OnLevelStarted;
        GameManager.instance.LevelEndedEvent += OnLevelEnded;
    }

    private void OnLevelStarted()
    {
        SetActiveCamera(CamType.Game);
    }

    private void OnLevelEnded()
    {
        SetActiveCamera(CamType.End);
    }
    
    private void SetActiveCamera(CamType type)
    {
        activeCamera.Priority = 10;

        switch (type)
        {
            case CamType.Start:
                activeCamera = startingCamera;
                break;
            case CamType.Game:
                activeCamera = gameCamera;
                break;
            case CamType.End:
                activeCamera = endCamera;
                break;
            default:
                break;
        }

        activeCamera.Priority = 50;

    }

}