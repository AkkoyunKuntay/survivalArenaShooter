using TMPro;
using UnityEngine;

public enum CanvasTypes
{
    Start,
    Game,
    Win,
    Fail
}

public class CanvasManager : MonoBehaviour
{
    [SerializeField] CanvasVisibilityController startCanvas;
    [SerializeField] CanvasVisibilityController gameCanvas;
    [SerializeField] CanvasVisibilityController winCanvas;
    [SerializeField] CanvasVisibilityController failCanvas;

    [Header("UI References")] 
    [SerializeField] private TextMeshProUGUI levelIndex;
    [SerializeField] private TextMeshProUGUI totalKill;

    [Header("Debug")] [SerializeField] CanvasVisibilityController activeCanvas;

    private void Start()
    {
        int totalStage = GameManager.instance.GetTotalStagePlayed();
        levelIndex.text = $"LEVEL {totalStage}";;
        
        int totalKillCount = Stats.TotalKills; 
        totalKill.text = $"TOTAL KILL {totalKillCount}";
        
        GameManager.instance.LevelStartedEvent += OnLevelStarted;
        GameManager.instance.LevelFailedEvent += OnLevelFailed;
        GameManager.instance.LevelSuccessEvent += OnLevelSuccessfull;
    }

    private void OnLevelStarted()
    {
        SetActiveCanvas(CanvasTypes.Game);
    }

    private void OnLevelSuccessfull()
    {
        SetActiveCanvas(CanvasTypes.Win);
    }

    private void OnLevelFailed()
    {
        SetActiveCanvas(CanvasTypes.Fail);
    }

    private void SetActiveCanvas(CanvasTypes type)
    {
        activeCanvas.Hide();
        switch (type)
        {
            case CanvasTypes.Start:
                activeCanvas = startCanvas;
                break;
            case CanvasTypes.Game:
                activeCanvas = gameCanvas;
                break;
            case CanvasTypes.Win:
                activeCanvas = winCanvas;
                break;
            case CanvasTypes.Fail:
                activeCanvas = failCanvas;
                break;
            default:
                break;
        }

        activeCanvas.Show();
    }
}