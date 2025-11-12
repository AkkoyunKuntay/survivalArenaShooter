using UnityEngine;

public static class Stats
{
    private const string TotalKillKey = "n_totalKillCount";

    public static int TotalKills
    {
        get => PlayerPrefs.GetInt(TotalKillKey, 0);
        private set { PlayerPrefs.SetInt(TotalKillKey, value); PlayerPrefs.Save(); }
    }

    public static void AddKill(int amount = 1)
    {
        TotalKills = TotalKills + amount;
    }
}