using UnityEngine;

[System.Serializable]
public class LevelRuntimeConfig
{
    public int waves;
    public int enemiesPerWave;
    public int enemyDamage;
}

public class LevelManager : CustomSingleton<LevelManager>
{
    [Header("Settings")] [SerializeField] private float levelTime = 90f;
    [SerializeField] private LevelSO levelData;
    public LevelRuntimeConfig RuntimeConfig;

    [Header("Debug")] [SerializeField] private bool timerActive;
    [SerializeField] private float timer;

    public System.Action TimerFinishedEvent;

    public float GetTimer() { return timer; }
    protected override void Awake()
    {
        base.Awake();
        timer = levelTime;
    }

    private void Start()
    {
        int stageCount = GameManager.instance.GetTotalStagePlayed();
        levelData.EvaluateForStage(stageCount, out int waves, out int enemiesPerWave, out int enemyDamage);
        RuntimeConfig.waves = waves;
        RuntimeConfig.enemiesPerWave = enemiesPerWave;
        RuntimeConfig.enemyDamage = enemyDamage;

        GameManager.instance.LevelStartedEvent += OnLevelStarted;
    }

    private void OnLevelStarted()
    {
        SetTimerStatus(true);
    }

    private void SetTimerStatus(bool isEnabled)
    {
        timerActive = isEnabled;
    }

    private void Update()
    {
        if (!timerActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            TimerFinishedEvent?.Invoke();
            SetTimerStatus(false);
            Debug.Log("Time is Up!");
            GameManager.instance.EndGame(true);
        }
    }
}