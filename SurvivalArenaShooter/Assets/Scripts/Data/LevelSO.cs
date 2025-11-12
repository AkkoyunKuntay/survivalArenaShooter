using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelSO : ScriptableObject
{
    [Header("Identity")]
    public string levelName;
    public int levelIndex;
    
    [Header("Base Values")]
    public int baseWaves = 3;
    public int baseEnemiesPerWave = 5;
    public int baseEnemyDamage = 5;

    [Header("Linear Growth Per Stage (simple)")]
    public float wavesGrowthPerStage = 0.5f;
    public float enemiesGrowthPerStage = 0.1f;
    public float damageGrowthPerStage = 0.08f;

    [Header("Optional Curves (overrides linear if assigned)")]
    public AnimationCurve wavesByStage;
    public AnimationCurve enemiesByStage;
    public AnimationCurve damageByStage; 

    [Header("Clamps")]
    public int maxWaves = 20;
    public int maxEnemiesPerWave = 100;
    public int maxEnemyDamage = 999;

    public void EvaluateForStage(int stageCount, out int waves, out int enemiesPerWave, out int enemyDamage)
    {
        // Waves
        if (wavesByStage != null && wavesByStage.keys.Length > 0)
            waves = Mathf.RoundToInt(baseWaves + wavesByStage.Evaluate(stageCount));
        else
            waves = Mathf.RoundToInt(baseWaves + stageCount * wavesGrowthPerStage);

        // Enemies per wave
        if (enemiesByStage != null && enemiesByStage.keys.Length > 0)
            enemiesPerWave = Mathf.RoundToInt(baseEnemiesPerWave * Mathf.Max(1f, enemiesByStage.Evaluate(stageCount)));
        else
            enemiesPerWave = Mathf.RoundToInt(baseEnemiesPerWave * (1f + stageCount * enemiesGrowthPerStage));

        // Enemy damage
        if (damageByStage != null && damageByStage.keys.Length > 0)
            enemyDamage = Mathf.RoundToInt(baseEnemyDamage * Mathf.Max(1f, damageByStage.Evaluate(stageCount)));
        else
            enemyDamage = Mathf.RoundToInt(baseEnemyDamage * (1f + stageCount * damageGrowthPerStage));

        // clamps
        waves = Mathf.Clamp(waves, 1, maxWaves);
        enemiesPerWave = Mathf.Clamp(enemiesPerWave, 1, maxEnemiesPerWave);
        enemyDamage = Mathf.Clamp(enemyDamage, 1, maxEnemyDamage);
    }
}
